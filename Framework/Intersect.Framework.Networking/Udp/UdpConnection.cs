using System.Net;
using System.Net.Sockets;


namespace Intersect.Framework.Networking;

internal class UdpConnection : Connection
{
    private readonly UdpClient _udpClient;

    internal UdpConnection(Network network, IPEndPoint remoteEndPoint)
        : base(network, remoteEndPoint)
    {
        _udpClient = new UdpClient();
    }

    public override bool TrySendData(ReadOnlySpan<byte> data)
    {
        throw new NotImplementedException();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _udpClient.Dispose();
        }
    }
}
