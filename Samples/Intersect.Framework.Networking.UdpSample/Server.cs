using System.Net;
using System.Net.Sockets;
using System.Text;

using Intersect.Framework.Memory.Buffers;

namespace Intersect.Framework.Networking.UdpSample;

internal class Server : SampleConnection
{
    private UdpClient udpClient;
    private IPEndPoint localEndPoint;

    public Server(IPEndPoint endPoint) : base(nameof(Server))
    {
        localEndPoint = endPoint;
    }

    public Server Listen()
    {
        udpClient = new UdpClient(localEndPoint);

        Start();

        return this;
    }

    public override int Send(IPEndPoint? remoteEndPoint, ReadOnlySpan<byte> data)
    {
        return udpClient.Send(data, remoteEndPoint);
    }

    protected override byte[] DoReceive(ref IPEndPoint? remoteEndPoint)
    {
        var data = udpClient.Receive(ref remoteEndPoint);
        return data;
    }

    protected override void OnReceive(IPEndPoint? remoteEndPoint, MemoryBuffer message)
    {
        _ = message.Read(out string text);
        _ = message.Read(out int counter);

        var response = new MemoryBuffer((int)message.Length);
        response.Write(text);
        response.Write(counter + 1);
        var buffer = new Span<byte>(new byte[response.Length]);
        response.Position = 0;
        response.Read(buffer, 0, buffer.Length);
        Send(remoteEndPoint, buffer, false);
    }

}
