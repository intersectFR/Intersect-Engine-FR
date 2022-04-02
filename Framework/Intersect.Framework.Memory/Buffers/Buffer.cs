using System.Buffers.Binary;
using System.Text;

namespace Intersect.Framework.Memory.Buffers;

public abstract class Buffer : IDisposable
{
    public abstract bool CanRead { get; }

    public abstract bool CanSeek { get; }

    public abstract bool CanWrite { get; }

    public abstract long Capacity { get; set; }

    protected bool Disposed { get; private set; }

    public abstract long Length { get; }

    public abstract long Position { get; set; }

    protected abstract ReadOnlySpan<byte> GetReadSpan(long length);

    protected abstract Span<byte> GetWriteSpan(long length);

    public virtual int Read(out bool value)
    {
        var bytesRead = Read(out byte byteValue);
        value = byteValue != default;
        return bytesRead;
    }

    public abstract int Read(out byte value);

    public virtual int Read(Span<byte> buffer) =>
        Read(out int length) + Read(buffer, 0, length);

    public virtual int Read(Span<byte> buffer, int offset) =>
        Read(out int length) + Read(buffer, offset, length);

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

    public virtual int Read(out short value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        var bytesRead = Read(buffer, 0, sizeof(char));
        value = BitConverter.ToInt16(buffer);
        return bytesRead;
    }

    public virtual int Read(out string value) => Read(Encoding.UTF8, out value);

    public virtual int Read(Encoding encoding, out string value)
    {
        var lengthBytes = ReadVariable(out int length);
        var internalBuffer = GetReadSpan(length);
        value = encoding.GetString(internalBuffer);
        return lengthBytes + length;
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

    public virtual int ReadVariable(out char value)
    {
        var bytesRead = ReadVariable(out ushort intermediate);
        value = (char)intermediate;
        return bytesRead;
    }

    public virtual int ReadVariable(out int value)
    {
        var bytesRead = ReadVariable(out uint zigzag);
        value = (int)(zigzag >> 1) ^ -(int)(zigzag & 1);
        return bytesRead;
    }

    public virtual int ReadVariable(out long value)
    {
        var bytesRead = ReadVariable(out ulong zigzag);
        value = (long)(zigzag >> 1) ^ -(long)(zigzag & 1);
        return bytesRead;
    }

    public virtual int ReadVariable(out short value)
    {
        var bytesRead = ReadVariable(out ushort zigzag);
        value = (short)((short)(zigzag >> 1) ^ -(short)(zigzag & 1));
        return bytesRead;
    }

    public virtual int ReadVariable(out uint value)
    {
        var bytesRead = ReadVariable(out ulong expandedValue);
        value = (uint)expandedValue;
        return bytesRead;
    }

    public virtual int ReadVariable(out ulong value)
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

    public virtual int ReadVariable(out ushort value)
    {
        var bytesRead = ReadVariable(out ulong expandedValue);
        value = (ushort)expandedValue;
        return bytesRead;
    }

    public virtual unsafe int Write(bool value) => Write(*(byte*)&value);

    public abstract int Write(byte value);

    public virtual int Write(ReadOnlySpan<byte> buffer) =>
        WriteVariable(buffer.Length) + Write(buffer, 0, buffer.Length);

    public virtual int Write(ReadOnlySpan<byte> buffer, int offset) =>
        WriteVariable(buffer.Length - offset) + Write(buffer, offset, buffer.Length - offset);

    public abstract int Write(ReadOnlySpan<byte> buffer, int offset, int length);

    public virtual int Write(char value) => Write(BitConverter.GetBytes(value), 0, sizeof(char));

    public virtual int Write(double value) => Write(BitConverter.GetBytes(value), 0, sizeof(double));

    public virtual int Write(float value) => Write(BitConverter.GetBytes(value), 0, sizeof(float));

    public virtual int Write(int value) => Write(BitConverter.GetBytes(value), 0, sizeof(int));

    public virtual int Write(long value) => Write(BitConverter.GetBytes(value), 0, sizeof(long));

    public virtual int Write(sbyte value) => Write(unchecked((byte)value));

    public virtual int Write(short value) => Write(BitConverter.GetBytes(value), 0, sizeof(short));

    public virtual int Write(string value) => Write(value, Encoding.UTF8);

    public virtual int Write(string value, Encoding encoding)
    {
        var length = encoding.GetByteCount(value);
        var lengthBytes = WriteVariable(length);
        var destination = GetWriteSpan(length);
        _ = encoding.GetBytes(value.AsSpan(), destination);
        return lengthBytes + length;
    }

    public virtual int Write(uint value) => Write(BitConverter.GetBytes(value), 0, sizeof(uint));

    public virtual int Write(ulong value) => Write(BitConverter.GetBytes(value), 0, sizeof(ulong));

    public virtual int Write(ushort value) => Write(BitConverter.GetBytes(value), 0, sizeof(ushort));

    public virtual int WriteVariable(char value) => WriteVariable((ushort)value);

    public virtual int WriteVariable(int value)
    {
        ulong zigzag = (ulong)(value << 1) ^ (ulong)(value >> 31);
        return WriteVariable(zigzag);
    }

    public virtual int WriteVariable(long value)
    {
        ulong zigzag = (ulong)(value << 1) ^ (ulong)(value >> 63);
        return WriteVariable(zigzag);
    }

    public virtual int WriteVariable(short value)
    {
        ulong zigzag = (ulong)(value << 1) ^ (ulong)(value >> 15);
        return WriteVariable(zigzag);
    }

    public virtual int WriteVariable(uint value) => WriteVariable((ulong)value);

    public virtual int WriteVariable(ulong value)
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

    public virtual int WriteVariable(ushort value) => WriteVariable((ulong)value);

    protected virtual void Dispose(bool disposing)
    {
        if (!Disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            Disposed = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Buffer()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
