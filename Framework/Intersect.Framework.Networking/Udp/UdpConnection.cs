using System.Net;
using System.Net.Sockets;

namespace Intersect.Framework.Networking;

internal class UdpConnection : Connection
{
    private readonly UdpClient _udpClient;
    private IPEndPoint? _targetEndPoint;

    internal UdpConnection(Network network, IPEndPoint remoteEndPoint, UdpClient? udpClient = default)
        : base(network, remoteEndPoint)
    {
        _targetEndPoint = RemoteEndPoint;
        _udpClient = udpClient ?? new UdpClient();
    }

    internal void Connect()
    {
        _targetEndPoint = default;
        _udpClient.Connect(RemoteEndPoint);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _udpClient.Dispose();
        }
    }

    public override bool TrySend(Message message)
    {
        var data = message._backingBuffer
        Task<int> sendTask = _udpClient.SendAsync(data.ToArray(), length, _targetEndPoint);
        var sentBytes = sendTask.GetAwaiter().GetResult();
        return sentBytes == length;
    }
}
