namespace Intersect.Framework.Networking;

[Flags]
public enum NetworkState : byte
{
    Uninitialized = 0,

    Initialized = 0x1,
    Listening = Initialized | 0x2,
    Disconnected = Initialized,

    Connected = 0x10 | Listening,
    Reconnecting = 0x20 | Listening,
    ShutdownRequested = 0x40 | Listening,
    
    Disposed = 0x10 | Disconnected,
}
