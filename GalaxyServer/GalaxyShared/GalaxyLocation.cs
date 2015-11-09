using XnaGeometry;
using System;
namespace GalaxyShared
{
    [Serializable]
    public struct GalaxyLocation
    {
        public SectorCoord SectorCoord;
        public bool InWarp;
        public Vector3 SystemPos;
        public Vector3 Pos;

    }
}
