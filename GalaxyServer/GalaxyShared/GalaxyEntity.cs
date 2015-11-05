using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaGeometry;

namespace GalaxyShared
{
    [Serializable]
    public class GalaxyEntity
    {
        public int SystemHash;
        public Vector3 Pos;

        public GalaxyEntity()
        {

        }
    }
}
