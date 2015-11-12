using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared
{
    [Serializable]
    public abstract class Ship : Entity
    {
        public int TopSpeed;
        public string Name;
        public string TypeName;
        public string Description;
        public int CargoVolume;
        public int MiningLaserRange;
        
        public Player Owner;

        public List<Item> Cargo;

    }
}
