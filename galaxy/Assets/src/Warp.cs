using UnityEngine;
using System.Collections.Generic;
using System;
using GalaxyShared;




[AddComponentMenu("Camera/Warp ")]
public class Warp : MonoBehaviour {
    
    
    int SectorCount = 9; //must be odd
    ClientSector SectorToRemove = null;
    
    public GameObject Ship;
        
    Dictionary<int,ClientSector> LoadedSectors;

    double distanceThreshold; 


    // Use this for initialization
    void Start () {

        Galaxy.Init();
        // Thread thread = new Thread(new ThreadStart(NetworkLoop));
        // thread.Start();

        GameObject OriginalParticlePrefab = Resources.Load<GameObject>("StarParticles");


        Camera.main.farClipPlane = (float)(Galaxy.SECTOR_SIZE * Galaxy.EXPAND_FACTOR * (SectorCount / 2f));
        distanceThreshold = (double)Camera.main.farClipPlane;
        Ship.transform.rotation = Utility.UQuaternion(NetworkManager.PlayerState.Rotation);
        Ship.transform.position = Utility.UVector(NetworkManager.PlayerState.Location.Pos);
        Ship.transform.Translate(Vector3.forward * .2f);
       

        //warm up clientsectors
        LoadedSectors = new Dictionary<int, ClientSector>();        
        for (int i = 0; i < SectorCount*SectorCount*SectorCount; i++)
        {
            
            ClientSector c = new ClientSector();
            GameObject go = (GameObject)Instantiate(OriginalParticlePrefab,Vector3.zero, Quaternion.identity);            
            ParticleSystem p = go.GetComponent<ParticleSystem>();                    
            c.ParticleSystem = p;            
            c.Active = false;
            c.Hash = i+Galaxy.GALAXY_SIZE_LIGHTYEARS+10;
            LoadedSectors.Add(c.Hash, c);
        }

        int CUBE_SIZE = 256;
        Cubemap background = new Cubemap(CUBE_SIZE, TextureFormat.ARGB32, true);

        SectorCoord sc = NetworkManager.PlayerState.Location.SectorCoord;
        Vector3 player = new Vector3(sc.X, sc.Y, sc.Z);
        Debug.Log("player:" + player);
        int dist = Galaxy.GALAXY_SIZE_SECTORS / 2 - sc.X;
        float minb = 999;
        float maxb = -999;
        for (int x = 0; x < CUBE_SIZE; x = x + 1)
        {
            for (int y = 0; y < CUBE_SIZE; y = y + 1)
            {
                //get far vector
                float sx = Galaxy.GALAXY_SIZE_SECTORS;
                float sz = (y * (Galaxy.GALAXY_SIZE_SECTORS) / CUBE_SIZE) - Galaxy.GALAXY_SIZE_SECTORS / 2;
                float sy = (x * (Galaxy.GALAXY_SIZE_SECTORS) / CUBE_SIZE) - Galaxy.GALAXY_SIZE_SECTORS / 2;
                //  Debug.Log("sxetc:"+sx + "," + sy + "," + sz);
                Vector3 far = player + new Vector3(sx, sy, sz);
//                Debug.Log("far:" + far);
                float distance = Vector3.Distance(player, far);
                float pGap = 100.0f / distance;
                Vector3 ray = far - player;
                //Debug.Log("ray:"+ray);
                float r = 0;
                float g = 0;
                float b = 0;
                for (float pct = 0; pct <= 1; pct += pGap)
                {
                    Vector3 p = player+ ray * pct;
                  //  Debug.Log("p:" + p);
                    if ( Mathf.Abs(p.x) > Galaxy.GALAXY_SIZE_SECTORS/2.0f ||
                         Mathf.Abs(p.y) > Galaxy.GALAXY_SIZE_SECTORS / 2.0f ||
                         Mathf.Abs(p.z) > Galaxy.GALAXY_SIZE_SECTORS / 2.0f)
                    {
                    //    Debug.Log("Early exit");
                        break;
                    }
                    
                    
                    SectorCoord s = new SectorCoord(Convert.ToInt32(p.x), Convert.ToInt32(p.y), Convert.ToInt32(p.z));
                  //  Debug.Log("sector:"+s.X + "," + s.Y + "," + s.Z); 
                    
                    try {
                        System.Drawing.Color cc = Galaxy.GetColorAt(s);
                      //  Debug.Log("cc:" + cc);
                        r += cc.R / 10.0f /  256.0f;
                        g += cc.G / 10.0f /  256.0f;
                        b += cc.B / 10.0f /  256.0f;
//                        Debug.Log("rgb" + r + "," + g + "," + b);

                    } catch (Exception e)
                    {
                        Debug.Log("FAIL");
                        return;
                    }

                   

                }
              //  Debug.Log("rgb" + r + "," + g + "," + b);
                Color c = new Color(r, g, b);
          //      Debug.Log("color:"+c);
                background.SetPixel(CubemapFace.PositiveX, x, y, c);
                background.SetPixel(CubemapFace.NegativeX, x, y, c);
                background.SetPixel(CubemapFace.PositiveY, x, y, c);
                background.SetPixel(CubemapFace.NegativeY, x, y, c);
                background.SetPixel(CubemapFace.PositiveZ, x, y, c);
                background.SetPixel(CubemapFace.NegativeZ, x, y, c);
            }
        }
        
        background.Apply();
        Camera.main.GetComponent<Skybox>().material.SetTexture("_Tex", background);


    }


	// Update is called once per frame
	void Update () {
        
        UpdateSectors();

    }

    void UpdateSectors()
    {

        

        XnaGeometry.Vector3 shipPos = NetworkManager.PlayerState.Location.Pos;

        int x = Convert.ToInt32(shipPos.X / Galaxy.EXPAND_FACTOR / Galaxy.SECTOR_SIZE);
        int y = Convert.ToInt32(shipPos.Y / Galaxy.EXPAND_FACTOR / Galaxy.SECTOR_SIZE);
        int z = Convert.ToInt32(shipPos.Z / Galaxy.EXPAND_FACTOR / Galaxy.SECTOR_SIZE);


        int range = (SectorCount - 1) / 2;
        int minX = x - range;
        int minY = y - range;
        int minZ = z - range;
        int maxX = x + range;
        int maxY = y + range;
        int maxZ = z + range;

        double secMult = Galaxy.EXPAND_FACTOR * Galaxy.SECTOR_SIZE;

        double minDistance = double.MaxValue;
        ClientSector closestSector = null;

        for (z = minZ; z <= maxZ; z++)
        {
            int zHash = z * Galaxy.SECTOR_SIZE * Galaxy.SECTOR_SIZE;
            for (y = minY; y <= maxY; y++)
            {
                int yHash = y * Galaxy.SECTOR_SIZE;
                for (x = minX; x <= maxX; x++)
                {
                    int hash = x + yHash + zHash;
                    XnaGeometry.Vector3 sectorPos = new XnaGeometry.Vector3(x * secMult, y * secMult, z * secMult);
                    double distance = XnaGeometry.Vector3.Distance(shipPos, sectorPos);
                    //check what sector to remove, we remove only 1 per update
                    if (LoadedSectors.ContainsKey(hash))
                    {
                                             
                        if (distance > distanceThreshold && SectorToRemove == null)
                        {                            
                            LoadedSectors.TryGetValue(hash, out SectorToRemove);
                            SectorToRemove.Dispose();
                            LoadedSectors.Remove(hash);
                          //  UnityEngine.Debug.Log("Removing chunk");
                                              
                        } else if (distance < minDistance)
                        {
                            minDistance = distance;
                            LoadedSectors.TryGetValue(hash, out closestSector);
                        }
                        
                    }
                    else if (distance < distanceThreshold)
                    {
                        if (SectorToRemove != null)
                        {
                          //  UnityEngine.Debug.Log("Generating new chunk");
                            Sector s = new Sector(new SectorCoord(x, y, z));
                            s.GenerateSystems(1);
                            SectorToRemove.Activate(s);
                            LoadedSectors.Add(SectorToRemove.Hash,SectorToRemove);
                            SectorToRemove = null;
                         //   UnityEngine.Debug.Log("ChunkGenerated");
                            return;
                        } else
                        {
                            foreach (ClientSector cs in LoadedSectors.Values)
                            {
                                if (!cs.Active)
                                {
                                    SectorToRemove = cs;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (closestSector != null)
        {
            SolarSystem s;
            double distance = GetClosestSystem(closestSector.Sector, shipPos,out s);
            if (distance < Simulator.WARP_DISTANCE_THRESHOLD)
            {
                DropOutOfWarp(s);
            }
        }


   
       

    }//updatesectors


    public static double GetClosestSystem(Sector sector, XnaGeometry.Vector3 pos, out SolarSystem closeSystem)
    {
        double minDistance = double.MaxValue;
        closeSystem = sector.Systems[0];
        foreach (SolarSystem s in sector.Systems)
        {
            double distance = XnaGeometry.Vector3.Distance(pos, s.Pos * Galaxy.EXPAND_FACTOR);
            if (distance < minDistance)
            {
                minDistance = distance;
                closeSystem = s;
            }
        }
        return minDistance;
    }


    public void DropOutOfWarp(SolarSystem system)
    {
        DropOutOfWarpMessage msg = new DropOutOfWarpMessage();
        msg.SystemIndex = system.Index;
        msg.SystemKey = system.Key();
        msg.SectorCoord = system.ParentSector.Coord;
        NetworkManager.Send(msg);
        //tell the server we want to drop out of warp
    }


   
}
