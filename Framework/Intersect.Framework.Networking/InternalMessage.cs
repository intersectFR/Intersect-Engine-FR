namespace Intersect.Framework.Networking;

internal class InternalMessage
{
    private readonly InternalMessageHeader _header;
    private readonly InternalFragmentCollection _fragments;

    private InternalMessage(ref InternalMessageHeader header, int estimatedFragmentCount)
    {
        _header = header;
        _fragments = new InternalFragmentCollection(header.Id, header.Length, estimatedFragmentCount);
    }

    internal ref readonly InternalMessageHeader Header => ref _header;

    internal bool TryAdd(byte fragmentId, byte[] fragmentData) =>
        _fragments.TryAdd(fragmentId, fragmentData);

    internal static InternalMessage Create(ref InternalMessageHeader header, int mss)
    {
        var fragments = (int)Math.Ceiling(header.Length / (double)mss);
        if (fragments > 0x7f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(header),
                $"MSS is too low or the message header length is fradulent, received message with {fragments} fragments."
            );
        }

        return new InternalMessage(ref header, fragments);
    }

    internal static InternalMessage Create(ref InternalMessageHeader header, Message message, int mss)
    {
        var fragments = (int)Math.Ceiling(message.Length / (double)mss);
        if (fragments > 0x7f)
        {
            var maximumSupportedSize = mss << 7;
            throw new ArgumentOutOfRangeException(
                nameof(message),
                $"Maximum message length is {maximumSupportedSize} bytes, but received message of {message.Length} bytes."
            );
        }

        var internalMessage = new InternalMessage(ref header, fragments);
        for (var fragmentId = 0; fragmentId < fragments; ++fragmentId)
        {
            var fragmentData = new byte[mss];
            message.Position = 0;
            if (!message.GetReadSpan(mss).TryCopyTo(fragmentData))
            {
                throw new InvalidOperationException($"Failed to copy message fragment {fragmentId:x}.");
            }

            if (!internalMessage._fragments.TryAdd((byte)fragmentId, fragmentData))
            {
                throw new InvalidOperationException($"Failed to add fragment {fragmentId:x} to collection.");
            }
        }

        return internalMessage;
    }
}
