using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared
{
    public class Planet
    {
        public SolarSystem ParentSystem;
        public int Orbit;
        public float OrbitAngle;
        public float RotationRate;
        public int Hash;
        public float Size;
        

        public Planet(SolarSystem parentSystem, int orbit, Random r)
        {

            Hash = orbit ^ parentSystem.Hash;            
            ParentSystem = parentSystem;
            Orbit = orbit;
            RotationRate = GalaxyGen.RandomRange(r, .01f, .1f);
            OrbitAngle = GalaxyGen.RandomRange(r, 0f, 360f);
            Size = GalaxyGen.RandomRange(r, .25f, 14f);

        }
    }
}
