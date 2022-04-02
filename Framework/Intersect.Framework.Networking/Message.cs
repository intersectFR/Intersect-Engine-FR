using Intersect.Framework.Memory.Buffers;

namespace Intersect.Framework.Networking;

public sealed class Message : Memory.Buffers.Buffer
{
    internal readonly Memory.Buffers.Buffer _backingBuffer;
    private readonly bool _writable;

    internal Message(Message? message = default, bool writable = false)
    {
        _backingBuffer = message?._backingBuffer ?? new MemoryBuffer();
        _writable = writable;
    }

    /// <inheritdoc/>
    public override bool CanRead => _backingBuffer.CanRead;

    /// <inheritdoc/>
    public override bool CanSeek => _backingBuffer.CanSeek;

    /// <inheritdoc/>
    public override bool CanWrite => _writable && _backingBuffer.CanWrite;

    /// <inheritdoc/>
    public override long Capacity
    {
        get => _backingBuffer.Capacity;
        set => _backingBuffer.Capacity = value;
    }

    /// <inheritdoc/>
    public override long Length => _backingBuffer.Length;

    public Network Network { get; set; }

    /// <inheritdoc/>
    public override long Position {
        get => _backingBuffer.Position;
        set => _backingBuffer.Position = value;
    }

    /// <inheritdoc/>
    public Message AsReadable() => new(message: this, writable: false);

    /// <inheritdoc/>
    public Message AsWritable() => new(message: this, writable: true);

    /// <inheritdoc/>
    public override ReadOnlySpan<byte> GetReadSpan(long length) => _backingBuffer.GetReadSpan(length);

    /// <inheritdoc/>
    public override Span<byte> GetWriteSpan(long length) => _backingBuffer.GetWriteSpan(length);
}
