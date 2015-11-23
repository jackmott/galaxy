using UnityEngine;
using GalaxyShared;
using System;
using Vectrosity;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

public class ClientSolarSystem : MonoBehaviour
{

    private static SolarSystem SolarSystem;
    public static Cubemap Cubemap;

    public GameObject Recticle;

    public GameObject ShipMenu;
    public GameObject Inventory;

    public Material LineMaterial;


    public GameObject Ship;

    public VectorLine MiningLaser;

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
        star.transform.position = Vector3.zero;
        star.transform.localScale *= SolarSystem.Star.Size * Planet.EARTH_CONSTANT * 30;
        GameObject light = (GameObject)Instantiate(Resources.Load<GameObject>("StarLight"), Vector3.zero, Quaternion.identity);
        light.transform.position = Vector3.zero;
        Light l = light.GetComponent<Light>();
        LensFlare flare = star.GetComponent<LensFlare>();
        Color c = new Color(SolarSystem.Star.Color.R / 255f, SolarSystem.Star.Color.G / 255f, SolarSystem.Star.Color.B / 255f);
        flare.color = c;

        l.color = c;
        star.GetComponent<Renderer>().material.SetColor("_EmissionColor", c);


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


     

            int vertexCount = Convert.ToInt32(100f * (p.Orbit / 5f));
            VectorLine orbitLine = new VectorLine("OrbitLine", new List<Vector3>(vertexCount), 1.0f, LineType.Continuous);
            orbitLine.material = LineMaterial;
            orbitLine.MakeCircle(Vector3.zero, Vector3.up, Vector3.Distance(Vector3.zero, Utility.UVector(p.Pos)));
            orbitLine.Draw3DAuto();

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
            asteroidGO.transform.localScale *= (float)a.Size * Planet.EARTH_CONSTANT;
            ClientAsteroid ca = asteroidGO.GetComponent<ClientAsteroid>();
            Renderer r = asteroidGO.GetComponent<Renderer>();
            r.material.color = new Color(.5f, .2f, .1f);
            ca.SetAsteroid(a);
            a.GameObject = asteroidGO;
        }

    }

    public void UpdateAsteroid(int hash, ushort remaining)
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

    public void UpdateInventory()
    {
        StringBuilder sb = new StringBuilder(32);
        sb.Append("<color=\"cyan\">Inventory</color>\n");
        foreach (Item i in NetworkManager.PlayerState.Ship.Cargo)
        {  
            sb.Append("<color=\"magenta\"><</color><color=\"green\">");
            sb.Append(i.Count);
            sb.Append("</color><color=\"magenta\">></color><color=\"cyan\">");
            sb.Append(i.Name);
            sb.Append("</color>\n");
        }
        Inventory.GetComponent<Text>().text = sb.ToString();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey("i"))
        {

            ShipMenu.SetActive(!ShipMenu.activeSelf);
            Inventory.SetActive(!Inventory.activeSelf);

            if (Inventory.activeSelf)
            {
                UpdateInventory();
            }
        }

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
                }
                else if (Input.GetMouseButtonUp(0))

                {
                    IClickable i = o.GetComponent<IClickable>();
                    i.OnLeftClick();
                }
            }
        }

    }
}
