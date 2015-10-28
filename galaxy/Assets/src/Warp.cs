using UnityEngine;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using GalaxyShared;

public class Warp : MonoBehaviour {
    
    float speed = 0;
    int sectorCount = 5; //must be odd
    int expandFactor = 5; //multiplied by galaxy coordinates to get unity coordinates
    
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
        colorPropID = Shader.PropertyToID("_color");
        resourceGo = Resources.Load<GameObject>("star-object");
        material = Resources.Load<Material>("star-material");

        GameObject OriginalParticlePrefab = Resources.Load<GameObject>("StarParticles");


        loadedSectors = new Dictionary<int, ClientSector>();
        for (int i = 0; i < sectorCount*sectorCount*sectorCount; i++)
        {
            
            ClientSector c = new ClientSector();
            GameObject go = (GameObject)GameObject.Instantiate(OriginalParticlePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            ParticleSystem p = go.GetComponent<ParticleSystem>();
            c.particleSystem = p;
            c.active = false;
            c.hash = i+GalaxyGen.GALAXY_SIZE_LIGHTYEARS+10;
            loadedSectors.Add(c.hash, c);
        }
        Camera.main.farClipPlane = GalaxyGen.SECTOR_SIZE * expandFactor * (sectorCount / 2.0f);

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
        
        int x = Convert.ToInt32(cameraPos.x / expandFactor / GalaxyGen.SECTOR_SIZE);
        int y = Convert.ToInt32(cameraPos.y / expandFactor / GalaxyGen.SECTOR_SIZE);
        int z = Convert.ToInt32(cameraPos.z / expandFactor / GalaxyGen.SECTOR_SIZE);


        
        int minX = x - (sectorCount - 1) / 2;        
        int minY = y - (sectorCount - 1) / 2;        
        int minZ = z - (sectorCount - 1) / 2;
        
                        
        foreach (ClientSector cSector in loadedSectors.Values)
        {
            
            x = cSector.coord.x;
            y = cSector.coord.y;
            z = cSector.coord.z;

            Vector3 sectorPos = new Vector3(x * expandFactor * GalaxyGen.SECTOR_SIZE, y * expandFactor * GalaxyGen.SECTOR_SIZE, z * expandFactor * GalaxyGen.SECTOR_SIZE);

            float distance = Vector3.Distance(cameraPos, sectorPos);

            int hash = x + y * GalaxyGen.SECTOR_SIZE + z * GalaxyGen.SECTOR_SIZE * GalaxyGen.SECTOR_SIZE;

            if (distance > distanceThreshold) cSector.Dispose();
            
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
                                removeSector.Activate(x, y, z, GenStars(x,y, z));
                                loadedSectors.Add(removeSector.hash, removeSector);
                            }
                            
                        } // if distance <= distanceThreshold
                    }// if sector doesn't already exist
                    
                }//z

            }//y

        }//x


    }//updatesectors



    public ParticleSystem.Particle[] GenStars(int x, int y, int z)
    {

        int xAdjust = x * GalaxyGen.SECTOR_SIZE * expandFactor;
        int yAdjust = y * GalaxyGen.SECTOR_SIZE * expandFactor;
        int zAdjust = z * GalaxyGen.SECTOR_SIZE * expandFactor;

        
        GalaxySector sector = new GalaxySector(new SectorCoord(x, y, z));
        gen.PopulateSector(sector, 1);

                
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[sector.systems.Count];
        int i = 0;        
        foreach (SolarSystem system in sector.systems)
        {
            particles[i].position = new Vector3(system.coord.x * expandFactor + xAdjust, system.coord.y * expandFactor + yAdjust, system.coord.z * expandFactor + zAdjust);
            particles[i].size = .5f;
            particles[i].color = new Color(system.color.R / 128f, system.color.G / 128f, system.color.B / 128f);            
            //particles[i].color = Color.red;
            i++;
        }
        

        return particles;
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
