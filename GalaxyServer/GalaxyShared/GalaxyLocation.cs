using XnaGeometry;
using System;
namespace GalaxyShared
{
    [Serializable]
    public struct GalaxyLocation
    {
        public SectorCoord SectorCoord;
        public bool InWarp;
        public int SystemIndex;
        public Vector3 Pos;

    }
}
