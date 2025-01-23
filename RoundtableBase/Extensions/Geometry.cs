using System.Numerics;

namespace RoundtableBase.Extensions;

public static class Geometry
{
    /// <summary>
    /// Get the squared length of a Vector in the XZ (FromSoft ground) plane distance.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static float GetXZSquaredLength(this Vector3 v)
    {
        float xSq = MathF.Pow(v.X, 2);
        float zSq = MathF.Pow(v.Z, 2);
        return xSq + zSq;
    }
    
    /// <summary>
    /// Slide a position sideways by `distance` units, relative to a 'facing ' Y rotation angle in degrees.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotationY"></param>
    /// <param name="distance"></param>
    /// <param name="offsetY"></param>
    /// <returns></returns>
    public static Vector3 SlideSideways(this Vector3 position, float rotationY, float distance, float offsetY = 0f)
    {
        // Convert to radians and negate to correct for LHS coordinate system.
        float radians = -rotationY * MathF.PI / 180f;

        // Calculate orthogonal vector.
        float deltaX = (float)Math.Cos(radians + Math.PI / 2) * distance;
        float deltaZ = (float)Math.Sin(radians + Math.PI / 2) * distance;

        // Add to position.
        return new Vector3(position.X + deltaX, position.Y + offsetY, position.Z + deltaZ);
    }
    
    public static Vector3 SlideForwards(this Vector3 position, float rotationY, float distance, float offsetY = 0f)
    {
        // Convert to radians and negate to correct for LHS coordinate system.
        float radians = -rotationY * MathF.PI / 180f;

        // Calculate forward vector.
        float deltaX = (float)Math.Cos(radians) * distance;
        float deltaZ = (float)Math.Sin(radians) * distance;

        // Add to position.
        return new Vector3(position.X + deltaX, position.Y + offsetY, position.Z + deltaZ);
    }

    /// <summary>
    /// Correct an angle to be within the half-open range [-180, 180).
    /// </summary>
    /// <param name="angleDeg"></param>
    /// <returns></returns>
    public static float CorrectAngleRange(this float angleDeg)
    {
        while (angleDeg >= 180f)
            angleDeg -= 360f;
        while (angleDeg < -180f)
            angleDeg += 360f;
        return angleDeg;
    }
    
    /// <summary>
    /// Get XZ angle of `b` from `a`, in degrees by default.
    ///
    /// Setting enemy/asset with position `a` to this Y rotation will make it face `b`. Note standard negation for LHS
    /// and order of `Atan2` arguments to reflect that enemies "face" their negative Z axis.
    ///
    /// Reasoning:
    ///     In normal usage, atan(y, x) gives you the angle from the positive X axis toward the positive Y axis.
    ///     However, for "facing", we consider the negative Z axis as the "forward" direction. If an enemy was placed in
    ///     the world with rotation (0, 0, 0), they would be facing "south" (negative Z), and so a point with coords
    ///     (0, -N) directly south of them would have an angle of 0 degrees.
    ///     And since degrees increase clockwise in this system, we want atan(-x, -z). This is like saying "zero is at
    ///     -Z axis" and "invert the target X axis".
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="radians"></param>
    /// <returns></returns>
    public static float GetFacingAngleY(this Vector3 a, Vector3 b, bool radians = false)
    {
        Vector3 aToB = b - a;
        float angleRad = MathF.Atan2(-aToB.X, -aToB.Z);
        return radians ? angleRad : 180f / MathF.PI * angleRad;
    }
    
    /// <summary>
    /// Construct 4x4 rotation matrix by multiplying Euler angles together in XZY order (used by FromSoft).
    /// </summary>
    /// <param name="eulerXZY"></param>
    /// <param name="radians"></param>
    /// <returns></returns>
    public static Matrix4x4 EulerAnglesTo4x4(Vector3 eulerXZY, bool radians = false)
    {
        if (!radians)
            eulerXZY = MathF.PI / 180f * eulerXZY;
        float sx = MathF.Sin(eulerXZY.X);
        float sy = MathF.Sin(eulerXZY.Y);
        float sz = MathF.Sin(eulerXZY.Z);
        float cx = MathF.Cos(eulerXZY.X);
        float cy = MathF.Cos(eulerXZY.Y);
        float cz = MathF.Cos(eulerXZY.Z);
        Matrix4x4 mX = new(
            1f, 0f, 0f, 0f,
            0f, cx, -sx, 0f,
            0f, sx, cx, 0f,
            0f, 0f, 0f, 1f);
        Matrix4x4 mY = new(
            cy, 0f, sy, 0f,
            0f, 1f, 0f, 0f,
            -sy, 0f, cy, 0f,
            0f, 0f, 0f, 1f);
        Matrix4x4 mZ = new(
            cz, -sz, 0f, 0f,
            sz, cz, 0f, 0f,
            0f, 0f, 1f, 0f,
            0f, 0f, 0f, 1f);
        return mY * mZ * mX;
    }
    
    /// <summary>
    /// Convert a 4x4 rotation matrix to Euler angles in XZY (FromSoft) order.
    ///
    /// Only looks at the upper-left 3x3 submatrix.
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="radians"></param>
    /// <returns></returns>
    public static Vector3 ToEulerAngles(this Matrix4x4 matrix, bool radians = false)
    {
        float x, y, z;
        if (matrix.M21 < 1f)
        {
            if (matrix.M21 > -1f)
            {
                // Unique solution.
                z = MathF.Asin(matrix.M21);
                y = MathF.Atan2(-matrix.M31, matrix.M11);
                x = MathF.Atan2(-matrix.M23, matrix.M22);
            }
            else
            {
                // Not a unique solution: x - y = Atan2(M32, M33)
                z = -MathF.PI / 2;
                y = -MathF.Atan2(matrix.M32, matrix.M33);
                x = 0f;
            }
        }
        else
        {
            // Not a unique solution: x + y = Atan2(M32, M33)
            z = MathF.PI / 2;
            y = MathF.Atan2(matrix.M32, matrix.M33);
            x = 0f;
        }
        
        Vector3 eulerRad = new(x, y, z);
        return radians ? eulerRad : 180f / MathF.PI * eulerRad;
    }
}