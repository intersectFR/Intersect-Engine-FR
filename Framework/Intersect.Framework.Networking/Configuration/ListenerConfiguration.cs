namespace Intersect.Framework.Networking.Configuration;

[Serializable]
public class ListenerConfiguration
{
    public List<ListenConfiguration> ListenTo { get; set; }
}
