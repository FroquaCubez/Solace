using static ViennaDotNet.TappablesGenerator.Tappable;

namespace ViennaDotNet.TappablesGenerator
{
    public record Tappable(
        string id,
        float lat,
        float lon,
        long spawnTime,
        long validFor,
        string icon,
        Rarity rarity,
        Drops drops
    )
    {
        public enum Rarity
        {
            COMMON,
            UNCOMMON,
            RARE,
            EPIC,
            LEGENDARY
        }

        public record Drops(
            int experiencePoints,
            Drops.Item[] items
        )
        {
            public record Item(
                string id,
                int count
            )
            {
            }
        }
    }
}
