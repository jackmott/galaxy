using UnityEngine;
using GalaxyShared;
using System.Collections.Generic;


public class ClientSolarSystem : MonoBehaviour
{

    private static SolarSystem SolarSystem;
    public static Cubemap Cubemap;       
    public GameObject Ship;    
    public Dictionary<int, Asteroid> asteroidDictionary;


    //private GameObject[] Planets;
    //private GameObject[] Asteroids;


    // Use this for initialization
    void Start()
    {
        //SolarSystem = new SolarSystem(NetworkManager.PlayerState.Location.SystemPos);
        SolarSystem = NetworkManager.PlayerState.SolarSystem;
        asteroidDictionary = new Dictionary<int, Asteroid>(1000);
        GameObject star = (GameObject)Instantiate(Resources.Load<GameObject>("Star"), Vector3.zero, Quaternion.identity);
        ClientStar cs = star.GetComponent<ClientStar>();
        cs.SetStar(SolarSystem.Star);
        star.transform.position = Vector3.zero;
        star.transform.localScale *= SolarSystem.Star.Size * Planet.EARTH_CONSTANT * 30;
        GameObject light = (GameObject)Instantiate(Resources.Load<GameObject>("StarLight"), Vector3.zero, Quaternion.identity);
        light.transform.position = Vector3.zero;
        Light l = light.GetComponent<Light>();
        LensFlare flare = star.GetComponent<LensFlare>();
        Color c = new Color(SolarSystem.Star.Color.r, SolarSystem.Star.Color.g,  SolarSystem.Star.Color.b);
        flare.color = c;

        l.color = Color.Lerp(Color.white, c,.5f);
        


        GeneratePlanets();
        GenerateAsteroids();




        //  Planets = GameObject.FindGameObjectsWithTag("Planet");
        //        Asteroids = GameObject.FindGameObjectsWithTag("Asteroid");

        //load stars if we have em
        if (Cubemap != null)
        {
            Camera.main.GetComponent<Skybox>().material.SetTexture("_Tex", Cubemap);

        }




        //if the server said we start here, we start here, otherwise back away from the sun
        if (NetworkManager.PlayerState == null)
        {
            Ship.transform.Translate(Vector3.back * Planet.EARTH_CONSTANT * 60);
        }
        else
        {
            Ship.transform.position = Utility.UVector(NetworkManager.PlayerState.Location.Pos);
            Ship.transform.rotation = Utility.UQuaternion(NetworkManager.PlayerState.Rotation);
        }

    }


    public void GeneratePlanets()
    {
        GameObject resourcePlanet = Resources.Load<GameObject>("Planet");
        foreach (Planet p in SolarSystem.Planets)
        {
            p.ParentSystem = SolarSystem;
            GameObject planetGO = (GameObject)Instantiate(resourcePlanet, Utility.UVector(p.Pos), Quaternion.identity);
            planetGO.transform.localScale *= (float)p.Size * Planet.EARTH_CONSTANT;
            ClientPlanet cp = planetGO.GetComponent<ClientPlanet>();
            cp.Planet = p;

        }


    }

    public void GenerateAsteroids()
    {
        GameObject resourceAsteroid = Resources.Load<GameObject>("Asteroid");
        foreach (Asteroid a in SolarSystem.Asteroids)
        {            
            a.ParentSystem = SolarSystem;
            asteroidDictionary.Add(a.Hash, a);
            GameObject asteroidGO = (GameObject)Instantiate(resourceAsteroid, Utility.UVector(a.Pos), UnityEngine.Random.rotation);
            asteroidGO.transform.localScale *= (float)(a.Size * Asteroid.CLIENT_SIZE_MULTIPLIER);
            ClientAsteroid ca = asteroidGO.GetComponent<ClientAsteroid>();
            Renderer r = asteroidGO.GetComponent<Renderer>();
            r.material.color = new Color(.5f, .2f, .1f);
            ca.SetAsteroid(a);
            a.GameObject = asteroidGO;
        }

    }

    public void UpdateAsteroid(int hash, byte remaining)
    {
        Asteroid a = null;
        asteroidDictionary.TryGetValue(hash, out a);
        if (remaining > 0 )
        {
            a.Remaining = remaining;
        } else
        {
            Destroy((GameObject)a.GameObject);
            SolarSystem.Asteroids.Remove(a);
            asteroidDictionary.Remove(hash);
        }


    }

    

    // Update is called once per frame
    void Update()
    {

    }
}
