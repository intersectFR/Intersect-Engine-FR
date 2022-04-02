using System.Net;
using System.Net.Sockets;
using System.Text;

using Intersect.Framework.Memory.Buffers;

namespace Intersect.Framework.Networking.UdpSample;

internal class Client : Connection
{
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    private bool isRunning;
    private Thread thread;

    public Client(IPEndPoint endPoint) : base(nameof(Client))
    {
        remoteEndPoint = endPoint;
    }

    public Client Connect()
    {

        udpClient = new UdpClient
        {
            DontFragment = false,
        };

        udpClient.Connect(remoteEndPoint);

        Start();

        return this;
    }

    protected override byte[] DoReceive(ref IPEndPoint? remoteEndPoint)
    {
        var data = udpClient.Receive(ref remoteEndPoint);
        return data;
    }

    public override int Send(IPEndPoint? remoteEndPoint, ReadOnlySpan<byte> data)
    {
        return udpClient.Send(data, remoteEndPoint);
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
        Send(null, buffer, false);
    }
}
