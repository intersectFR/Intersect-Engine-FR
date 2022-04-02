using System;
using System.Collections.Generic;
using System.Text;

namespace Intersect.Framework.Networking;
internal interface INetwork : IDisposable
{
    IReadOnlyList<Connection> Connections { get; }

    NetworkState State { get; }

    NetworkStatistics Statistics { get; }

    void Shutdown();
}
