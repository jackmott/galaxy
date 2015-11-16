using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared
{
    [Serializable]
    public abstract class Item
    {
        public string Name;
        public int Count;
        public double Mass;
        public double Volume;
    }

    [Serializable]
    public class IronOre : Item
    {
        public IronOre(int amount)
        {
            Name = "Iron Ore";
            Count = amount;
            Mass = 10;
            Volume = 1;
        }
    }
}
