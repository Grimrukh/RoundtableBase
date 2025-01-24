using System.Text;

using PropertyHook;

namespace RoundtableBase.Memory;

/// <summary>
/// Wraps a `PHPointer` (or child thereof) from `PropertyHook` for use as a base class for Roundtable in-memory classes.
/// </summary>
/// <param name="pointer"></param>
public abstract class RTPointer(PHPointer pointer)
{
    public PHPointer Pointer { get; } = pointer;

    public bool IsValid => Pointer.IsNonZero;
    
    /// <summary>
    /// Create a child pointer with the given nested sequence of offsets.
    /// </summary>
    /// <param name="offsets"></param>
    /// <returns></returns>
    public PHPointer CreateChildPointer(params int[] offsets)
    {
        return Pointer.CreateChildPointer(offsets);
    }
    
    /// <summary>
    /// Read `length` number of bytes from the given offset.
    /// </summary>
    public byte[] ReadBytes(int offset, uint length)
    {
        return Pointer.ReadBytes(offset, length);
    }
    /// <summary>
    /// Write `length` number of bytes at the given offset.
    /// </summary>
    public bool WriteBytes(int offset, byte[] bytes)
    {
        return Pointer.WriteBytes(offset, bytes);
    }
    /// <summary>
    /// Read an address from the given offset. (There is no Write version of this method.)
    /// </summary>
    public IntPtr ReadIntPtr(int offset)
    {
        return Pointer.ReadIntPtr(offset);
    }
    /// <summary>
    /// Read a 4-byte bitfield and return whether the bit specified by the given mask is set.
    /// </summary>
    public bool ReadFlag32(int offset, uint mask)
    {
        return Pointer.ReadFlag32(offset, mask);
    }
    /// <summary>
    /// Set the state of a bit specified by the given mask in a 4-byte bitfield.
    /// </summary>
    public bool WriteFlag32(int offset, uint mask, bool state)
    {
        return Pointer.WriteFlag32(offset, mask, state);
    }
    /// <summary>
    /// Read a 1-byte signed integer.
    /// </summary>
    public sbyte ReadSByte(int offset)
    {
        return Pointer.ReadSByte(offset);
    }
    /// <summary>
    /// Write a 1-byte signed integer.
    /// </summary>
    public bool WriteSByte(int offset, sbyte value)
    {
        return Pointer.WriteSByte(offset, value);
    }
    /// <summary>
    /// Read a 1-byte unsigned integer.
    /// </summary>
    public byte ReadByte(int offset)
    {
        return Pointer.ReadByte(offset);
    }
    /// <summary>
    /// Write a 1-byte unsigned integer.
    /// </summary>
    public bool WriteByte(int offset, byte value)
    {
        return Pointer.WriteByte(offset, value);
    }
    /// <summary>
    /// Read a 1-byte boolean value.
    /// </summary>
    public bool ReadBoolean(int offset)
    {
        return Pointer.ReadBoolean(offset);
    }
    /// <summary>
    /// Write a 1-byte boolean value.
    /// </summary>
    public bool WriteBoolean(int offset, bool value)
    {
        return Pointer.WriteBoolean(offset, value);
    }
    /// <summary>
    /// Read a 2-byte signed integer.
    /// </summary>
    public short ReadInt16(int offset)
    {
        return Pointer.ReadInt16(offset);
    }
    /// <summary>
    /// Write a 2-byte signed integer.
    /// </summary>
    public bool WriteInt16(int offset, short value)
    {
        return Pointer.WriteInt16(offset, value);
    }
    /// <summary>
    /// Read a 2-byte unsigned integer.
    /// </summary>
    public ushort ReadUInt16(int offset)
    {
        return Pointer.ReadUInt16(offset);
    }
    /// <summary>
    /// Write a 2-byte unsigned integer.
    /// </summary>
    public bool WriteUInt16(int offset, ushort value)
    {
        return Pointer.WriteUInt16(offset, value);
    }
    /// <summary>
    /// Read a 4-byte signed integer.
    /// </summary>
    public int ReadInt32(int offset)
    {
        return Pointer.ReadInt32(offset);
    }
    /// <summary>
    /// Write a 4-byte signed integer.
    /// </summary>
    public bool WriteInt32(int offset, int value)
    {
        return Pointer.WriteInt32(offset, value);
    }
    /// <summary>
    /// Read a 4-byte unsigned integer.
    /// </summary>
    public uint ReadUInt32(int offset)
    {
        return Pointer.ReadUInt32(offset);
    }
    /// <summary>
    /// Write a 4-byte unsigned integer.
    /// </summary>
    public bool WriteUInt32(int offset, uint value)
    {
        return Pointer.WriteUInt32(offset, value);
    }
    /// <summary>
    /// Read an 8-byte signed integer.
    /// </summary>
    public long ReadInt64(int offset)
    {
        return Pointer.ReadInt64(offset);
    }
    /// <summary>
    /// Write an 8-byte signed integer.
    /// </summary>
    public bool WriteInt64(int offset, long value)
    {
        return Pointer.WriteInt64(offset, value);
    }
    /// <summary>
    /// Read an 8-byte unsigned integer.
    /// </summary>
    public ulong ReadUInt64(int offset)
    {
        return Pointer.ReadUInt64(offset);
    }
    /// <summary>
    /// Write an 8-byte unsigned integer.
    /// </summary>
    public bool WriteUInt64(int offset, ulong value)
    {
        return Pointer.WriteUInt64(offset, value);
    }
    /// <summary>
    /// Read a 4-byte floating point number.
    /// </summary>
    public float ReadSingle(int offset)
    {
        return Pointer.ReadSingle(offset);
    }
    /// <summary>
    /// Write a 4-byte floating point number.
    /// </summary>
    public bool WriteSingle(int offset, float value)
    {
        return Pointer.WriteSingle(offset, value);
    }
    /// <summary>
    /// Read an 8-byte floating point number.
    /// </summary>
    public double ReadDouble(int offset)
    {
        return Pointer.ReadDouble(offset);
    }
    /// <summary>
    /// Write an 8-byte floating point number.
    /// </summary>
    public bool WriteDouble(int offset, double value)
    {
        return Pointer.WriteDouble(offset, value);
    }
    /// <summary>
    /// Reads and decodes a fixed amount of bytes as a string.
    ///
    /// If `trim` is set, terminates after the first null character.
    /// </summary>
    public string ReadString(int offset, Encoding encoding, uint byteCount, bool trim = true)
    {
        return Pointer.ReadString(offset, encoding, byteCount, trim);
    }
    /// <summary>
    /// Encodes and writes a string as a fixed amount of bytes.
    ///
    /// If the string is too long to fit, it is truncated before encoding.
    /// </summary>
    public bool WriteString(int offset, Encoding encoding, uint byteCount, string value)
    {
        return Pointer.WriteString(offset, encoding, byteCount, value);
    }
}