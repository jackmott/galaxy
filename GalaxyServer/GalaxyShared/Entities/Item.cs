using ProtoBuf;

namespace GalaxyShared
{
    [ProtoContract]
    public abstract class Item
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public int Count;
        [ProtoMember(3)]
        public double Mass;
        [ProtoMember(4)]
        public double Volume;
    }

    [ProtoContract]
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
