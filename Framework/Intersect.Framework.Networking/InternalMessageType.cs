namespace Intersect.Framework.Networking;

internal enum InternalMessageType : byte
{
    Error = 0,
    Ping,
    Pong,
    MssRequest,
    MssAck,
    DiscoveryRequest,
    DiscoveryResponse,
    ConnectionRequest,
    ConnectionResponse,
    Disconnect,
    DisconnectAcknowledge,
    Terminate,
    Acknowledge,
    Broadcast,
    Unconnected,
}
