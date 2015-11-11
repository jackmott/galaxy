using XnaGeometry;
using System;
namespace GalaxyShared
{
    [Serializable]
    public struct Location
    {
        public SectorCoord SectorCoord;
        public bool InWarp;
        public Vector3 SystemPos;
        public Vector3 Pos;

    }
}
