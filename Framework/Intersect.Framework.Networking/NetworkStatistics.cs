using System;
using System.Collections.Generic;
using System.Text;


namespace Intersect.Framework.Networking;

public class NetworkStatistics
{
    public int ActiveConnections { get; internal set; }

    public int Listeners { get; internal set; }

    public int Latency { get; internal set; }

    public int TimeAlive { get; internal set; }

    public int TimeConnected { get; internal set; }

    public int TotalBytesSent { get; internal set; }

    public int TotalBytesReceived { get; internal set; }
}
