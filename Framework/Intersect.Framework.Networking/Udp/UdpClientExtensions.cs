using System.Net.Sockets;


namespace Intersect.Framework.Networking.Udp;

internal static class UdpClientExtensions
{
    public static async Task<UdpReceiveResult> ReceiveAsync(this UdpClient udpClient, CancellationToken cancellationToken)
    {
        try
        {
            var udpReceiveResult = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
            return udpReceiveResult;
        }
        catch (OperationCanceledException)
        {
            udpClient.Client.Close();
            throw;
        }
    }
}
