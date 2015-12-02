using ProtoBuf;
using System.IO;
using Newtonsoft.Json;

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
        [ProtoMember(6)]
        public string Description;

        public Item()
        { }

        public Item(ItemType itemType, ushort count)
        {
            ItemType = itemType;            
            Count = count;
            SetDataFromJSON();
        }

        public void SetDataFromJSON()
        {
            StreamReader sr;
            try
            {
                sr = new StreamReader("Items/json/" + ItemType.ToString() + ".json");
            }
            catch
            {
                sr = new StreamReader("Assets/Plugins/GalaxyShared/Items/json/" + ItemType.ToString() + ".json");
            }
            JsonConvert.PopulateObject(sr.ReadToEnd(), this);
        }
    }

    
}
