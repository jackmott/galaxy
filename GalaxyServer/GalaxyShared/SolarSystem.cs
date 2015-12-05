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

        public LinkedList<object> Clients;
        public Sector ParentSector;        
                
        static readonly int[] PlanetCountDistribution = { 0, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 8, 8, 9, 9, 9, 10,11};
        


        public SolarSystem(int index, Sector parentSector, Vector3 pos, FastRandom rand)
        {            
            ParentSector = parentSector;
            ParentSectorCoord = parentSector.Coord;
            Index = index;
            Pos = pos;                    
            Star = new Star(this,rand);
        }

        public SolarSystem()
        {
            Asteroids = new List<Asteroid>();
        }

        public string key()
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

            Planets = new List<Planet>();
            Asteroids = new List<Asteroid>();
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

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            SolarSystem s = obj as SolarSystem;
            if ((System.Object)s == null)
            {
                return false;
            }

            // Return true if the fields match:
            return s.Pos.Equals(this.Pos);
        }

        public bool Equals(SolarSystem s)
        {
            // If parameter is null return false:
            if ((object)s == null)
            {
                return false;
            }

            return s.Pos.Equals(this.Pos);
        }

       
    }
}
