using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace ViennaDotNet.ApiServer.Types.Buildplates
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SurfaceOrientation
    {
        [EnumMember(Value = "Horizontal")] HORIZONTAL,
        [EnumMember(Value = "Vertical")] VERTICAL    // TODO: unverified
    }
}
