using Newtonsoft.Json;

namespace ViennaDotNet.DB.Models.Player.Workshop
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class CraftingSlots
    {
        [JsonProperty]
        public readonly CraftingSlot[] slots;

        public CraftingSlots()
        {
            slots = new CraftingSlot[] { new CraftingSlot(), new CraftingSlot(), new CraftingSlot() };
        }
    }
}
