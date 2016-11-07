using ProtoBuf;
namespace GalaxyShared
{
    [ProtoContract]
    public struct SectorCoord
    {
        [ProtoMember(1)]
        public int X;
        [ProtoMember(2)]
        public int Y;
        [ProtoMember(3)]
        public int Z;

        public SectorCoord(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

		public override string ToString()
		{
			return "(" + X + "," + Y + "," + Z + ")";
		}


	}

  
}
