using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GalaxyShared
{
    public class Planet
    {
        public const float EARTH_CONSTANT = 50;

        public SolarSystem ParentSystem;
        public int Orbit;
        public float OrbitAngle;
        public float RotationRate;
        public int Hash;
        public float Size;
        public Vector3 pos;

        public Planet(SolarSystem parentSystem, int orbit, System.Random r)
        {

            

            
            Hash = orbit ^ parentSystem.Hash;            
            ParentSystem = parentSystem;
            Orbit = orbit;
            RotationRate = GalaxyGen.RandomRange(r, .01f, .1f);
            OrbitAngle = GalaxyGen.RandomRange(r, 0f, 360f);
            Size = GalaxyGen.RandomRange(r, 2.5f, 14f);

            GameObject go = SolarSystem.go;
            go.transform.position = Vector3.one;
            go.transform.rotation = Quaternion.AngleAxis(OrbitAngle, Vector3.up);
            go.transform.Translate(Vector3.forward * (Orbit + 1) * EARTH_CONSTANT * 40);
            pos = go.transform.position;



        }
    }
}
