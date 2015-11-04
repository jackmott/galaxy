using UnityEngine;
using GalaxyShared;
using GalaxyShared.Networking.Messages;

public class ClientSolarSystem : MonoBehaviour
{

    public static SolarSystem SolarSystem;
    public static Cubemap Cubemap;
    public static Quaternion CameraRotation;
    public static Vector3 PlayerStartPos;


    // Use this for initialization
    void Start()
    {
        if (SolarSystem == null)
        {
            GalaxyGen gen = new GalaxyGen();
            GalaxySector sector = gen.GetSector(new SectorCoord(0, 0, 0), 1);
            SolarSystem = sector.Systems[0];
        }

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

        if (Cubemap != null)
        {         
           Camera.main.GetComponent<Skybox>().material.SetTexture("_Tex", Cubemap);
           Camera.main.transform.rotation = CameraRotation;
           Camera.main.transform.Translate(Vector3.back * Planet.EARTH_CONSTANT * 50);
         
        }       
        else 
        {
            Camera.main.transform.Translate(Vector3.back * Planet.EARTH_CONSTANT * 50);
        }
        


    }
   

    public void GeneratePlanets()
    {
        GameObject resourcePlanet = Resources.Load<GameObject>("Planet");
        foreach (Planet p in SolarSystem.Planets)
        {
            GameObject planetGO = (GameObject)Instantiate(resourcePlanet, p.pos, Quaternion.AngleAxis(p.OrbitAngle, Vector3.up));       
            planetGO.transform.localScale *= p.Size * Planet.EARTH_CONSTANT;
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
            GameObject asteroidGO = (GameObject)Instantiate(resourceAsteroid, a.pos, Random.rotation);            
            asteroidGO.transform.localScale *= a.Size * Planet.EARTH_CONSTANT;          
        }

    }

  
    // Update is called once per frame
    void Update()
    {

    }
}
