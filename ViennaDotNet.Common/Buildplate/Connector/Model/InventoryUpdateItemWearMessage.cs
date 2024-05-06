namespace ViennaDotNet.Buildplate.Connector.Model
{
    public record InventoryUpdateItemWearMessage(
        string playerId,
        string itemId,
        string instanceId,
        int wear
    )
    {
    }
}
