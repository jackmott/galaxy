using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared
{
    public struct GalaxyCoord
    {
        public ushort x;
        public ushort y;
        public ushort z;

        public GalaxyCoord(ushort x, ushort y, ushort z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
