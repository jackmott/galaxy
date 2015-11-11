﻿using System;
using System.Collections.Generic;
using System.Text;
using XnaGeometry;

namespace GalaxyShared
{
    [Serializable]
    public class SolarSystem
    {
        
        public SectorCoord ParentSectorCoord;        
        public int Index;
        public Vector3 Pos;                


        public Star Star;
        
        public List<Planet> Planets;
        public List<Asteroid> Asteroids;

        [NonSerialized]
        FastRandom rand;
                
        static readonly int[] PlanetCountDistribution = { 0, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 8, 8, 8, 8, 9, 9, 9, 9, 10, 10, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
        


        public SolarSystem(int index, SectorCoord parentSectorCoord, Vector3 pos)
        {
            rand = new FastRandom(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), Convert.ToInt32(pos.Z));
            ParentSectorCoord = parentSectorCoord;
            Index = index;
            Pos = pos;                    
            Star = new Star(this);
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
            sb.Append("I");
            sb.Append(Index);
            return sb.ToString();
        }

        public void Generate()
        {
            
           

            List<int> availableOrbits = new List<int>();
            int numOrbits = 50;
            for (int i = 0; i < numOrbits; i++)
            {
                availableOrbits.Add(i);
            }
            
            int numPlanets = PlanetCountDistribution[rand.Next(0, PlanetCountDistribution.Length)];

            Planets = new List<Planet>();
            Asteroids = new List<Asteroid>();
            for (int i = 0; i < numPlanets; i++)
            {
                int orbitIndex = rand.Next(0, availableOrbits.Count); 
                int orbit = availableOrbits[orbitIndex];
                availableOrbits.RemoveAt(orbitIndex);
                Planet p = new Planet(this, orbit);
                Planets.Add(p);

            }

            foreach (int orbit in availableOrbits)
            {
                int asteroidChance = rand.Next(0, 100);
                if (asteroidChance == 0)
                {
                    int numAsteroids = rand.Next(20 * orbit, 100 * orbit);
                    for (int i = 0; i < numAsteroids;i++)
                    {
                        double magnitude = rand.Next(5000d, 25000d);
                        Vector3 posAdjust = new Vector3(rand.Next(-magnitude, magnitude), rand.Next(-magnitude, magnitude), rand.Next(-magnitude, magnitude));                        
                        Asteroids.Add(new Asteroid(this, orbit,rand.Next(0.0d,MathHelper.TwoPi),rand.Next(1d,3.5d),posAdjust));
                    }

                }
            }

        }

    }
}
