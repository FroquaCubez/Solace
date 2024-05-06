using Newtonsoft.Json;
using ViennaDotNet.Common.Utils;

namespace ViennaDotNet.DB.Models.Player
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class RedeemedTappables
    {
        [JsonProperty]
        private Dictionary<string, long> tappables = new();

        public RedeemedTappables()
        {
            // empty
        }

        public bool isRedeemed(string id)
        {
            return tappables.ContainsKey(id);
        }

        public void add(string id, long expiresAt)
        {
            tappables[id] = expiresAt;
        }

        public void prune(long currentTime)
        {
            tappables.RemoveIf(entry => entry.Value < currentTime);
        }
    }
}
