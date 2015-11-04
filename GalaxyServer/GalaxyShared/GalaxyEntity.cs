using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GalaxyShared
{
    [Serializable]
    public class GalaxyEntity
    {
        public int SystemHash;
        public Coord Pos;

        public GalaxyEntity()
        {

        }
    }
}
