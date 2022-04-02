namespace Intersect.Framework.Networking;

internal static class NetworkConstants
{
    public const int MaximumIpHeaderSize = 60;

    public const int MaximumTcpHeaderSize = 60;

    public const int MaximumUdpHeaderSize = 8;

    private const int MtuIeee802 = 508;
    private const int MtuX25 = 576;
    private const int MtuSimple = 1024;
    private const int MtuGoogleCloud = 1460;
    private const int MtuIee802_3 = 1492;
    private const int MtuEthernet = 1500;

    private static readonly int[] MtuSizes =
    {
        MtuIeee802,
        MtuX25,
        MtuSimple,
        MtuGoogleCloud,
        MtuIee802_3,
        MtuEthernet,
    };

    public static readonly int[] MssTcp = MtuSizes.Select(mtu => mtu - (MaximumIpHeaderSize + MaximumTcpHeaderSize)).ToArray();

    public static readonly int[] MssUdp = MtuSizes.Select(mtu => mtu - (MaximumIpHeaderSize + MaximumUdpHeaderSize)).ToArray();
}
