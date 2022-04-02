using System;
using System.Collections.Generic;
using System.Text;

namespace Intersect.Framework.Networking;

public interface IServerNetwork
{
    void Listen(ushort port = default);
}
