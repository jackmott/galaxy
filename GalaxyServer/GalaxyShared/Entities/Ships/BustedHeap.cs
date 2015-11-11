using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared
{
    [Serializable]
    class BustedHeap : Ship
    {
        public BustedHeap(Player player)
        {
            Owner = player;            
            Init();
        }

        public BustedHeap()
        {
            Init();
        }

        public void Init()
        {
            TypeName = "Heap of Junk";
            Name = "Unnamed";
            TopSpeed = 100;
            CargoVolume = 100;
            Description = "A small, used personal craft that probably once served as a shuttle.";
            Cargo = new List<Item>();
        }
    }
}
