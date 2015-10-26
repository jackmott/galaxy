using UnityEngine;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using GalaxyShared;

public class Warp : MonoBehaviour {
    List<TcpClient> clients;
    float speed = 0;
    int sectorCount = 5; //must be odd
    int expandFactor = 5; //multiplied by galaxy coordinates to get unity coordinates
    
    Dictionary<int,ClientSector> loadedSectors;
    Stack<GameObject> starPool;



    // Use this for initialization
    void Start () {

        // Thread thread = new Thread(new ThreadStart(NetworkLoop));
        // thread.Start();
        starPool = new Stack<GameObject>(50000);
        loadedSectors = new Dictionary<int, ClientSector>();
        Camera.main.farClipPlane = GalaxyGen.SECTOR_SIZE * expandFactor * (sectorCount / 2.0f);

    }


	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            speed = speed + .05f;
            
        }

        Camera.main.transform.Translate(Vector3.forward * speed);

        UpdateSectors();
    }

    void UpdateSectors()
    {

        float distanceThreshold = Camera.main.farClipPlane + 100;
        
        Vector3 cameraPos = Camera.main.transform.position;
        
        int x = Convert.ToInt32(cameraPos.x / expandFactor / GalaxyGen.SECTOR_SIZE);
        int y = Convert.ToInt32(cameraPos.y / expandFactor / GalaxyGen.SECTOR_SIZE);
        int z = Convert.ToInt32(cameraPos.z / expandFactor / GalaxyGen.SECTOR_SIZE);


        
        int minX = x - (sectorCount - 1) / 2;
        int minY = y - (sectorCount - 1) / 2;
        int minZ = z - (sectorCount - 1) / 2;
        
        
        List<int> itemsToRemove = new List<int>();
        foreach (ClientSector cSector in loadedSectors.Values)
        {
            
            x = cSector.pos.x;
            y = cSector.pos.y;
            z = cSector.pos.z;
            Vector3 sectorPos = new Vector3(x * expandFactor * GalaxyGen.SECTOR_SIZE, y * expandFactor * GalaxyGen.SECTOR_SIZE, z * expandFactor * GalaxyGen.SECTOR_SIZE);
            float distance = Vector3.Distance(cameraPos, sectorPos);
            int hash = x + y * GalaxyGen.SECTOR_SIZE + z * GalaxyGen.SECTOR_SIZE * GalaxyGen.SECTOR_SIZE;
            if (distance > distanceThreshold) itemsToRemove.Add(hash);
            
        }

        foreach (int hash in itemsToRemove)
        {
            ClientSector cSector;
            loadedSectors.TryGetValue(hash, out cSector);
            cSector.Dispose(starPool);
            loadedSectors.Remove(hash);                            
        }


        for ( x = minX; x < minX + sectorCount; x++)
        {
            for (y = minY; y < minY + sectorCount; y++)
            {
                for (z = minZ; z < minZ + sectorCount; z++)
                {

      //              Debug.Log("build new sectors xyz: (" + x + "," + y + "," + z + ")");
                    int hash = x + y * GalaxyGen.SECTOR_SIZE + z * GalaxyGen.SECTOR_SIZE * GalaxyGen.SECTOR_SIZE;
                    if (!loadedSectors.ContainsKey(hash))
                    {
                        Vector3 sectorPos = new Vector3(x * expandFactor * GalaxyGen.SECTOR_SIZE, y * expandFactor * GalaxyGen.SECTOR_SIZE, z * expandFactor * GalaxyGen.SECTOR_SIZE);
                        float distance = Vector3.Distance(cameraPos, sectorPos);
                        if (distance <= distanceThreshold)
                        {                         
                            ClientSector cSector = GenStars(x, y, z);
                            loadedSectors.Add(hash, cSector);
                        }
                    }
                    
                }

            }

        }


    }


    
   
    public ClientSector GenStars(int x, int y, int z)
    {

       
                        
        int xAdjust = x * GalaxyGen.SECTOR_SIZE * expandFactor;
        int yAdjust = y * GalaxyGen.SECTOR_SIZE * expandFactor;
        int zAdjust = z * GalaxyGen.SECTOR_SIZE * expandFactor;
        
        GalaxyGen gen = new GalaxyGen();
        GalaxySector sector = new GalaxySector(new GalaxyCoord(x, y, z));
        gen.PopulateSector(sector);

               
        List<GameObject> systems = new List<GameObject>();
        GameObject resourceGo = Resources.Load<GameObject>("star-white");

        
        int colorPropID = Shader.PropertyToID("_color");
        
        

        foreach (SolarSystem system in sector.systems)
        {
            GameObject go;            
            if (starPool.Count == 0)
            {
                go = (GameObject)GameObject.Instantiate(resourceGo, new Vector3(system.coord.x * expandFactor + xAdjust, system.coord.y * expandFactor + yAdjust, system.coord.z * expandFactor + zAdjust), new Quaternion());
               
            }
            else
            {
                go = starPool.Pop();
                go.transform.position = new Vector3(system.coord.x * expandFactor + xAdjust, system.coord.y * expandFactor + yAdjust, system.coord.z * expandFactor + zAdjust);
            }
            go.transform.localScale = new Vector3(system.size/8.0f, system.size/8.0f, 1);
            Renderer r = go.GetComponent<Renderer>();
            if (system.type == GalaxyGen.StarType.Red)
            {                
                r.material.SetColor(colorPropID, Color.red);
                
            }
            else if (system.type == GalaxyGen.StarType.White)
            {               
                r.material.SetColor(colorPropID, Color.white);
            }
            else if (system.type == GalaxyGen.StarType.Yellow)
            {
                r.material.SetColor(colorPropID, Color.yellow);
            }
            else if (system.type == GalaxyGen.StarType.Blue)
            {                
                r.material.SetColor(colorPropID, Color.blue);
            }
            systems.Add(go);
        }        
        
        
        ClientSector cSector = new ClientSector(new GalaxyCoord(x, y, z), systems);

     


        return cSector;
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
