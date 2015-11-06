using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared
{
    [Serializable]
    public abstract class GalaxyShip : GalaxyEntity
    {
        public int TopSpeed;
        public string Name;
        public string TypeName;
        public string Description;
        public int CargoVolume;

        
        public GalaxyPlayer Owner;

        public List<GalaxyItem> Cargo;

    }
}
