using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared
{ 
    class Moon
    {
        Planet ParentPlanet;
        int Hash;
        int Orbit;
        int Size;
        
        public Moon(Planet parentPlanet, int orbit)
        {
            ParentPlanet = parentPlanet;
            Hash = ParentPlanet.Hash ^ orbit;
            Orbit = orbit;
            //Size = (float)r.NextDouble() * 10f + 1f;
        }
    }
}
