using ProtoBuf;
namespace GalaxyShared
{
    [ProtoContract]
    public struct SectorCoord
    {
        [ProtoMember(0)]
        public int X;
        [ProtoMember(1)]
        public int Y;
        [ProtoMember(2)]
        public int Z;

        public SectorCoord(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

       
    }

  
}
