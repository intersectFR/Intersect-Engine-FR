using System.Buffers;
using System.Runtime.CompilerServices;

using Intersect.Framework.Exceptions;

namespace Intersect.Framework.Memory.Buffers;

public class MemoryBuffer : Buffer
{
    internal static int MaximumCapacity = int.MaxValue;

    private int _length;
    private IMemoryOwner<byte> _memoryOwner;
    private int _position;

    public MemoryBuffer() : this(0)
    {
    }

    public MemoryBuffer(int initialCapacity) : this(
              canRead: true,
              canSeek: true,
              canWrite: true,
              initialCapacity: initialCapacity
              )
    {
    }

    public MemoryBuffer(byte[] initialData)
        : this((ReadOnlyMemory<byte>)initialData)
    {
    }

    public MemoryBuffer(ReadOnlyMemory<byte> initialData) : this(
              canRead: true,
              canSeek: true,
              canWrite: true,
              initialCapacity: initialData.Length)
    {
        initialData.CopyTo(Buffer);
        _length = initialData.Length;
    }

    public MemoryBuffer(IMemoryOwner<byte> initialDataOwner)
        : this(initialDataOwner.Memory)
    {
    }

    public MemoryBuffer(
        bool canRead,
        bool canSeek,
        bool canWrite,
        int initialCapacity)
    {
        CanRead = canRead;
        CanSeek = canSeek;
        CanWrite = canWrite;
        EnsureCapacity(initialCapacity);
    }

    /// <summary>
    /// The current <see cref="Memory{T}"/> backing this <see cref="Buffers.Buffer"/>.
    /// </summary>
    protected internal Memory<byte> Buffer => _memoryOwner?.Memory ?? Memory<byte>.Empty;

    /// <inheritdoc/>
    public override bool CanRead { get; }

    /// <inheritdoc/>
    public override bool CanSeek { get; }

    /// <inheritdoc/>
    public override bool CanWrite { get; }

    /// <inheritdoc/>
    public override long Capacity
    {
        get => _memoryOwner?.Memory.Length ?? 0;
        set
        {
            if (value < 0 || int.MaxValue < value)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            EnsureCapacity((int)value);
        }
    }

    /// <inheritdoc/>
    public override long Length => _length;

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">the new position is less than 0 or more than </exception>
    public override long Position
    {
        get => _position;
        set
        {
            if (value < 0 || _length <= value)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"The provided value is outside of the range [0, {_length}).");
            }

            _position = (int)value;
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (!Disposed)
        {
            _memoryOwner?.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Ensures that the internal buffer has sufficient total space for write operations.
    /// </summary>
    /// <param name="capacity">the new capacity of the internal buffer in bytes</param>
    /// <exception cref="InvalidOperationException">failed to copy data from the old to new internal buffer</exception>
    protected void EnsureCapacity(int capacity)
    {
        if (Buffer.Length >= capacity)
        {
            return;
        }

        var newMemoryOwner = MemoryPool<byte>.Shared.Rent(capacity);
        if (!Buffer.IsEmpty && !Buffer.TryCopyTo(newMemoryOwner.Memory))
        {
            throw new InvalidOperationException("Failed to copy current buffer contents to new internal buffer.");
        }

        var oldMemoryOwner = _memoryOwner;
        _memoryOwner = newMemoryOwner;
        oldMemoryOwner?.Dispose();
    }

    /// <inheritdoc/>
    protected override ReadOnlySpan<byte> GetReadSpan(long length)
    {
        // Check if the internal buffer has enough remaining data
        var remaining = _length - _position;
        if (remaining < length)
        {
            throw new ArgumentOutOfRangeException(nameof(length), $"Tried to read {length} bytes, only {remaining} left in buffer.");
        }

        var span = Buffer.Span.Slice(_position, (int)length);
        _position += span.Length;
        return span;
    }

    /// <inheritdoc/>
    protected override Span<byte> GetWriteSpan(long length)
    {
        // Check if the internal buffer can hold the additional data
        var remainingInternal = MaximumCapacity - _position;
        if (remainingInternal < length)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, $"The buffer cannot store {length} more bytes.");
        }

        EnsureCapacity(_position + (int)length);

        var span = Buffer.Span.Slice(_position, (int)length);
        _position += span.Length;
        return span;
    }

    /// <inheritdoc/>
    /// <exception cref="IndexOutOfRangeException">the buffer does not have 1 more byte to read</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int Read(out byte value)
    {
        // Check for buffer overflows
        var remaining = _length - _position;
        if (remaining < 1)
        {
            throw new IndexOutOfRangeException($"No remaining memory to read from.");
        }

        value = Buffer.Span[_position++];
        return sizeof(byte);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">the buffer does not have <paramref name="length"/> more bytes to read</exception>
    /// <exception cref="InvalidOperationException">failed to copy data from the internal buffer to <paramref name="buffer"/></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int Read(Span<byte> buffer, int offset, int length)
    {
        // Check if the input buffer can store the read data
        var availableInBuffer = buffer.Length - offset;
        if (availableInBuffer < length)
        {
            throw new ArgumentOutOfRangeException(nameof(length), new IndexOutOfRangeParameters(buffer.Length, offset, length), $"The offset and length exceed the length of the input buffer.");
        }

        // Check if the internal buffer has enough remaining data
        var remaining = _length - _position;
        if (remaining < length)
        {
            throw new ArgumentOutOfRangeException(nameof(length), $"Tried to read {length} bytes, only {remaining} left in buffer.");
        }

        if (Buffer.Span.Slice(_position, length).TryCopyTo(buffer[offset..]))
        {
            _position += length;
            return length;
        }

        throw new InvalidOperationException("Failed to copy data to destination buffer.");
    }

    /// <inheritdoc/>
    /// <exception cref="IndexOutOfRangeException">the buffer cannot store or expand to fit 1 more byte</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int Write(byte value)
    {
        var remaining = MaximumCapacity - _position;
        if (remaining < 1)
        {
            throw new IndexOutOfRangeException($"No remaining memory to write to.");
        }

        EnsureCapacity(_position + sizeof(byte));
        Buffer.Span[_position++] = value;
        _length = Math.Max(_position, _length);
        return sizeof(byte);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">the buffer cannot store or expand to fit <paramref name="length"/> more bytes</exception>
    /// <exception cref="InvalidOperationException">failed to copy data from <paramref name="buffer"/> to the internal buffer</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int Write(ReadOnlySpan<byte> buffer, int offset, int length)
    {
        // Check if the input buffer contains the range specified by offset and length
        var availableInBuffer = buffer.Length - offset;
        if (availableInBuffer < length)
        {
            throw new ArgumentOutOfRangeException(nameof(length), new IndexOutOfRangeParameters(buffer.Length, offset, length), $"The offset and length exceed the length of the input buffer.");
        }

        // Check if the internal buffer can hold the additional data
        var remainingInternal = MaximumCapacity - _position;
        if (remainingInternal < length)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, $"The buffer cannot store {length} more bytes.");
        }

        EnsureCapacity(_position + length);
        if (buffer.Slice(offset, length).TryCopyTo(Buffer.Span[_position..]))
        {
            _position += length;
            _length = Math.Max(_position, _length);
            return length;
        }

        throw new InvalidOperationException("Failed to copy data to internal buffer.");
    }
}
