using UnityEngine;
using GalaxyShared;
using System;
using System.Runtime.InteropServices;

public class PlanetTextureGenerator
{
    
    
    int width;
    int height;
    

    public string NormalMap;
    FastRandom rand;
    Color[] colors;
    
    ColorRamp colorRamp;

    int octaves;
    float gain;
    float lacunarity;
    float stretch;
    public float rotationRate;

            
    private bool ready = false;
    Noise noise;

    static string[] planetNormals = { "Cracked", "Dark Dunes", "Drifting Continents", "Extreme", "Frozen Rock", "Gas Giant", "Hilly", "Hurricanes", "Lava Crust", "Lava Valleys", "Lush Green", "Mountains", "Mud Ice And Water", "Rocky", "Sand" };


    [DllImport("PerlinNoiseLibrary")]
    private static extern IntPtr GetSphericalPerlinNoise(int width, int height, int octaves, float lacunarity, float gain, float stretch, float offsetx, float offsety);
    [DllImport("PerlinNoiseLibrary")]
    private static extern void CleanUpSphericalPerlinNoise(IntPtr resultArray);


    public PlanetTextureGenerator(Planet p,int width, int height)
    {
        if (p == null) rand = new FastRandom(99,64663, 124, 124);
        else  rand = new FastRandom(p.Pos.X, p.Pos.Y, p.Pos.Z);
        colors = new Color[width * height];
        noise = new Noise();        
        this.width = width;
        this.height = height;
    }
    
    public PlanetTextureGenerator(int width, int height)
    {
        rand = new FastRandom(100f,100f,100f);
        colors = new Color[width * height];
        noise = new Noise();
        this.width = width;
        this.height = height;
    }

    //generate random planets, forever
    public void start()
    {
        Debug.Log("start() pg");

        if (!ready)
        {
            generatePlanet();

        }


    }


    //generate 1 planet, with the current planetinfo
    public void startPlanetInfo()
    {
        Debug.Log("startPlanetInfo() pg");
        generatePlanet();
    }

    public void generatePlanet()
    {
        RandomInfo();
        Generate3DPerlinMapNativeSIMD();
        //Generate3DPerlinMap();
    }

    

    public Color[] GetPlanetColors()
    {
        if (!ready) return null;
        return colors;
    }

        
    public bool IsReady()
    {
        return ready;
    }

    public void Finished()
    {
        ready = false;
    }

    

    private void RandomInfo()
    {
        
        NormalMap = planetNormals[rand.Next( 0, planetNormals.Length)];
        octaves = rand.Next( 1, 6);
        gain =rand.Next( 1f, 7.0f);
        lacunarity =rand.Next( 1f, 7.0f);        
        stretch =rand.Next( 0f, 10f);


        int numColors = 15; // rand.Next(3, 15);
        Color[] colors = new Color[numColors];
        float[] ranges = new float[numColors - 1];
        float percentRemaining = 1f;
        float minPercent = 1f;
        

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(rand.Next(0f, 1f), rand.Next(0f, 1f), rand.Next(0f, 1f), 1f);
            
        }

        for (int i = 0; i < ranges.Length - 1; i++)
        {

           // if (i == 0) // water
           // {
            //    float percent =rand.Next(minPercent, .7f);
            //    ranges[i] = percent;
            //    percentRemaining -= percent;
          //  }
           // else // else
           // {

                int remainingColorsCount = ranges.Length - i - 1;
                float maxPercent = percentRemaining - (minPercent * remainingColorsCount);
                float percent =rand.Next(minPercent, maxPercent);
                ranges[i] = percent;
                percentRemaining -= percent;
           // }

        }
        ranges[ranges.Length - 1] = percentRemaining;               
        colorRamp = new ColorRamp(colors, ranges);

    }

    private void Generate3DPerlinMap()
    {
        Debug.Log("generate3dperlin() pg");

        const float pi = 3.14159265359f;
        const float twopi = pi * 2.0f;

        //float offsetx = (float)rand.Next(-200f, 200f);
        //float offsety = (float)rand.Next(-200f, 200f);

        float offsetx = 0;
        float offsety = 0;

        

        float x3d, y3d, z3d, theta, phi;
        phi = 0;
        int count = 0;

        float sinPhi;

        float piOverHeight = pi / (float)height;
        float twoPiOverWidth = twopi / (float)width;


        noise.Gradient = colorRamp.Gradient;
        for (int y = 0; y < height; y++)
        {

            
            phi += piOverHeight;
            z3d = Mathf.Cos(phi) * stretch;
            sinPhi = Mathf.Sin(phi);
            theta = 0;
            

            for (int x = 0; x < width; x++)
            {
                theta += twoPiOverWidth;

                x3d = Mathf.Cos(theta) * sinPhi;
                y3d = Mathf.Sin(theta) * sinPhi;

                colors[count] = noise.fbm3(x3d*2  + offsetx, y3d*2  + offsety, z3d*2, octaves, gain, lacunarity);               
                count++;

            }
        }
       
        
        //GameObject.Find("Water").renderer.material.color = planetInfo.colorRamp.colors[0];
        //colors = noise.rescaleAndColorArray(floatColors, min, max, colorRamp.Gradient);

        ready = true;
        Debug.Log("ENDgenerate3dperlin() pg");
    }


    private void Generate3DPerlinMapNativeSIMD()
    {
        Debug.Log("generate3dperlin() pg");



        //  float offsetx = (float)rand.Next(-200f, 200f);
        //        float offsety = (float)rand.Next(-200f, 200f);
        float offsetx = 0;
        float offsety = 0;
        IntPtr ptrNativeData = IntPtr.Zero;

        ptrNativeData = GetSphericalPerlinNoise(width, height, octaves, lacunarity, gain, stretch, offsetx, offsety);

        float[] floatColor = new float[width*height];
        IntPtr p = ptrNativeData;

        Marshal.Copy(p, floatColor, 0, width * height);
        CleanUpSphericalPerlinNoise(ptrNativeData);
        for (int i = 0; i < width*height; i++)
        {                               
            colors[i] = colorRamp.Gradient[(int)Mathf.Clamp((floatColor[i]  * (colorRamp.Gradient.Length - 1)),0,255)];                        
        }
      
                        
        ready = true;
        Debug.Log("ENDgenerate3dperlin() pg");
    }

}
