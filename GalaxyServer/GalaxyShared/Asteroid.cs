using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GalaxyShared
{
    public class Asteroid
    {
        public SolarSystem ParentSystem;
        public int Orbit;
        public float OrbitAngle;
        public float RotationRate;
        public int Hash;
        public float Size;
        public System.Random R;
        public Vector3 pos;
        


        public Asteroid(SolarSystem parentSystem, int orbit, System.Random r)
        {            
            R = r;
            Hash = orbit ^ parentSystem.Hash;
            ParentSystem = parentSystem;
            Orbit = orbit;
            RotationRate = GalaxyGen.RandomRange(r, .01f, .1f);
            OrbitAngle = GalaxyGen.RandomRange(r, 0f, 360f);
            Size = GalaxyGen.RandomRange(r, 1f, 3.5f);

            GameObject go = SolarSystem.go;
            go.transform.position = Vector3.one;
            go.transform.rotation = Quaternion.AngleAxis(OrbitAngle, Vector3.up);
            go.transform.Translate(Vector3.forward * (Orbit + 1) * Planet.EARTH_CONSTANT * 50);
            float magnitude = 5000;
            float xAdjust = GalaxyGen.RandomRange(R, -magnitude, magnitude);
            float yAdjust = GalaxyGen.RandomRange(R, -magnitude, magnitude);
            float zAdjust = GalaxyGen.RandomRange(R, -magnitude, magnitude);
            go.transform.Translate(xAdjust, yAdjust, zAdjust);        
            pos = go.transform.position;

        }
    }
}
