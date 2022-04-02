using System.Runtime.CompilerServices;

namespace Intersect.Framework.Memory.Utility;

internal unsafe class AndroidUnmanagedConverter : IPlatformUnmanagedConverter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Read<T>(ReadOnlySpan<byte> buffer, out T value) where T : unmanaged
    {
        var size = sizeof(T);
        fixed (byte* bufferPtr = buffer)
        {
            T* valueBuffer = stackalloc T[1];
            Buffer.MemoryCopy(bufferPtr, valueBuffer, size, size);
            value = *valueBuffer;
        }
        return size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Write<T>(Span<byte> buffer, T value) where T : unmanaged
    {
        var size = sizeof(T);
        fixed (byte* bufferPtr = buffer)
        {
            T* valueBuffer = stackalloc T[1] { value };
            Buffer.MemoryCopy(valueBuffer, bufferPtr, size, size);
        }
        return size;
    }
}
