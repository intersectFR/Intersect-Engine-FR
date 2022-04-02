using System.Net;

namespace Intersect.Framework.Networking;

public abstract class Connection : IDisposable
{
    protected Connection(Network network, IPEndPoint remoteEndPoint)
    {
        Network = network;
        RemoteEndPoint = remoteEndPoint;
    }

    public Id<Connection> Id { get; internal set; }

    public bool IsDisposed { get; private set; }

    public IPEndPoint LocalEndPoint { get; }

    public Network Network { get; }

    public IPEndPoint RemoteEndPoint { get; }

    public abstract bool TrySend(Message message);

    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException($"{GetType().FullName}:{Id}");
        }

        IsDisposed = true;

        if (disposing)
        {
            // TODO: dispose managed state (managed objects)
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
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
}
