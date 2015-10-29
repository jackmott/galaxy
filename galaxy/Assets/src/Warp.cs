using UnityEngine;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using GalaxyShared;


[AddComponentMenu("Camera/Warp ")]
public class Warp : MonoBehaviour {
    
    float speed = 0;
    int sectorCount = 10; //must be odd
    public static float expandFactor = 3; //multiplied by galaxy coordinates to get unity coordinates
    public static ClientSector closestSector = null;

    //stuff that solarsystem scene will look up
    public static SolarSystem systemToLoad;
    public static Cubemap cubemap;
    public static Quaternion cameraRotation;
    

    Dictionary<int,ClientSector> loadedSectors;
    

    GameObject resourceGo;
    Material material;
    GalaxyGen gen;

    int colorPropID;


    // Use this for initialization
    void Start () {
        
        
        // Thread thread = new Thread(new ThreadStart(NetworkLoop));
        // thread.Start();
        gen = new GalaxyGen();                
        GameObject OriginalParticlePrefab = Resources.Load<GameObject>("StarParticles");


        //warm up clientsectors
        loadedSectors = new Dictionary<int, ClientSector>();
        for (int i = 0; i < sectorCount*sectorCount*sectorCount; i++)
        {
            
            ClientSector c = new ClientSector();
            GameObject go = (GameObject)GameObject.Instantiate(OriginalParticlePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            ParticleSystem p = go.GetComponent<ParticleSystem>();
            c.particleSystem = p;            
            c.active = false;
            c.hash = i+GalaxySector.GALAXY_SIZE_LIGHTYEARS+10;
            loadedSectors.Add(c.hash, c);
        }
        Camera.main.farClipPlane = GalaxySector.SECTOR_SIZE * expandFactor * (sectorCount / 2.0f);

    }


	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            speed = speed + .05f;
            
        } else if (Input.GetKeyDown("p"))
        {
            speed = 0f;
        }

        Camera.main.transform.Translate(Vector3.forward * speed);

        UpdateSectors();


    }

    void UpdateSectors()
    {

        float distanceThreshold = Camera.main.farClipPlane + 100;
        
        Vector3 cameraPos = Camera.main.transform.position;
        
        int x = Convert.ToInt32(cameraPos.x / expandFactor / GalaxySector.SECTOR_SIZE);
        int y = Convert.ToInt32(cameraPos.y / expandFactor / GalaxySector.SECTOR_SIZE);
        int z = Convert.ToInt32(cameraPos.z / expandFactor / GalaxySector.SECTOR_SIZE);


        
        int minX = x - (sectorCount - 1) / 2;        
        int minY = y - (sectorCount - 1) / 2;        
        int minZ = z - (sectorCount - 1) / 2;


        closestSector = null;
        float minDistance = float.MaxValue;                
        foreach (ClientSector cSector in loadedSectors.Values)
        {
            
            x = cSector.coord.x;
            y = cSector.coord.y;
            z = cSector.coord.z;

            Vector3 sectorPos = new Vector3(x * expandFactor * GalaxySector.SECTOR_SIZE, y * expandFactor * GalaxySector.SECTOR_SIZE, z * expandFactor * GalaxySector.SECTOR_SIZE);

            float distance = Vector3.Distance(cameraPos, sectorPos);

            int hash = x + y * GalaxySector.SECTOR_SIZE + z * GalaxySector.SECTOR_SIZE * GalaxySector.SECTOR_SIZE;

            if (distance > distanceThreshold) cSector.Dispose();
            else if (distance < minDistance)
            {
                minDistance = distance;
                closestSector = cSector;
            }
        }

        if (closestSector.active)
        {
            systemToLoad = closestSector.GetClosestSystem();
            
            if (systemToLoad.clientDistance < .1)
            {
                GenSkybox();
                Application.LoadLevel("SolarSystem");
            }
            
        }

        for ( x = minX; x < minX + sectorCount; x++)
        {
            for (y = minY; y < minY + sectorCount; y++)
            {
                for (z = minZ; z < minZ + sectorCount; z++)
                {

      //              Debug.Log("build new sectors xyz: (" + x + "," + y + "," + z + ")");
                    int hash = x + y * GalaxySector.SECTOR_SIZE + z * GalaxySector.SECTOR_SIZE * GalaxySector.SECTOR_SIZE;
                    if (!loadedSectors.ContainsKey(hash))
                    {
                        Vector3 sectorPos = new Vector3(x * expandFactor * GalaxySector.SECTOR_SIZE, y * expandFactor * GalaxySector.SECTOR_SIZE, z * expandFactor * GalaxySector.SECTOR_SIZE);
                        float distance = Vector3.Distance(cameraPos, sectorPos);
                        
                        if (distance <= distanceThreshold)
                        {

                            ClientSector removeSector = null;
                            foreach (ClientSector sector in loadedSectors.Values)
                            {
                                if (!sector.active)
                                {
                                    removeSector = sector;
                                    break;
                                } //if sector active
                            } // foreach sector

                            if (removeSector != null)
                            {
                                loadedSectors.Remove(removeSector.hash);                                
                                removeSector.Activate(gen.GetSector(new SectorCoord(x,y, z),1));
                                loadedSectors.Add(removeSector.hash, removeSector);
                            }
                            
                        } // if distance <= distanceThreshold
                    }// if sector doesn't already exist
                    
                }//z

            }//y

        }//x


    }//updatesectors

   
    public void GenSkybox()
    {
        closestSector.particleSystem.Clear();
        cubemap = new Cubemap(2048, TextureFormat.ARGB32, false);
        cameraRotation = Camera.main.transform.rotation;
        bool work = Camera.main.RenderToCubemap(cubemap);
        
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
