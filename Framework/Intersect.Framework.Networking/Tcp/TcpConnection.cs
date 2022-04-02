using System.Net;


namespace Intersect.Framework.Networking.Tcp;

internal class TcpConnection : Connection
{
    public TcpConnection(Network network, IPEndPoint remoteEndPoint) : base(network, remoteEndPoint)
    {
    }

    public override bool TrySend(Message message)
    {
        throw new NotImplementedException();
    }
}
