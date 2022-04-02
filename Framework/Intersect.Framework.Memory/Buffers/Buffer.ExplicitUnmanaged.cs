namespace Intersect.Framework.Memory.Buffers;

public partial class Buffer
{
#if EXPLICIT_UNMANAGED

    public int Read(out bool value)
    {
        var bytesRead = Read(out byte byteValue);
        value = byteValue != default;
        return bytesRead;
    }

    public int Read(out byte value)
    {
        var buffer = GetReadSpan(sizeof(byte));
        value = buffer[0];
        return buffer.Length;
    }

    public unsafe int Read(out char value)
    {
        var bufferSection = GetReadSpan(sizeof(char));
        value = BitConverter.ToChar(bufferSection);
        return bufferSection.Length;
    }

    public unsafe int Read(out double value)
    {
        var bufferSection = GetReadSpan(sizeof(double));
        value = BitConverter.ToDouble(bufferSection);
        return bufferSection.Length;
    }

    public unsafe int Read(out float value)
    {
        var bufferSection = GetReadSpan(sizeof(float));
        value = BitConverter.ToSingle(bufferSection);
        return bufferSection.Length;
    }

    public unsafe int Read(out int value)
    {
        var bufferSection = GetReadSpan(sizeof(int));
        value = BitConverter.ToInt32(bufferSection);
        return bufferSection.Length;
    }

    public unsafe int Read(out long value)
    {
        var bufferSection = GetReadSpan(sizeof(long));
        value = BitConverter.ToInt64(bufferSection);
        return bufferSection.Length;
    }

    public unsafe int Read(out sbyte value)
    {
        var bufferSection = GetReadSpan(sizeof(sbyte));
        value = unchecked((sbyte)bufferSection[0]);
        return bufferSection.Length;
    }

    public int Read(out short value)
    {
        var bufferSection = GetReadSpan(sizeof(short));
        value = BitConverter.ToInt16(bufferSection);
        return bufferSection.Length;
    }

    public unsafe int Read(out uint value)
    {
        var bufferSection = GetReadSpan(sizeof(uint));
        value = BitConverter.ToUInt32(bufferSection);
        return bufferSection.Length;
    }

    public unsafe int Read(out ulong value)
    {
        var bufferSection = GetReadSpan(sizeof(ulong));
        value = BitConverter.ToUInt64(bufferSection);
        return bufferSection.Length;
    }

    public unsafe int Read(out ushort value)
    {
        var bufferSection = GetReadSpan(sizeof(ushort));
        value = BitConverter.ToUInt16(bufferSection);
        return bufferSection.Length;
    }

    public unsafe int Write(bool value) => Write(*(byte*)&value);

    public int Write(byte value)
    {
        var bufferSection = GetWriteSpan(sizeof(byte));
        bufferSection[0] = value;
        return bufferSection.Length;
    }

    public int Write(char value) => Write(BitConverter.GetBytes(value), 0, sizeof(char));

    public int Write(double value) => Write(BitConverter.GetBytes(value), 0, sizeof(double));

    public int Write(float value) => Write(BitConverter.GetBytes(value), 0, sizeof(float));

    public unsafe int Write(int value) => Write(BitConverter.GetBytes(value), 0, sizeof(int));

    public int Write(long value) => Write(BitConverter.GetBytes(value), 0, sizeof(long));

    public int Write(sbyte value) => Write(unchecked((byte)value));

    public int Write(short value) => Write(BitConverter.GetBytes(value), 0, sizeof(short));

    public int Write(uint value) => Write(BitConverter.GetBytes(value), 0, sizeof(uint));

    public int Write(ulong value) => Write(BitConverter.GetBytes(value), 0, sizeof(ulong));

    public int Write(ushort value) => Write(BitConverter.GetBytes(value), 0, sizeof(ushort));

#endif
}
