using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared
{
    [Serializable]
    public class Ship : Entity
    {
        public int TopSpeed;
        public string Name;
        public string TypeName;
        public string Description;
        public int CargoVolume;
        public int MiningLaserRange;

        public Player Owner;

        public List<Item> Cargo;

        public int MiningLaserPower;

        public void Update(Ship ship)
        {
            this.Cargo = ship.Cargo;
        }

        public bool AddCargo(Item addItem)
        {
            foreach (Item i in Cargo)
            {
                if (i.GetType() == addItem.GetType())
                {
                    i.Count += addItem.Count;
                    return true;
                }
            }

            Cargo.Add(addItem);
            return true;
            //Todo check against cargo volume
        }

    }
}
