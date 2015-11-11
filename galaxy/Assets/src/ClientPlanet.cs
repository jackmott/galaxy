using UnityEngine;
using System.Collections;
using GalaxyShared;
using System.Threading;
using System;

public class ClientPlanet : MonoBehaviour,IClickable {

    public Planet Planet;
    public PlanetTextureGenerator pg;

    public int TEXTURE_WIDTH = 2048;
    public int LOW_TEXTURE_WIDTH = 256;
    public int ROTATION_RATE = 10;
    

	// Use this for initialization
	void Start () {

        LODGroup lodGroup = GetComponent<LODGroup>();
        lodGroup.RecalculateBounds();
        //low detail
        PlanetTextureGenerator lowDetailGenerator = new PlanetTextureGenerator(Planet, LOW_TEXTURE_WIDTH, LOW_TEXTURE_WIDTH/2);
        lowDetailGenerator.generatePlanet();
        Texture2D planetTex = new Texture2D(LOW_TEXTURE_WIDTH, LOW_TEXTURE_WIDTH/2, TextureFormat.ARGB32, false);
        planetTex.SetPixels(lowDetailGenerator.GetPlanetColors());
        planetTex.Apply();
        Material material = transform.FindChild("LowPlanet").gameObject.GetComponent<Renderer>().material;
        material.mainTexture = planetTex;
        material = transform.FindChild("HighPlanet").gameObject.GetComponent<Renderer>().material;
        material.mainTexture = planetTex;

        
        
        //high detail
        pg = new PlanetTextureGenerator(Planet, TEXTURE_WIDTH, TEXTURE_WIDTH / 2);
        Thread t = new Thread(new ThreadStart(pg.start));
        t.Priority = System.Threading.ThreadPriority.Lowest;
        t.Start();

    }
	
	// Update is called once per frame
	void Update () {
        if (Planet != null)
        {
           transform.Rotate(Vector3.up, (float)(ROTATION_RATE* Planet.RotationRate * Time.deltaTime));
        }
        if (pg != null && pg.IsReady())
        {
            SetPlanetTexture();
        }
        	
	}

    

    public void SetPlanetTexture()
    {
        Texture2D planetTex = new Texture2D(TEXTURE_WIDTH,TEXTURE_WIDTH/2, TextureFormat.ARGB32, false);                
        planetTex.SetPixels(pg.GetPlanetColors());
        planetTex.Apply();
        Texture2D normalMap = Resources.Load<Texture2D>("PlanetNormals/" + pg.NormalMap);
        Material material = transform.FindChild("HighPlanet").gameObject.GetComponent<Renderer>().material;
        material.SetTexture("_BumpMap", normalMap);
        material.mainTexture = planetTex;
        pg.Finished();
    }

    public void OnLeftClick()
    {
        Debug.Log("Left Click");
    }

    public void OnRightClick()
    {
        Debug.Log("Right Click");
    }
}
