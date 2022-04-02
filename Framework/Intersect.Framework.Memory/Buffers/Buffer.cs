using System.Runtime.CompilerServices;
using System.Text;

using Intersect.Framework.Memory.Utility;

namespace Intersect.Framework.Memory.Buffers;

public abstract partial class Buffer : IDisposable
{
    public abstract bool CanRead { get; }

    public abstract bool CanSeek { get; }

    public abstract bool CanWrite { get; }

    public abstract long Capacity { get; set; }

    protected bool Disposed { get; private set; }

    public abstract long Length { get; }

    public abstract long Position { get; set; }

    public abstract ReadOnlySpan<byte> GetReadSpan(long length);

    public abstract Span<byte> GetWriteSpan(long length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int Read<T>(out T value) where T : unmanaged =>
        UnmanagedConverter.Read(GetWriteSpan(sizeof(T)), out value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Read(Span<byte> buffer) =>
        Read(out int length) + Read(buffer, 0, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Read(Span<byte> buffer, int offset) =>
        Read(out int length) + Read(buffer, offset, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Read(Span<byte> buffer, int offset, int length)
    {
        var bufferSection = GetReadSpan(length);
        bufferSection.CopyTo(buffer.Slice(offset, length));
        return bufferSection.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Read(out string value) => Read(Encoding.UTF8, out value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Read(Encoding encoding, out string value)
    {
        var lengthBytes = ReadVariable(out int length);
        var internalBuffer = GetReadSpan(length);
        value = encoding.GetString(internalBuffer);
        return lengthBytes + length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int Write<T>(T value) where T : unmanaged =>
        UnmanagedConverter.Write(GetWriteSpan(sizeof(T)), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Write(ReadOnlySpan<byte> buffer) =>
        WriteVariable(buffer.Length) + Write(buffer, 0, buffer.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Write(ReadOnlySpan<byte> buffer, int offset) =>
        WriteVariable(buffer.Length - offset) + Write(buffer, offset, buffer.Length - offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Write(ReadOnlySpan<byte> buffer, int offset, int length)
    {
        var bufferSection = GetWriteSpan(length);
        buffer.Slice(offset, length).CopyTo(bufferSection);
        return bufferSection.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Write(string value) => Write(value, Encoding.UTF8);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Write(string value, Encoding encoding)
    {
        var length = encoding.GetByteCount(value);
        var lengthBytes = WriteVariable(length);
        var destination = GetWriteSpan(length);
        _ = encoding.GetBytes(value.AsSpan(), destination);
        return lengthBytes + length;
    }

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
