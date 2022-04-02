using System.Net.Sockets;

namespace Intersect.Framework.Networking.Udp;

internal class UdpReceiver
{
    private readonly CancellationToken _cancellationToken;
    private readonly UdpClient _udpClient;
    private readonly UdpConnectionManager _udpConnectionManager;

    internal UdpReceiver(
        UdpConnectionManager udpConnectionManager,
        UdpClient udpClient,
        CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _udpClient = udpClient;
        _udpConnectionManager = udpConnectionManager;
    }

    internal void OnReceive(Task<UdpReceiveResult> udpReceiveResultTask)
    {
        _cancellationToken.ThrowIfCancellationRequested();
        _udpConnectionManager.OnReceive(udpReceiveResultTask.Result);
        _ = Spawn();
    }

    internal Task Spawn()
    {
        _cancellationToken.ThrowIfCancellationRequested();
        return _udpClient.ReceiveAsync(_cancellationToken).ContinueWith(OnReceive);
    }
}
