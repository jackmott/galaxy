using UnityEngine;
using System.Collections.Generic;
using GalaxyShared;
using System.Threading;
using Tuple;

public class ClientPlanet : MonoBehaviour,IHasInfo {

    public Planet Planet;        
    public int ROTATION_RATE = 10;

   
	// Use this for initialization
	void Start () {

        ImprovedPerlinNoise perlin = new ImprovedPerlinNoise(1);
        perlin.LoadResourcesFor3DNoise();
        GetComponent<Renderer>().material.SetTexture("_PermTable2D", perlin.GetPermutationTable2D());
        GetComponent<Renderer>().material.SetTexture("_Gradient3D", perlin.GetGradient3D());


    }
	
	// Update is called once per frame
	void Update () {
        
           transform.Rotate(Vector3.up, (float)(ROTATION_RATE* 10 * Time.deltaTime));
        
        
        
       // GetComponent<Renderer>().material.SetFloat("_Frequency", Planet.Frequency/5.0f);
      //  GetComponent<Renderer>().material.SetFloat("_Lacunarity", Planet.Lacunarity);
       // GetComponent<Renderer>().material.SetFloat("_Gain", Planet.Gain);
      //  GetComponent<Renderer>().material.SetInt("_Octaves", Planet.Octaves);


    }

    
    
    public Info GetInfo()
    {
        Info info;
        info.Title = "Planet";
        Tuple<string, string> Size = new Tuple<string, string>("Size", Planet.Size.ToString());
        Tuple<string, string> Rotation = new Tuple<string, string>("Rotation Rate", Planet.RotationRate.ToString());
        Tuple<string, string> Orbit = new Tuple<string, string>("Orbit", Planet.Orbit.ToString());

        info.Specs = new List<Tuple<string, string>>();
        info.Specs.Add(Size);
        info.Specs.Add(Rotation);
        info.Specs.Add(Orbit);

        
        info.Actions = new List<Tuple<string, string>>();
        
        return info;
    }
}
