using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Intersect.Framework.Networking.UdpSample;

internal interface IConnection
{
    int Send(IPEndPoint? endPoint, ReadOnlySpan<byte> data);
}
