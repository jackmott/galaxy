using ProtoBuf;

namespace GalaxyShared
{
    [ProtoContract]
    public class Entity
    {
        [ProtoMember(1)]
        public Location Location;

        public Entity()
        {

        }
    }
}
