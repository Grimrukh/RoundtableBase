using System.Runtime.InteropServices;

namespace RoundtableBase.Extensions;

public static class Memory
{
    /// <summary>
    /// Convert a C# struct to its underlying byte array.
    /// </summary>
    /// <param name="myStruct"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static byte[] GetBytes<T>(this T myStruct) where T : struct 
    {
        int size = Marshal.SizeOf(myStruct);
        byte[] bytes = new byte[size];

        IntPtr ptr = IntPtr.Zero;
        try
        {
            ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(myStruct, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return bytes;
    }
}