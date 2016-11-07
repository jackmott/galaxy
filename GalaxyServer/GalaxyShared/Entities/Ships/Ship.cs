using XnaGeometry;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;

namespace GalaxyShared
{
    [ProtoContract]
    public class Ship : Entity, IMessage

    {
        public Player Owner;

        [ProtoMember(1)]
        public ushort TopSpeed=100;		
        [ProtoMember(2)]
        public string TypeName;        
        [ProtoMember(3)]
        public ushort CargoVolume;
        [ProtoMember(4)]
        public byte MiningLaserRange;        
        [ProtoMember(5)]
        public List<Item> Cargo;
        [ProtoMember(6)]
        public byte MiningLaserPower = 1;

        public Ship()
        {
            Cargo = new List<Item>();
        }

        public Ship(Player owner)
        {
            Owner = owner;
            Cargo = new List<Item>();
			Pos = owner.Pos;
        }

        public void Update(Ship ship)
        {
            this.Cargo = ship.Cargo;
        }

        public bool AddCargo(Item addItem)
        {
            foreach (Item i in Cargo)
            {
                if (i.Name == addItem.Name)
                {
                    i.Count += addItem.Count;
                    return true;
                }
            }

            Cargo.Add(addItem);
            return true;
            //Todo check against cargo volume
        }


        public void Proto(Stream stream, byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.Ship;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);
        }

        public void AcceptHandler(IMessageHandler handler, object o = null)
        {
            handler.HandleMessage(this, o);
        }


    }
}
