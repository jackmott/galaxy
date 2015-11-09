using UnityEngine;
using GalaxyShared;


public class ClientSolarSystem : MonoBehaviour
{

    public static SolarSystem SolarSystem;
    public static Cubemap Cubemap;
        

    // Use this for initialization
    void Start()
    {
        SolarSystem = new SolarSystem(NetworkManager.PlayerState.Location.SystemPos);
        
        GameObject star = (GameObject)Instantiate(Resources.Load<GameObject>("Star"), Vector3.zero, Quaternion.identity);
        star.transform.position = Vector3.zero;
        star.transform.localScale *= SolarSystem.Star.Size * Planet.EARTH_CONSTANT * 30;
        GameObject light = (GameObject)Instantiate(Resources.Load<GameObject>("StarLight"), Vector3.zero, Quaternion.identity);
        light.transform.position = Vector3.zero;
        Light l = light.GetComponent<Light>();
        LensFlare flare = star.GetComponent<LensFlare>();        
        Color c = new Color(SolarSystem.Star.Color.R / 255f, SolarSystem.Star.Color.G / 255f, SolarSystem.Star.Color.B / 255f);
        flare.color = c;
        
        l.color = c;
        star.GetComponent<Renderer>().material.SetColor("_EmissionColor",c);
        SolarSystem.Generate();
        GeneratePlanets();
        GenerateAsteroids();

        //load stars if we have em
        if (Cubemap != null)
        {         
           Camera.main.GetComponent<Skybox>().material.SetTexture("_Tex", Cubemap);
           
        }
        

        

        //if the server said we start here, we start here, otherwise back away from the sun
        if (NetworkManager.PlayerState == null)
        {
            Camera.main.transform.Translate(Vector3.back * Planet.EARTH_CONSTANT * 60);
        }
        else
        {
            Camera.main.transform.position = Utility.UVector(NetworkManager.PlayerState.Location.Pos);
            Camera.main.transform.rotation = Utility.UQuaternion(NetworkManager.PlayerState.Rotation);
        }

        
        
    }
   

    public void GeneratePlanets()
    {
        GameObject resourcePlanet = Resources.Load<GameObject>("Planet");
        foreach (Planet p in SolarSystem.Planets)
        {
            GameObject planetGO = (GameObject)Instantiate(resourcePlanet, Utility.UVector(p.Pos), Quaternion.identity);       
            planetGO.transform.localScale *= (float)p.Size * Planet.EARTH_CONSTANT;
            ClientPlanet cp = planetGO.GetComponent<ClientPlanet>();
            cp.Planet = p;
           

            LineRenderer lr = planetGO.GetComponent<LineRenderer>();
            Color c = new Color(1, 1, 1);
            lr.material = new Material(Shader.Find("Unlit/Color"));
            lr.SetColors(c, c);
            lr.SetWidth(10f, 10f);
            int vertexCount = 200;
            lr.SetVertexCount(vertexCount + 1);

            for (int i = 0; i < vertexCount + 1; i++)
            {

                float angle = ((float)i / (float)vertexCount) * Mathf.PI * 2f;
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (p.Orbit + 1) * Planet.EARTH_CONSTANT * 50;                
                lr.SetPosition(i, pos);
            }            

        }

    }

    public void GenerateAsteroids()
    {
        GameObject resourceAsteroid = Resources.Load<GameObject>("Asteroid");
        foreach (Asteroid a in SolarSystem.Asteroids)
        {
            GameObject asteroidGO = (GameObject)Instantiate(resourceAsteroid, Utility.UVector(a.Pos), Random.rotation);            
            asteroidGO.transform.localScale *= (float)a.Size * Planet.EARTH_CONSTANT;          
        }

    }

  
    // Update is called once per frame
    void Update()
    {

    }
}
