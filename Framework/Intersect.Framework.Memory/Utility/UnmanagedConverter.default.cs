using System.Runtime.CompilerServices;

namespace Intersect.Framework.Memory.Utility;

internal unsafe class DefaultUnmanagedConverter : IPlatformUnmanagedConverter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Read<T>(ReadOnlySpan<byte> buffer, out T value) where T : unmanaged
    {
        var size = sizeof(T);
        fixed (byte* bufferPtr = buffer)
        {
            value = *(T*)bufferPtr;
        }
        return size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Write<T>(Span<byte> buffer, T value) where T : unmanaged
    {
        var size = sizeof(T);
        fixed (byte* bufferPtr = buffer)
        {
            *(T*)bufferPtr = value;
        }
        return size;
    }
}
