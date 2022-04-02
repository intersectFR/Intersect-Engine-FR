using System;
using System.Collections.Generic;
using System.Net;
using System.Text;


namespace Intersect.Framework.Networking.Configuration;

[Serializable]
public class ConnectionConfiguration
{
    public EndPoint EndPoint { get; set; }

    public ConnectionProtocol Protocol { get; set; }

    public int Timeout { get; set; }
}
