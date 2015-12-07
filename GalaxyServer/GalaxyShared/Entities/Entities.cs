﻿using ProtoBuf;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


namespace GalaxyShared
{
    

    [ProtoContract]
    [ProtoInclude(101, typeof(ConstructionModule))]
    [ProtoInclude(102, typeof(StationModule))]
    [ProtoInclude(103, typeof(Ship))]
    public class Entity
    {
        [ProtoMember(1)]
        public Location Location;
        [ProtoMember(2)]
        public string Name;
        [ProtoMember(3)]
        public string Description;

        protected long LastUpdateMillis;

        public Entity()
        {

        }

        //update anything internal based on server time
        public virtual void UpdateState(long millis)
        {
            //update state
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
        public long BuildTimeRemaining;

        

        public ConstructionModule(long startMillis)
        {
            LastUpdateMillis = startMillis;
        }

        public override void UpdateState(long millis)
        {
            BuildTimeRemaining = millis - LastUpdateMillis;
        }

        public bool IsDone()
        {        
            return BuildTimeRemaining <= 0;            
        }

    }

    

    [ProtoContract]
    [ProtoInclude(101, typeof(StationCoupler))]
    [ProtoInclude(102, typeof(Bar))]
    [ProtoInclude(103, typeof(Dock))]    
    public class StationModule : Entity
    {
        [ProtoMember(1)]
        public List<BuildRequirement> BuildRequirements;
        [ProtoMember(2)]
        public int BuildTime;

        public bool CanBuild(Player p)
        {
            foreach (BuildRequirement br in BuildRequirements)
            {
                Item i = p.Ship.Cargo.Find(x => x.ItemType == br.ItemType);
                if (i == null || i.Count < br.Quantity) return false;                
            }
            return true;
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

    }
}
