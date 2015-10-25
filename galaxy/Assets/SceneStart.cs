using UnityEngine;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using GalaxyShared;

public class SceneStart : MonoBehaviour {
    List<TcpClient> clients;
    float speed = 0;
    float x, y, z;

    // Use this for initialization
    void Start () {

        // Thread thread = new Thread(new ThreadStart(NetworkLoop));
        // thread.Start();

        int sectorCount = 1;

        for (int x = -sectorCount; x <= sectorCount; x=x+1)
        {
            for (int y = -sectorCount; y <= sectorCount; y=y+1)
            {
                for (int z = -sectorCount; z <= sectorCount; z=z+1)
                {
                    GenStars(x, y, z);
                }
            }
        }

        

    }


	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            speed = speed + .01f;
            
        }

        Camera.main.transform.Translate(Vector3.forward * speed);
    }



    public void GenStars(int x, int y, int z)
    {
        
        
        int expandFactor = 5;

        int xAdjust = x * GalaxyGen.SECTOR_SIZE * expandFactor;
        int yAdjust = y * GalaxyGen.SECTOR_SIZE * expandFactor;
        int zAdjust = z * GalaxyGen.SECTOR_SIZE * expandFactor;


        RenderSettings.skybox = null;
        GalaxyGen gen = new GalaxyGen();
        GalaxySector sector = new GalaxySector(new GalaxyCoord(x, y, z));
        gen.PopulateSector(sector);

        Stack<GameObject> systems = new Stack<GameObject>();
        foreach (SolarSystem system in sector.systems)
        {
          
           GameObject go = Resources.Load<GameObject>("star-white");
           GameObject.Instantiate(go);
           go.transform.position = new Vector3(system.coord.x * expandFactor + xAdjust, system.coord.y * expandFactor + yAdjust, system.coord.z * expandFactor + zAdjust);
            //go.transform.LookAt(Camera.main.transform.position, -Vector3.up);
            
           // go.transform.parent = Camera.main.transform;
            systems.Push(go);


        }
    }

    public void GenSkybox(Stack<GameObject> systems)
    {
        

        Cubemap cubemap = new Cubemap(1024, TextureFormat.ARGB32, false);
        Camera.main.RenderToCubemap(cubemap);

        while (systems.Count > 0)
        {
            Destroy(systems.Pop());
        }


        Shader skyshader = Shader.Find("Skybox/Cubemap");
        Material mat = new Material(skyshader);
        mat.SetTexture("_Tex", cubemap);
        //sb.material = mat;
        
         
         RenderSettings.skybox = mat;
    }

    public void NetworkLoop()
    {
        TcpClient socket = new TcpClient("localhost", 8888);
        NetworkStream stream = socket.GetStream();
        
        byte[] buffer = new byte[1024];
        int pos = 0;
        while (true)
        {
            pos = 0;             
            while (pos < 2)
            {
                int bytesRead = stream.Read(buffer, pos, 2);
                if (bytesRead == 0) { 
                    //cleanup
                }
                pos += bytesRead;
            }

            int size = BitConverter.ToInt16(buffer, 0);



            while (pos < size)
            {
                int bytesRead = stream.Read(buffer, pos, size - 2);
                if (bytesRead == 0) {
                    //cleanup
                }
                pos += bytesRead;
            }
            

        }

    }
}
