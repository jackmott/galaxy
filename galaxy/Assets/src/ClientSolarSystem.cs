﻿using UnityEngine;
using GalaxyShared;

public class ClientSolarSystem : MonoBehaviour
{

    SolarSystem SolarSystem;
    

    // Use this for initialization
    void Start()
    {
        if (Warp.SystemToLoad != null)
        {
            SolarSystem = Warp.SystemToLoad;
        }
        else
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
        star.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(SolarSystem.Star.Color.R / 255f, SolarSystem.Star.Color.G / 255f, SolarSystem.Star.Color.B / 255f));
        SolarSystem.Generate();
        GeneratePlanets();
        GenerateAsteroids();

        if (Warp.Cubemap != null)
        {         
           Camera.main.GetComponent<Skybox>().material.SetTexture("_Tex", Warp.Cubemap);
           Camera.main.transform.rotation = Warp.CameraRotation;
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
           

         /*   LineRenderer lr = planetGO.GetComponent<LineRenderer>();
            Color c = new Color(1, 1, 1);
            lr.material = new Material(Shader.Find("Unlit/Color"));
            lr.SetColors(c, c);
            lr.SetWidth(10f, 10f);
            int vertexCount = 50;
            lr.SetVertexCount(vertexCount + 1);

            for (int i = 0; i < vertexCount + 1; i++)
            {

                float angle = ((float)i / (float)vertexCount) * Mathf.PI * 2f;
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (p.Orbit + 1) * EARTH_CONSANT * 50;
                //pos = new Vector3(pos.x + planetGO.transform.position.x, pos.y + planetGO.transform.position.y, pos.z + planetGO.transform.position.z);
                lr.SetPosition(i, pos);
            }
            */


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
