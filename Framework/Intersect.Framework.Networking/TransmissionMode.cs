namespace Intersect.Framework.Networking;

/// <summary>
/// The transmission mode for packets, determining their reliability and order behavior.
/// </summary>
[Flags]
public enum TransmissionMode : byte
{
    /// <summary>
    /// Messages are not guaranteed to arrive, but duplicates will be dropped. Not compatible with <see cref="Ordered"/>.
    /// </summary>
    Unreliable = 0x0,

    /// <summary>
    /// Messages are guaranteed to arrive unless the connection is terminated.
    /// </summary>
    Reliable = 0x1,

    /// <summary>
    /// Messages in the same channel are guaranteed to be in order, dropping messages received out of order.
    /// </summary>
    Sequenced = 0x2,

    /// <summary>
    /// Messages in the same channel are guaranteed to be in order, delaying messages received out of order. Not compatible with <see cref="Unreliable"/>.
    /// </summary>
    Ordered = 0x4,
}
