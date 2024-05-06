namespace ViennaDotNet.Buildplate.Connector.Model
{
    public record PlayerConnectedResponse(
        bool accepted,
        PlayerConnectedResponse.Inventory? inventory
    )
    {
        public record Inventory(
            Inventory.Item[] items,
            Inventory.HotbarItem?[] hotbar
        )
        {
            public record Item(
                string id,
                int count,
                string? instanceId,
                int wear
            )
            {
            }

            public record HotbarItem(
                string id,
                int count,
                string? instanceId
            )
            {
            }
        }
    }
}
