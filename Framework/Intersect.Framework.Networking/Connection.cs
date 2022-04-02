using System.Net;

namespace Intersect.Framework.Networking;

public abstract class Connection : IDisposable
{
    private bool disposedValue;

    protected Connection(Network network, IPEndPoint remoteEndPoint)
    {
        Network = network;
        RemoteEndPoint = remoteEndPoint;
    }

    public Id<Connection> Id { get; internal set; }

    public IPEndPoint LocalEndPoint { get; }

    public Network Network { get; }

    public IPEndPoint RemoteEndPoint { get; }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Connection()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public abstract bool TrySendData(ReadOnlySpan<byte> data);
}
