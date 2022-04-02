using System.Net;

namespace Intersect.Framework.Networking.Configuration;

[Serializable]
public class ListenConfiguration : ConnectionConfiguration
{
    public int MaximumConnections { get; set; }
}
