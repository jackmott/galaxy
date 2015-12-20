using UnityEngine;
using System.Collections.Generic;
using GalaxyShared;
using Tuple;
using FastNoise;
using System.Threading;

public class ClientPlanet : MonoBehaviour, IHasInfo
{

    public Planet Planet;
    public int ROTATION_RATE = 10;
    private bool TextureReady = false;
    static string[] PlanetNormals = { "Cracked", "Dark Dunes", "Drifting Continents", "Extreme", "Frozen Rock", "Gas Giant", "Hilly", "Hurricanes", "Lava Crust", "Lava Valleys", "Lush Green", "Mountains", "Mud Ice And Water", "Rocky", "Sand" };
    private Color[] Gradient;
    private Color[] PlanetColors;
    private NoiseMaker noise;
    FastRandom rand;
    // Use this for initialization
    void Start()
    {

        rand = new FastRandom(transform.position.x, transform.position.y, transform.position.z);
        noise = new NoiseMaker(Planet.Octaves, Planet.Lacunarity, Planet.Gain, rand.Next(0.0f, 2.0f), Planet.Frequency, (NoiseMaker.FractalType)rand.Next(0, 3), (NoiseMaker.NoiseType)rand.Next(0, 1));
        string normalName = PlanetNormals[rand.Next(0, PlanetNormals.Length)];
        Gradient = GenerateColorGradient();
        Texture2D normalMap = Resources.Load<Texture2D>("PlanetNormals/" + normalName);
        GetComponent<Renderer>().material.SetTexture("_BumpMap", normalMap);
        Color[] colors = noise.GetColorForSphere(512, 256,Gradient);
        Texture2D texture = new Texture2D(512, 256, TextureFormat.ARGB32, false);
        texture.SetPixels(colors);
        texture.Apply();
        GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
        Thread t = new Thread(new ThreadStart(BackgroundNoise));
        t.Start();
    }

    private void BackgroundNoise()
    {
        PlanetColors = noise.GetColorForSphere(4096, 2048, Gradient);      
        TextureReady = true;
    }


    // Update is called once per frame
    void Update()
    {

        transform.Rotate(Vector3.up, (float)(ROTATION_RATE * Planet.RotationRate * Time.deltaTime));
        if (TextureReady)
        {
            Texture2D texture = new Texture2D(4096, 2048, TextureFormat.ARGB32, true);
            texture.SetPixels(PlanetColors);
            texture.Apply();
            GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
            TextureReady = false;
        }

    }

    public Color[] GenerateColorGradient()
    {

        int numColors = rand.Next(2, 10);
        Color[] colors = new Color[numColors];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(rand.Next(0.0f, 1.0f), rand.Next(0.0f, 1.0f), rand.Next(0.0f, 1.0f), 1);
        }
        int[] ranges = new int[colors.Length - 1];
        int colorCount = 0;
        for (int i = 0; i < ranges.Length; i++)
        {
            ranges[i] = rand.Next(1, 255);
            colorCount += ranges[i];
        }


        Color[] result = new Color[colorCount];
        int colorIndex = 0;
        for (int i = 0; i < colors.Length - 1; i++)
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
