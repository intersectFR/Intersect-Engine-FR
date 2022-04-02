using System.Runtime.InteropServices;

namespace Intersect.Framework.Networking;

[StructLayout(LayoutKind.Explicit)]
internal struct InternalMessageHeader
{
    [FieldOffset(0)]
    public uint Id;

    [FieldOffset(4)]
    public byte PackedType;

    [FieldOffset(5)]
    public byte Channel;

    [FieldOffset(6)]
    public ushort Length;

    [FieldOffset(8)]
    public byte Fragment;
}
