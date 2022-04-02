using Intersect.Framework.Networking.Configuration;

namespace Intersect.Framework.Networking;

public abstract partial class ConnectionManager : IDisposable
{
    private readonly Thread _dispatchThread;
    private readonly object _dispatchThreadLock;
    private readonly Network _network;

    private bool disposedValue;

    protected ConnectionManager(
        Network network,
        ConnectionConfiguration connectionConfiguration)
    {
        _dispatchThread = new Thread(RunDispatchThread);
        _dispatchThreadLock = new object();
        _network = network;

        ConnectionConfiguration = connectionConfiguration;
    }

    public ConnectionConfiguration ConnectionConfiguration { get; }

    private void RunDispatchThread()
    {
        while (!disposedValue && _network.State == NetworkState.Listening) ;
    }

    public abstract void Listen(CancellationToken cancellationToken);

    protected void OnReceive(ReadOnlySpan<byte> data)
    {

    }

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
    // ~ConnectionManager()
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
