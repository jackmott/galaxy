using ProtoBuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GalaxyShared
{
    [ProtoContract]
    public struct BuildRequirement
    {
        [ProtoMember(1)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemType ItemType;
        [ProtoMember(2)]
        public int Quantity;
    }

}