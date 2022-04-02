namespace Intersect.Framework.Networking;

internal sealed class InternalFragmentCollection : IDisposable
{
    private readonly uint _id;
    private readonly ushort _packetLength;

    private byte _count;
    private byte[][] _fragments;
    private bool _lastFragmentSeen;

    public InternalFragmentCollection(uint id, ushort packetLength, int fragments = default)
    {
        _id = id;
        _packetLength = packetLength;

        _count = 0;
        _fragments = new byte[fragments == default ? Math.Max(2, packetLength >> 10) : fragments][];
    }

    public int Count => CheckDisposed(ref _count);

    public bool IsComplete => _lastFragmentSeen && _count == CheckDisposed(ref _fragments).Length;

    private void EnsureCapacity(int fragmentId, int fragmentSize)
    {
        var containerSize = CheckDisposed(ref _fragments).Length;

        if (_lastFragmentSeen || fragmentId < containerSize)
        {
            return;
        }

        // By default assume it's the last fragment
        var estimatedFragmentCount = containerSize + 1;

        // But if it is not the last packet, estimate the packet count based on the packet size
        if (fragmentId > containerSize)
        {
            estimatedFragmentCount = Math.Max(fragmentId + 1, (int)Math.Ceiling(_packetLength / (double)fragmentSize));
        }

        var newFragmentContainer = new byte[estimatedFragmentCount][];
        Buffer.BlockCopy(_fragments, 0, newFragmentContainer, 0, _fragments.Length);
        _fragments = newFragmentContainer;
    }

    public bool TryAdd(byte fragmentId, byte[] fragmentData)
    {
        EnsureCapacity(fragmentId & 0x7f, fragmentData.Length);

        // Change this after ensuring capacity to make sure that EnsureCapacity
        // ensures that there is indeed capacity for the last fragment
        _lastFragmentSeen |= (fragmentId & 0x80) != 0;

        if (_fragments[fragmentId] != default)
        {
            return false;
        }

        _fragments[fragmentId] = fragmentData;
        return true;
    }

    public bool TryAssemble(in Span<byte> buffer)
    {
        if (!IsComplete)
        {
            // If we do not have all of the fragments, abort
            return false;
        }

        for (var fragmentId = 0; fragmentId < _fragments.Length; ++fragmentId)
        {
            if (!_fragments[fragmentId].AsSpan().TryCopyTo(buffer))
            {
                // If a fragment fails to copy, abort
                return false;
            }
        }

        return true;
    }

    private void CheckDisposed()
    {
        if (_fragments == default)
        {
            throw new ObjectDisposedException($"{nameof(InternalFragmentCollection)}({_id:x})");
        }
    }

    private ref T CheckDisposed<T>(ref T value)
    {
        CheckDisposed();
        return ref value;
    }

    public void Dispose() => CheckDisposed(ref _fragments) = default!;
}
