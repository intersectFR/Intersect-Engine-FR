namespace Intersect.Framework.Networking;

public abstract class Message
{
    private readonly Memory<byte> _buffer;

    internal Message(Memory<byte> buffer)
    {
    }

    public abstract bool CanRead { get; }

    public abstract bool CanSeek { get; }

    public abstract bool CanWrite { get; }

    public Connection Connection { get; set; }

    public abstract long Length { get; }

    public Network Network => Connection.Network;

    public virtual int Read(out bool value)
    {
        var bytesRead = Read(out byte byteValue);
        value = byteValue != default;
        return bytesRead;
    }

    public abstract int Read(out byte value);

    public virtual int Read(Span<byte> buffer) => Read(buffer, 0, buffer.Length);

    public virtual int Read(Span<byte> buffer, int offset) => Read(buffer, offset, buffer.Length - offset);

    public abstract int Read(Span<byte> buffer, int offset, int length);

    public virtual unsafe int Read(out char value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(char)];
        var bytesRead = Read(buffer, 0, sizeof(char));
        value = BitConverter.ToChar(buffer);
        return bytesRead;
    }

    public virtual unsafe int Read(out double value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        var bytesRead = Read(buffer, 0, sizeof(char));
        value = BitConverter.ToDouble(buffer);
        return bytesRead;
    }

    public virtual unsafe int Read(out float value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(float)];
        var bytesRead = Read(buffer, 0, sizeof(char));
        value = BitConverter.ToSingle(buffer);
        return bytesRead;
    }

    public virtual unsafe int Read(out int value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        var bytesRead = Read(buffer, 0, sizeof(char));
        value = BitConverter.ToInt32(buffer);
        return bytesRead;
    }

    public virtual unsafe int Read(out long value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        var bytesRead = Read(buffer, 0, sizeof(char));
        value = BitConverter.ToInt64(buffer);
        return bytesRead;
    }

    public virtual unsafe int Read(out sbyte value)
    {
        var bytesRead = Read(out byte byteValue);
        value = unchecked((sbyte)byteValue);
        return bytesRead;
    }

    public virtual unsafe int Read(out short value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        var bytesRead = Read(buffer, 0, sizeof(char));
        value = BitConverter.ToInt16(buffer);
        return bytesRead;
    }

    public virtual unsafe int Read(out uint value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        var bytesRead = Read(buffer, 0, sizeof(char));
        value = BitConverter.ToUInt32(buffer);
        return bytesRead;
    }

    public virtual unsafe int Read(out ulong value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];
        var bytesRead = Read(buffer, 0, sizeof(char));
        value = BitConverter.ToUInt64(buffer);
        return bytesRead;
    }

    public virtual unsafe int Read(out ushort value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];
        var bytesRead = Read(buffer, 0, sizeof(char));
        value = BitConverter.ToUInt16(buffer);
        return bytesRead;
    }

    public virtual unsafe int Write(bool value) => Write(*(byte*)&value);

    public abstract int Write(byte value);

    public virtual int Write(ReadOnlySpan<byte> buffer) => Write(buffer, 0, buffer.Length);

    public virtual int Write(ReadOnlySpan<byte> buffer, int offset) => Write(buffer, offset, buffer.Length - offset);

    public abstract int Write(ReadOnlySpan<byte> buffer, int offset, int count);

    public virtual int Write(char value) => Write(BitConverter.GetBytes(value));

    public virtual int Write(double value) => Write(BitConverter.GetBytes(value));

    public virtual int Write(float value) => Write(BitConverter.GetBytes(value));

    public virtual int Write(int value) => Write(BitConverter.GetBytes(value));

    public virtual int Write(long value) => Write(BitConverter.GetBytes(value));

    public virtual int Write(sbyte value) => Write(unchecked((byte)value));

    public virtual int Write(short value) => Write(BitConverter.GetBytes(value));

    public virtual int Write(uint value) => Write(BitConverter.GetBytes(value));

    public virtual int Write(ulong value) => Write(BitConverter.GetBytes(value));

    public virtual int Write(ushort value) => Write(BitConverter.GetBytes(value));
}
