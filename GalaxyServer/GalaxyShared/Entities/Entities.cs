using ProtoBuf;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using XnaGeometry;
using System;

namespace GalaxyShared
{
    
    [ProtoContract]
    [ProtoInclude(101, typeof(ConstructionModule))]
    [ProtoInclude(102, typeof(StationModule))]
    [ProtoInclude(103, typeof(Ship))]
    public class Entity
    {
        [ProtoMember(1)]
        public Vector3 Pos;
        [ProtoMember(2)]
        public Quaternion Rotation;
        [ProtoMember(3)]
        public string Name;
        [ProtoMember(4)]
        public string Description;
        [ProtoMember(5)]
        public Guid Guid;

        protected long LastUpdateMillis;

        public Entity()
        {
            Guid = new Guid();
        }

        
        public void SetDataFromJSON()
        {
            StreamReader sr;
            try {
                sr = new StreamReader("Entities/json/" + this.GetType().Name + ".json");
            } catch
            {                
                sr = new StreamReader("Assets/Plugins/GalaxyShared/Entities/json/" + this.GetType().Name + ".json");
            }
            JsonConvert.PopulateObject(sr.ReadToEnd(), this);
        }
        
    }

    [ProtoContract]
    public class ConstructionModule : Entity
    {
        [ProtoMember(1)]
        public List<BuildRequirement> RequirementsRemaining;
        [ProtoMember(2)]
        public int BuildTimeRemaining;
        [ProtoMember(3)]
        public StationModule ResultingStationModule;

        

        public ConstructionModule(long startMillis,Vector3 pos, Quaternion rotation,StationModule resultingStationModule) :base()
        {
            Pos = pos;
            Rotation = rotation;
            ResultingStationModule = resultingStationModule;
            LastUpdateMillis = startMillis;
            BuildTimeRemaining = resultingStationModule.BuildTime;
        }

        public bool UpdateStateAndCheckIfDone(long millis)
        {
            BuildTimeRemaining = (int)(millis - LastUpdateMillis);
            return BuildTimeRemaining <= 0;
        }

        
    }

    

    [ProtoContract]
    [ProtoInclude(101, typeof(StationCoupler))]
    [ProtoInclude(102, typeof(Bar))]
    [ProtoInclude(103, typeof(Dock))]    
    public class StationModule : Entity, IMessage
    {
        [ProtoMember(1)]
        public List<BuildRequirement> BuildRequirements;
        [ProtoMember(2)]
        public int BuildTime;

       
        public StationModule() : base()
        {

        }

        public bool CanBuild(Player p)
        {
            foreach (BuildRequirement br in BuildRequirements)
            {
                Item i = p.Ship.Cargo.Find(x => x.ItemType == br.ItemType);
                if (i == null || i.Count < br.Quantity) return false;                
            }
            return true;
        }
        public void Proto(Stream stream, byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.StationModule;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);
        }

        public void AcceptHandler(IMessageHandler handler, object o = null)
        {
            handler.HandleMessage(this, o);
        }
    }

    [ProtoContract]
    public class StationCoupler : StationModule
    {
        
    }

    [ProtoContract]
    public class Bar : StationModule
    {

    }

    [ProtoContract]
    public class Dock : StationModule
    {
        public Dock() : base()
        {
            BuildTime = 5000;
        }
    }
}
