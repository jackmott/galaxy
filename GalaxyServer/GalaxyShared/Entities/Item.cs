using ProtoBuf;

namespace GalaxyShared
{
    [ProtoContract]
    [ProtoInclude(100, typeof(IronOre))]
    public abstract class Item
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public ushort Count;
        [ProtoMember(3)]
        public float Mass;
        [ProtoMember(4)]
        public float Volume;

        public Item()
        { }
    }

    [ProtoContract]
    public class IronOre : Item
    {
        public IronOre()
        {

        }
        public IronOre(ushort amount)
        {
            Name = "Iron Ore";
            Count = amount;
            Mass = 10;
            Volume = 1;
        }
    }
}
