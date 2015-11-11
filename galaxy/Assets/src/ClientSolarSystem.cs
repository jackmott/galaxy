using UnityEngine;
using GalaxyShared;
using System;

public class ClientSolarSystem : MonoBehaviour
{

    public static SolarSystem SolarSystem;  
    public static Cubemap Cubemap;
    public static GameObject Recticle;
    


    //private GameObject[] Planets;
    //private GameObject[] Asteroids;


    // Use this for initialization
    void Start()
    {
        //SolarSystem = new SolarSystem(NetworkManager.PlayerState.Location.SystemPos);
        SolarSystem = NetworkManager.PlayerState.SolarSystem;
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
        
        
        GeneratePlanets();
        GenerateAsteroids();

        Recticle = GameObject.FindGameObjectWithTag("Recticle").gameObject;
        

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
            lr.SetWidth(20f, 20f);
            int vertexCount = Convert.ToInt32(100f * (p.Orbit/5f));
            lr.SetVertexCount(vertexCount + 1);

            for (int i = 0; i < vertexCount + 1; i++)
            {
                float angle = ((float)i / (float)vertexCount) * Mathf.PI * 2f;
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (p.Orbit + 1) * Planet.EARTH_CONSTANT * Planet.ORBIT_MULTIPLIER;                
                lr.SetPosition(i, pos);
            }            

        }
        

    }

    public void GenerateAsteroids()
    {
        GameObject resourceAsteroid = Resources.Load<GameObject>("Asteroid");
        foreach (Asteroid a in SolarSystem.Asteroids)
        {
            GameObject asteroidGO = (GameObject)Instantiate(resourceAsteroid, Utility.UVector(a.Pos), UnityEngine.Random.rotation);            
            asteroidGO.transform.localScale *= (float)a.Size * Planet.EARTH_CONSTANT;          
        }

    }

  
    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        Recticle.SetActive(false);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            GameObject o = hit.collider.gameObject;
            if (o.tag == "Planet" || o.tag == "Asteroid" || o.tag == "Star")
            {                
                Vector3 ScreenPos = Camera.main.WorldToScreenPoint(o.transform.position);
                Recticle.SetActive(true);
                Recticle.transform.position = new Vector3(ScreenPos.x, ScreenPos.y, 0);
                if (Input.GetMouseButtonUp(1))
                {
                    IClickable i = o.GetComponent<IClickable>();
                    i.OnRightClick();
                } else if (Input.GetMouseButtonUp(0))

                {
                    IClickable i = o.GetComponent<IClickable>();
                    i.OnLeftClick();
                }             
            }
        }

    }
}
