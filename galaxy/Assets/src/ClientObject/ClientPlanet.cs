using UnityEngine;
using System.Collections.Generic;
using GalaxyShared;
using Tuple;

public class ClientPlanet : MonoBehaviour, IHasInfo
{

    public Planet Planet;
    public int ROTATION_RATE = 10;

    static string[] planetNormals = { "Cracked", "Dark Dunes", "Drifting Continents", "Extreme", "Frozen Rock", "Gas Giant", "Hilly", "Hurricanes", "Lava Crust", "Lava Valleys", "Lush Green", "Mountains", "Mud Ice And Water", "Rocky", "Sand" };

    FastRandom rand;
    // Use this for initialization
    void Start()
    {
        rand = new FastRandom(Planet.Pos.X, Planet.Pos.Y, Planet.Pos.Z);

        string normalName = planetNormals[rand.Next(0, planetNormals.Length)];
        Texture2D normalMap = Resources.Load<Texture2D>("PlanetNormals/" + normalName);

        GetComponent<Renderer>().material.SetTexture("_BumpMap", normalMap);
        GetComponent<Renderer>().material.SetTexture("_ColorGradient", GenerateColorGradient());                
        GetComponent<Renderer>().material.SetFloat("_Frequency", Planet.Frequency / 5.0f);
        GetComponent<Renderer>().material.SetFloat("_Lacunarity", Planet.Lacunarity);
        GetComponent<Renderer>().material.SetFloat("_Persistence", Planet.Gain);


    }




    // Update is called once per frame
    void Update()
    {

        transform.Rotate(Vector3.up, (float)(ROTATION_RATE * Planet.RotationRate * Time.deltaTime));

      

    }

    public Texture2D GenerateColorGradient()
    {
              
        int numColors = rand.Next(1, 6);
        Color[] colors = new Color[numColors];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(rand.Next(0.0f, 1.0f), rand.Next(0.0f, 1.0f), rand.Next(0.0f, 1.0f),1);
        }
        int[] ranges = new int[colors.Length - 1];
        int colorCount = 0;
        for (int i = 0; i < ranges.Length;i++)
        {
            ranges[i] = rand.Next(1, 100);
            colorCount += ranges[i];
        }
        

        Texture2D result = new Texture2D(colorCount, 1, TextureFormat.ARGB32, false);
        int colorIndex = 0;
        for (int i = 0; i < colors.Length-1;i++)
        {
            Color start = colors[i];
            Color end = colors[i + 1];
            
            for (int j = 0; j < ranges[i]; j++)
            {                
                result.SetPixel(colorIndex, 0, Color.Lerp(start, end, (float)j / (float)ranges[i]));
                colorIndex++;
            }
        }
        
        result.Apply();
        return result;

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
