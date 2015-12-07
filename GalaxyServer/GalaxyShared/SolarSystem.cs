using System;
using System.Collections.Generic;
using System.Text;
using XnaGeometry;
using ProtoBuf;

namespace GalaxyShared
{
    [ProtoContract]
    public class SolarSystem
    {
        [ProtoMember(1)]
        public SectorCoord ParentSectorCoord;
        [ProtoMember(2)]
        public int Index;
        [ProtoMember(3)]
        public Vector3 Pos;

        [ProtoMember(4)]
        public Star Star;

        [ProtoMember(5)]
        public List<Planet> Planets;
        [ProtoMember(6)]
        public List<Asteroid> Asteroids;
        [ProtoMember(7)]
        public List<ConstructionModule> ConstructionModules;
        [ProtoMember(8)]
        public List<StationModule> StationModules;

        public long LastStateUpdate = 0;
        public LinkedList<object> Clients;
        public Sector ParentSector;
        public const int UPDATE_RATE = 500; //ms
                
        static readonly int[] PlanetCountDistribution = { 0, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 8, 8, 9, 9, 9, 10,11};
        


        public SolarSystem(int index, Sector parentSector, Vector3 pos, FastRandom rand)
        {
            InitLists();
            ParentSector = parentSector;
            ParentSectorCoord = parentSector.Coord;
            Index = index;
            Pos = pos;                    
            Star = new Star(this,rand);
        }

        public SolarSystem()
        {
            InitLists();
        }

        private void InitLists()
        {
            Planets = new List<Planet>();
            Asteroids = new List<Asteroid>();
            ConstructionModules = new List<ConstructionModule>();
            StationModules = new List<StationModule>();
        }

        public string Key()
        {
            StringBuilder sb = new StringBuilder(32);
            sb.Append("S");
            sb.Append(ParentSectorCoord.X);
            sb.Append(",");
            sb.Append(ParentSectorCoord.Y);
            sb.Append(",");
            sb.Append(ParentSectorCoord.Z);
            sb.Append(":");
            sb.Append(Index);
            return sb.ToString();
        }

        public void Generate()
        {

            FastRandom Rand = new FastRandom(Pos.X, Pos.Y, Pos.Z);

            List<int> availableOrbits = new List<int>();
            int numOrbits = 25;
            for (int i = 1; i < numOrbits; i++)
            {
                availableOrbits.Add(i);
            }
            
            int numPlanets = PlanetCountDistribution[Rand.Next(0, PlanetCountDistribution.Length)];

          
            for (int i = 0; i < numPlanets; i++)
            {
                int orbitIndex = Rand.Next(0, availableOrbits.Count); 
                int orbit = availableOrbits[orbitIndex];
                availableOrbits.RemoveAt(orbitIndex);
                Planet p = new Planet(this, orbit);
                Planets.Add(p);

            }

            int asteroidIndex = 0;
            foreach (byte orbit in availableOrbits)
            {
                int asteroidChance = Rand.Next(0, 10);
                if (asteroidChance == 0)
                {
                    int numAsteroids = Rand.Next(20 * orbit, 100 * orbit);
                    for (int i = 0; i < numAsteroids;i++)
                    {
                        double magnitude = Rand.Next(50d, 500d);
                        Vector3 posAdjust = new Vector3(Rand.Next(-magnitude, magnitude), Rand.Next(-magnitude, magnitude), Rand.Next(-magnitude, magnitude));                        
                        Asteroids.Add(new Asteroid(this, orbit,Rand.Next(0.0f,(float)MathHelper.TwoPi),(byte)Rand.Next(1,255),posAdjust,asteroidIndex));
                        asteroidIndex++;
                    }

                }
            }

        }

       

       
    }
}
