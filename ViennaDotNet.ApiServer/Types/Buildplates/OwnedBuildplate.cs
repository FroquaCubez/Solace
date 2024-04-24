using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Reflection;
using System.Runtime.Serialization;

namespace ViennaDotNet.ApiServer.Types.Buildplates
{
    public record OwnedBuildplate(
        string id,
        string templateId,
        Dimension dimension,
        Offset offset,
        int blocksPerMeter,
        OwnedBuildplate.Type type,
        SurfaceOrientation surfaceOrientation,
        string model,
        int order,
        bool locked,
        int requiredLevel,
        bool isModified,
        string lastUpdated,
        int numberOfBlocks,
        string eTag
    )
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Type
        {
            [EnumMember(Value = "Survival")] SURVIVAL
        }
    }
}
