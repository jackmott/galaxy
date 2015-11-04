using System;
namespace GalaxyShared
{
    [Serializable]
    public struct SectorCoord
    {
        public int X;
        public int Y;
        public int Z;

        public SectorCoord(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

       
    }

    [Serializable]
    public struct Coord
    {
        public float X;
        public float Y;
        public float Z;

        public Coord(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

       
    }
}
