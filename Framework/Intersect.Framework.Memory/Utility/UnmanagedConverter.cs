using System.Runtime.CompilerServices;

namespace Intersect.Framework.Memory.Utility;

public static unsafe class UnmanagedConverter
{
    private static readonly IPlatformUnmanagedConverter _platformUnmanagedConverter;

    static UnmanagedConverter()
    {
        _platformUnmanagedConverter = new DefaultUnmanagedConverter();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Read<T>(ReadOnlySpan<byte> buffer, out T value) where T : unmanaged =>
        _platformUnmanagedConverter.Read(buffer, out value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Write<T>(Span<byte> buffer, T value) where T : unmanaged =>
        _platformUnmanagedConverter.Write(buffer, value);
}
