using System.Net;


namespace Intersect.Framework.Networking.Tcp;

internal class TcpConnection : Connection
{
    public TcpConnection(Network network, IPEndPoint remoteEndPoint) : base(network, remoteEndPoint)
    {
    }

    public override bool TrySendData(ReadOnlySpan<byte> data)
    {
        throw new NotImplementedException();
    }
}
