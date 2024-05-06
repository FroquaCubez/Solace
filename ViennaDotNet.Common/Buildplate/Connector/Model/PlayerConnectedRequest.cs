namespace ViennaDotNet.Buildplate.Connector.Model
{
    public record PlayerConnectedRequest(
        string uuid,
        string joinCode
    )
    {
    }
}
