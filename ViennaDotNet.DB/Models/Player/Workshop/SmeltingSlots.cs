using Newtonsoft.Json;

namespace ViennaDotNet.DB.Models.Player.Workshop
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class SmeltingSlots
    {
        [JsonProperty]
        public readonly SmeltingSlot[] slots;

        public SmeltingSlots()
        {
            slots = new SmeltingSlot[] { new SmeltingSlot(), new SmeltingSlot(), new SmeltingSlot() };
        }
    }
}
