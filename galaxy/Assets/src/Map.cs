using UnityEngine;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using GalaxyShared;

public class Map : MonoBehaviour
{
    
    float speed = 0;
    
    float expandFactor = .001f; //multiplied by galaxy coordinates to get unity coordinates
        
    GameObject resourceGo;
    Material material;

    GalaxyGen gen;

    // Use this for initialization
    void Start()
    {

        // Thread thread = new Thread(new ThreadStart(NetworkLoop));
        // thread.Start();
        gen = new GalaxyGen();
        resourceGo = Resources.Load<GameObject>("star-object");
        material = Resources.Load<Material>("star-material");

        for (int x = 0; x < 2500; x = x + 30)
        {
            for (int y = 0; y < 2500; y = y + 30)
            {
                for (int z = 0; z < 90; z = z + 30)
                {
                    GenStars(x, y, z, 50);
                }
            }
           
        }


        Camera.main.farClipPlane = 50000;

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            speed = speed + .05f;

        } else if (Input.GetKeyDown("p"))
        {
            speed = 0;
        }

        
        Camera.main.transform.Translate(Vector3.forward * speed);

   
    }

   


    public void GenStars(int x, int y, int z, int everyNth)
    {



        float xAdjust = x * GalaxyGen.SECTOR_SIZE/30;
        float yAdjust = y * GalaxyGen.SECTOR_SIZE/30;
        float zAdjust = z * GalaxyGen.SECTOR_SIZE/30;

        
        GalaxySector sector = new GalaxySector(new SectorCoord(x, y, z));
        gen.PopulateSector(sector,everyNth);


        List<GameObject> systems = new List<GameObject>();

        int colorPropID = Shader.PropertyToID("_color");

        Debug.Log(sector.systems.Count);

        foreach (SolarSystem system in sector.systems)
        {
            GameObject go;

            Vector3 pos = new Vector3((system.coord.x + xAdjust)*expandFactor, (system.coord.y + yAdjust) * expandFactor, (system.coord.z + zAdjust)*expandFactor);
            go = (GameObject)GameObject.Instantiate(resourceGo,pos, new Quaternion());

            
             go.transform.localScale = new Vector3(system.size/50.0f, system.size/50.0f, 1);
            Renderer r = go.GetComponent<Renderer>();
            r.material.SetColor(colorPropID, new Color(system.color.R/128f, system.color.G/128f, system.color.B/128f));
            
        }



    }

      
  }

