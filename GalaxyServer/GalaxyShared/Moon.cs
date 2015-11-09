using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared
{ 
    class Moon
    {
        Planet ParentPlanet;
        
        int Orbit;
        int Size;
        
        public Moon(Planet parentPlanet, int orbit)
        {
            ParentPlanet = parentPlanet;            
            Orbit = orbit;
            //Size = (float)r.NextDouble() * 10f + 1f;
        }
    }
}
