namespace ViennaDotNet.Buildplate.Connector.Model
{
    public record InventoryRemoveItemMessage(
         string playerId,
         string itemId,
         int count,
         string? instanceId
    )
    {
    }
}
