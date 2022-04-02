namespace Intersect.Framework.Memory.Buffers;

public partial class Buffer
{
    #region Read Variable

    public int ReadVariable(out char value)
    {
        var bytesRead = ReadVariable(out ushort intermediate);
        value = (char)intermediate;
        return bytesRead;
    }

    public int ReadVariable(out int value)
    {
        var bytesRead = ReadVariable(out uint zigzag);
        value = (int)(zigzag >> 1) ^ -(int)(zigzag & 1);
        return bytesRead;
    }

    public int ReadVariable(out long value)
    {
        var bytesRead = ReadVariable(out ulong zigzag);
        value = (long)(zigzag >> 1) ^ -(long)(zigzag & 1);
        return bytesRead;
    }

    public int ReadVariable(out short value)
    {
        var bytesRead = ReadVariable(out ushort zigzag);
        value = (short)((short)(zigzag >> 1) ^ -(short)(zigzag & 1));
        return bytesRead;
    }

    public int ReadVariable(out uint value)
    {
        var bytesRead = ReadVariable(out ulong expandedValue);
        value = (uint)expandedValue;
        return bytesRead;
    }

    public int ReadVariable(out ulong value)
    {
        var bytesRead = 0;
        var offset = 0;
        value = 0;
        while (bytesRead <= sizeof(ulong))
        {
            bytesRead += Read(out byte @byte);
            value |= (ulong)(@byte & 0x7f) << offset;
            offset += 7;
            if ((@byte & 0x80) == 0)
            {
                break;
            }
        }
        return bytesRead;
    }

    public int ReadVariable(out ushort value)
    {
        var bytesRead = ReadVariable(out ulong expandedValue);
        value = (ushort)expandedValue;
        return bytesRead;
    }

    #endregion

    #region Write Variable

    public int WriteVariable(char value) => WriteVariable((ushort)value);

    public int WriteVariable(int value)
    {
        ulong zigzag = (ulong)(value << 1) ^ (ulong)(value >> 31);
        return WriteVariable(zigzag);
    }

    public int WriteVariable(long value)
    {
        ulong zigzag = (ulong)(value << 1) ^ (ulong)(value >> 63);
        return WriteVariable(zigzag);
    }

    public int WriteVariable(short value)
    {
        ulong zigzag = (ulong)(value << 1) ^ (ulong)(value >> 15);
        return WriteVariable(zigzag);
    }

    public int WriteVariable(uint value) => WriteVariable((ulong)value);

    public int WriteVariable(ulong value)
    {
        var bytesWritten = 0;
        var remaining = value;
        while (remaining >= 0x80)
        {
            bytesWritten += Write((byte)(remaining | 0x80));
            remaining >>= 7;
        }
        bytesWritten += Write((byte)remaining);
        return bytesWritten;
    }

    public int WriteVariable(ushort value) => WriteVariable((ulong)value);

    #endregion
}
