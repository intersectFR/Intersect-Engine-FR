using System;
using System.Collections.Generic;
using System.Text;

namespace Intersect.Framework.Networking;

internal interface INetwork : IDisposable
{
    IReadOnlyList<Connection> Connections { get; }

    Connection this[Id<Connection> connectionId] { get; }

    NetworkState State { get; }

    NetworkStatistics Statistics { get; }

    Message CreateMessage();

    void Shutdown();
}
