using UnityEngine;
using System.Collections;
using GalaxyShared;
using System.Threading;

public class ClientPlanet : MonoBehaviour {

    public Planet Planet;
    public PlanetTextureGenerator pg;

    public int TEXTURE_WIDTH = 2048;
    public int ROTATION_RATE = 10;
    

	// Use this for initialization
	void Start () {
        pg = new PlanetTextureGenerator(Planet, TEXTURE_WIDTH, TEXTURE_WIDTH/2);
        Thread t = new Thread(new ThreadStart(pg.start));
        t.Start();
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Planet != null)
        {
           transform.Rotate(Vector3.up, (float)(ROTATION_RATE* Planet.RotationRate * Time.deltaTime));
        }
        if (pg.IsReady())
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
        Material material = GetComponent<Renderer>().material;
        material.SetTexture("_BumpMap", normalMap);
        material.mainTexture = planetTex;
        pg.Finished();
    }


}
