namespace Intersect.Framework.Memory.Utility;

internal interface IPlatformUnmanagedConverter
{
    int Read<T>(ReadOnlySpan<byte> buffer, out T value) where T : unmanaged;

    int Write<T>(Span<byte> buffer, T value) where T : unmanaged;
}
