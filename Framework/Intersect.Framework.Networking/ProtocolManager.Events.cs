namespace Intersect.Framework.Networking;

internal class ProtocolEventHandlerArgs : EventArgs
{
    public ProtocolEventHandlerArgs(Connection connection, Message? message)
    {
        Connection = connection;
        Message = message;
    }

    public Connection Connection { get; }

    public Message? Message { get; }
}

internal delegate bool ProtocoEventHandler(ProtocolManager sender, ProtocolEventHandlerArgs args);
