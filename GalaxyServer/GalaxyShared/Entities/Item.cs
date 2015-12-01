using ProtoBuf;

namespace GalaxyShared
{
    public enum ItemType { IronOre }
    [ProtoContract]    
    public class Item
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public ushort Count;
        [ProtoMember(3)]
        public float Mass;
        [ProtoMember(4)]
        public float Volume;
        [ProtoMember(5)]
        public ItemType ItemType;

        public Item()
        { }

        public Item(ItemType itemType, ushort count)
        {
            ItemType = itemType;
            Name = ItemType.ToString();
            Count = count;
        }
    }

    
}
