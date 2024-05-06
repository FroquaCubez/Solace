namespace ViennaDotNet.Buildplate.Connector.Model
{
    public record InventoryAddItemMessage(
         string playerId,
         string itemId,
         int count,
         string? instanceId,
         int wear
    )
    {
    }
}
