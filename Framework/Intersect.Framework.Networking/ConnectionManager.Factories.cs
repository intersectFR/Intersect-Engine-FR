using Intersect.Framework.Networking.Configuration;
using Intersect.Framework.Networking.Udp;

namespace Intersect.Framework.Networking;

internal delegate Task<ConnectionManager> CreateConnectionManager(
        Network network,
        ConnectionConfiguration connectionConfiguration,
        CancellationToken cancellationToken);

public abstract partial class ConnectionManager
{
    private static readonly CreateConnectionManager[] _factoryMethods;

    static ConnectionManager()
    {
        _factoryMethods = new CreateConnectionManager[Enum.GetValues(typeof(ConnectionProtocol)).Length];
        _factoryMethods[(int)ConnectionProtocol.Udp] = UdpConnectionManager.Create;
    }

    public static Task<ConnectionManager> CreateConnectionManager(
        Network network,
        ConnectionConfiguration connectionConfiguration,
        CancellationToken cancellationToken)
    {
        var factoryMethod = _factoryMethods[(int)connectionConfiguration.Protocol];
        if (factoryMethod == default)
        {
            throw new NotSupportedException($"{connectionConfiguration.Protocol} is not yet supported.");
        }

        return factoryMethod(network, connectionConfiguration, cancellationToken);
    }
}
