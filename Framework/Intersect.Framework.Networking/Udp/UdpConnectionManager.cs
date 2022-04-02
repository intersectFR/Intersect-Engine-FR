using System.Net;
using System.Net.Sockets;

using Intersect.Framework.Networking.Configuration;

namespace Intersect.Framework.Networking.Udp;

internal class UdpConnectionManager : ProtocolManager
{
    private readonly Dictionary<IPEndPoint, UdpConnection> _connections;

    private readonly ListenConfiguration? _listenConfiguration;

    private readonly List<IPEndPoint> _localEndPoints;

    private readonly EndPoint _localGenericEndPoint;

    private readonly List<IPEndPoint> _remoteEndPoints;

    private readonly EndPoint? _remoteGenericEndPoint;

    private readonly List<UdpClient> _udpClients;

    private UdpConnectionManager(
        Network network,
        ConnectionConfiguration connectionConfiguration,
        ListenConfiguration? listenConfiguration,
        List<IPEndPoint> localEndPoints,
        EndPoint localGenericEndPoint,
        List<IPEndPoint> remoteEndPoints,
        EndPoint? remoteGenericEndPoint,
        List<UdpClient> udpClients) : base(network, connectionConfiguration)
    {
        _connections = new Dictionary<IPEndPoint, UdpConnection>();
        _listenConfiguration = listenConfiguration;
        _localEndPoints = localEndPoints;
        _localGenericEndPoint = localGenericEndPoint;
        _remoteEndPoints = remoteEndPoints;
        _remoteGenericEndPoint = remoteGenericEndPoint;
        _udpClients = udpClients;
    }

    public override void Listen(CancellationToken cancellationToken)
    {
        var udpReceivers = _udpClients.Select(udpClient => new UdpReceiver(this, udpClient, cancellationToken));
        foreach (var udpReceiver in udpReceivers)
        {
            _ = udpReceiver.Spawn();
        }
    }

    internal void OnReceive(UdpReceiveResult udpReceiveResult)
    {
        if (_connections.TryGetValue(udpReceiveResult.RemoteEndPoint, out var udpConnection))
        {
            
        }
        else
        {
            // TODO: Closed/New connection
        }
    }

    public static async Task<ProtocolManager> Create(
        Network network,
        ConnectionConfiguration connectionConfiguration,
        CancellationToken cancellationToken)
    {
        List<IPEndPoint> endPoints;
        switch (connectionConfiguration.EndPoint)
        {
            case IPEndPoint ipEndPoint:
                endPoints = new List<IPEndPoint> { ipEndPoint };
                break;
            case DnsEndPoint dnsEndPoint:
                var hostEntry = await Dns.GetHostEntryAsync(dnsEndPoint.Host);
                cancellationToken.ThrowIfCancellationRequested();
                endPoints = hostEntry.AddressList.Select(ipAddress => new IPEndPoint(ipAddress, dnsEndPoint.Port)).ToList();
                break;
            default:
                throw new ArgumentException(
                    $"Unsupported end point type {connectionConfiguration.GetType().FullName}",
                    nameof(connectionConfiguration)
                );
        }

        List<IPEndPoint> localEndPoints;
        EndPoint localGenericEndPoint;
        List<IPEndPoint> remoteEndPoints;
        EndPoint? remoteGenericEndPoint;
        List<UdpClient> udpClients;

        var listenConfiguration = connectionConfiguration as ListenConfiguration;
        if (listenConfiguration == default)
        {
            var localEndPoint = new IPEndPoint(IPAddress.Any, endPoints.First().Port);
            localEndPoints = new List<IPEndPoint> { localEndPoint };
            localGenericEndPoint = connectionConfiguration.EndPoint;
            remoteEndPoints = endPoints;
            remoteGenericEndPoint = connectionConfiguration.EndPoint;
            udpClients = new List<UdpClient> { new UdpClient() };
        }
        else
        {
            localEndPoints = endPoints;
            localGenericEndPoint = connectionConfiguration.EndPoint;
            remoteEndPoints = new List<IPEndPoint>();
            remoteGenericEndPoint = default;
            udpClients = endPoints.Select(endPoint => new UdpClient(endPoint)).ToList();
        }

        return new UdpConnectionManager(
            network: network,
            connectionConfiguration: connectionConfiguration,
            listenConfiguration: listenConfiguration,
            localEndPoints: localEndPoints,
            localGenericEndPoint: localGenericEndPoint,
            remoteEndPoints: remoteEndPoints,
            remoteGenericEndPoint: remoteGenericEndPoint,
            udpClients: udpClients);
    }

    public override Connection CreateConnection(IPEndPoint remoteEndPoint)
    {
        throw new NotImplementedException();
    }
}
