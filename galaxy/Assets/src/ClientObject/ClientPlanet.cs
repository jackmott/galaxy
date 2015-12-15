using UnityEngine;
using System.Collections.Generic;
using GalaxyShared;
using Tuple;
using FastNoise;
using System.Diagnostics;


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

        NoiseMaker noise = new NoiseMaker(Planet.Octaves,Planet.Lacunarity,Planet.Gain,rand.Next(0.0f,2.0f),Planet.Frequency,(NoiseMaker.NoiseType) rand.Next(0,2));

        Stopwatch s = new Stopwatch();
        s.Start();
        GetComponent<Renderer>().material.SetTexture("_MainTex",noise.GetTextureForSphere(4096,2048,TextureFormat.ARGB32, true,Color.black, Color.red,GenerateColorGradient()));
        s.Stop();
        UnityEngine.Debug.Log(s.ElapsedMilliseconds);
      
        /*
        string normalName = planetNormals[rand.Next(0, planetNormals.Length)];
        Texture2D normalMap = Resources.Load<Texture2D>("PlanetNormals/" + normalName);

        //GetComponent<Renderer>().material.SetTexture("_BumpMap", normalMap);
        //GetComponent<Renderer>().material.SetTexture("_ColorGradient", GenerateColorGradient());                
        ImprovedPerlinNoise perlin = new ImprovedPerlinNoise(rand.Next(1, 1000));
        perlin.LoadResourcesFor3DNoise();
        GetComponent<Renderer>().material.SetTexture("_PermTable2D", perlin.GetPermutationTable2D());
        GetComponent<Renderer>().material.SetTexture("_Gradient3D", perlin.GetGradient3D());
        GetComponent<Renderer>().material.SetFloat("_Frequency", Planet.Frequency / 5.0f);
        GetComponent<Renderer>().material.SetFloat("_Lacunarity", Planet.Lacunarity);
        GetComponent<Renderer>().material.SetFloat("_Gain", Planet.Gain);
        */

    }




    // Update is called once per frame
    void Update()
    {

        transform.Rotate(Vector3.up, (float)(ROTATION_RATE * Planet.RotationRate * Time.deltaTime));

      

    }

    public Color[] GenerateColorGradient()
    {
              
        int numColors = rand.Next(2, 10);
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


        Color[] result = new Color[colorCount];
        int colorIndex = 0;
        for (int i = 0; i < colors.Length-1;i++)
        {
            Color start = colors[i];
            Color end = colors[i + 1];
            
            for (int j = 0; j < ranges[i]; j++)
            {                
                result[colorIndex] = Color.Lerp(start, end, (float)j / (float)ranges[i]);
                colorIndex++;
            }
        }
                
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
