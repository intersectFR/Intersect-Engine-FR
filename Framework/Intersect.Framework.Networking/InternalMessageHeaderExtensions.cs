namespace Intersect.Framework.Networking;

internal static class InternalMessageHeaderExtensions
{
    public static TransmissionMode GetTransmissionMode(this ref InternalMessageHeader internalMessageHeader) =>
        (TransmissionMode)((internalMessageHeader.PackedType & 0xe0) >> 5);

    public static InternalMessageType GetMessageType(this ref InternalMessageHeader internalMessageHeader) =>
        (InternalMessageType)(internalMessageHeader.PackedType & 0x3f);

    public static byte GetFragmentId(this ref InternalMessageHeader internalMessageHeader) =>
        ((byte)(internalMessageHeader.Fragment & 0x7f));

    public static bool IsLastFragment(this ref InternalMessageHeader internalMessageHeader) =>
        (internalMessageHeader.Fragment & 0x80) == 0;
}
