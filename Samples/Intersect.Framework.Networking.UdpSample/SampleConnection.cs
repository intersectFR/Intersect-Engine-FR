using System.Net;
using System.Text;

using Intersect.Framework.Memory.Buffers;

namespace Intersect.Framework.Networking.UdpSample;

internal abstract class SampleConnection : IConnection
{
    private readonly string name;
    private readonly Thread receiveThread;

    protected SampleConnection(string name)
    {
        this.name = name;
        receiveThread = new Thread(ReceiveThread);
    }

    public bool IsRunning { get; set; }

    protected void Start()
    {
        IsRunning = true;
        receiveThread.Start();
    }

    private void ReceiveThread()
    {
        IsRunning = true;
        while (IsRunning)
        {
            IPEndPoint? remoteEndPoint = default;
            var data = DoReceive(ref remoteEndPoint);
            var message = new MemoryBuffer(data);
            Console.WriteLine($"[{name}] recv: {message.Length}");
            OnReceive(remoteEndPoint, message);
        }
    }

    protected abstract byte[] DoReceive(ref IPEndPoint? remoteEndPoint);

    protected abstract void OnReceive(IPEndPoint? remoteEndPoint, MemoryBuffer message);

    public int Send(IPEndPoint? remoteEndPoint, string text)
    {
        Console.WriteLine($"[{name}] send: {remoteEndPoint} {text}");
        var message = new MemoryBuffer();
        message.Write(text);
        message.Write(0);
        var buffer = new Span<byte>(new byte[message.Length]);
        message.Position = 0;
        message.Read(buffer, 0, buffer.Length);
        return Send(remoteEndPoint, buffer, true);
    }

    public int Send(IPEndPoint? remoteEndPoint, ReadOnlySpan<byte> data, bool suppressMessage)
    {
        if (!suppressMessage)
        {
            Console.WriteLine($"[{name}] send: {remoteEndPoint} {Convert.ToBase64String(data)}");
        }

        return Send(remoteEndPoint, data);
    }

    public abstract int Send(IPEndPoint? remoteEndPoint, ReadOnlySpan<byte> data);
}
