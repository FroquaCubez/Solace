namespace ViennaDotNet.Buildplate.Connector.Model
{
    public record InventorySetHotbarMessage(
        string playerId,
        InventorySetHotbarMessage.Item[] items
    )
    {
        public record Item(
            string itemId,
            int count,
            string? instanceId
        )
        {
        }
    }
}
