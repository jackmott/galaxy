using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyShared
{
    class Planet
    {
        SolarSystem parentSystem;

        public Planet(SolarSystem parentsystem)
        {
            this.parentSystem = parentsystem;
        }
    }
}
