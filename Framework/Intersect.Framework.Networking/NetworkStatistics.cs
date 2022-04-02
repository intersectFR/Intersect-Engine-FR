namespace Intersect.Framework.Networking;

public class NetworkStatistics
{
    private long _activeConnections;
    private long _latency;
    private long _listeners;
    private long _timeAlive;
    private long _timeConnected;
    private long _totalBytesReceived;
    private long _totalBytesSent;
    private long _totalPacketsLost;
    private long _totalPacketsReceived;
    private long _totalPacketsSent;

    public long ActiveConnections
    {
        get => Interlocked.Read(ref _activeConnections);
        internal set => Interlocked.Exchange(ref _activeConnections, value);
    }

    public long Latency
    {
        get => Interlocked.Read(ref _latency);
        internal set => Interlocked.Exchange(ref _latency, value);
    }

    public long Listeners
    {
        get => Interlocked.Read(ref _listeners);
        internal set => Interlocked.Exchange(ref _listeners, value);
    }

    public long TimeAlive
    {
        get => Interlocked.Read(ref _timeAlive);
        internal set => Interlocked.Exchange(ref _timeAlive, value);
    }

    public long TimeConnected
    {
        get => Interlocked.Read(ref _timeConnected);
        internal set => Interlocked.Exchange(ref _timeConnected, value);
    }

    public long TotalBytesReceived
    {
        get => Interlocked.Read(ref _totalBytesReceived);
        internal set => Interlocked.Exchange(ref _totalBytesReceived, value);
    }

    public long TotalBytesSent
    {
        get => Interlocked.Read(ref _totalBytesSent);
        internal set => Interlocked.Exchange(ref _totalBytesSent, value);
    }

    public long TotalPacketsLost
    {
        get => Interlocked.Read(ref _totalPacketsLost);
        internal set => Interlocked.Exchange(ref _totalPacketsLost, value);
    }

    public long TotalPacketsReceived
    {
        get => Interlocked.Read(ref _totalPacketsReceived);
        internal set => Interlocked.Exchange(ref _totalPacketsReceived, value);
    }

    public long TotalPacketsSent
    {
        get => Interlocked.Read(ref _totalPacketsSent);
        internal set => Interlocked.Exchange(ref _totalPacketsSent, value);
    }

    public long AddBytesReceived(long bytes) => Interlocked.Add(ref _totalBytesReceived, bytes);

    public long AddBytesSent(long bytes) => Interlocked.Add(ref _totalBytesSent, bytes);

    public long IncrementPacketsLost() => Interlocked.Increment(ref _totalPacketsLost);

    public long AddPacketsLost(long bytes) => Interlocked.Add(ref _totalPacketsLost, bytes);

    public long IncrementPacketsReceived() => Interlocked.Increment(ref _totalPacketsReceived);

    public long AddPacketsReceived(long bytes) => Interlocked.Add(ref _totalPacketsReceived, bytes);

    public long IncrementPacketsSent() => Interlocked.Increment(ref _totalPacketsSent);

    public long AddPacketsSent(long bytes) => Interlocked.Add(ref _totalPacketsSent, bytes);

    internal void Reset()
    {
        ActiveConnections = 0;
        Latency = 0;
        Listeners = 0;
        TimeAlive = 0;
        TimeConnected = 0;
        TotalBytesReceived = 0;
        TotalBytesSent = 0;
        TotalPacketsLost = 0;
        TotalPacketsReceived = 0;
        TotalPacketsSent = 0;
    }
}
