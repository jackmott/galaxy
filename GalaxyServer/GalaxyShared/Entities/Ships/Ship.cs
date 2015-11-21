
using System.Collections.Generic;
using ProtoBuf;
namespace GalaxyShared
{
    [ProtoContract]
    public class Ship : Entity
    {
        public Player Owner;

        [ProtoMember(1)]
        public int TopSpeed;
        [ProtoMember(2)]
        public string Name;
        [ProtoMember(3)]
        public string TypeName;
        [ProtoMember(4)]
        public string Description;
        [ProtoMember(5)]
        public int CargoVolume;
        [ProtoMember(6)]
        public int MiningLaserRange;        
        [ProtoMember(7)]
        public List<Item> Cargo;
        [ProtoMember(8)]
        public int MiningLaserPower;

        public Ship(Player owner)
        {
            Owner = owner;
        }

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
