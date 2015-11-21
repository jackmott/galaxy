using XnaGeometry;
using ProtoBuf;
namespace GalaxyShared
{
    [ProtoContract]
    public struct Location
    {
        [ProtoMember(1)]
        public SectorCoord SectorCoord;
        [ProtoMember(2)]
        public bool InWarp;
        [ProtoMember(3)]
        public Vector3 SystemPos;
        [ProtoMember(4)]
        public Vector3 Pos;

    }
}
