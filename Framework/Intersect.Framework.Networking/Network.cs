using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;


namespace Intersect.Framework.Networking;

public abstract class Network : IDisposable, INetwork
{
    private bool disposedValue;
    private readonly List<Connection> _connections;
    private readonly MemoryPool<byte> _bufferPool;

    protected Network()
    {
        _connections = new List<Connection>();
        _bufferPool = MemoryPool<byte>.Shared;
    }

    public NetworkState State { get; private set; }

    public NetworkStatistics Statistics { get; private set; }

    public IReadOnlyList<Connection> Connections => throw new NotImplementedException();

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                State = NetworkState.Disposed;

                foreach (var connection in _connections)
                {
                    connection.Dispose();
                }

                _connections.Clear();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Network()
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

    public void Shutdown()
    {
        throw new NotImplementedException();
    }
}
