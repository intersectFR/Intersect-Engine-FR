using System;
using System.Collections.Generic;
using System.Text;


namespace Intersect.Framework.Networking;

public interface IClientNetwork
{
    void Connect();

    void Disconnect();

    void Send(Message message);
}
