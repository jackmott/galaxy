using UnityEngine;
using System.Collections.Generic;
using System;
using GalaxyShared;




[AddComponentMenu("Camera/Warp ")]
public class Warp : MonoBehaviour {
    
    
    int SectorCount = 9; //must be odd
    
    public static ClientSector ClosestSector = null;
    public GameObject Ship;
        
    Dictionary<int,ClientSector> LoadedSectors;            
    
    

    // Use this for initialization
    void Start () {


        // Thread thread = new Thread(new ThreadStart(NetworkLoop));
        // thread.Start();
    
        GameObject OriginalParticlePrefab = Resources.Load<GameObject>("StarParticles");


        Camera.main.farClipPlane = (float)(Sector.SECTOR_SIZE * Sector.EXPAND_FACTOR * (SectorCount / 2f));
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
            c.Hash = i+Sector.GALAXY_SIZE_LIGHTYEARS+10;
            LoadedSectors.Add(c.Hash, c);
        }
        

    }


	// Update is called once per frame
	void Update () {
        
        UpdateSectors();

    }

    void UpdateSectors()
    {

        float distanceThreshold = Camera.main.farClipPlane;

        XnaGeometry.Vector3 shipPos = NetworkManager.PlayerState.Location.Pos;

        int x = Convert.ToInt32(shipPos.X / Sector.EXPAND_FACTOR / Sector.SECTOR_SIZE);
        int y = Convert.ToInt32(shipPos.Y / Sector.EXPAND_FACTOR / Sector.SECTOR_SIZE);
        int z = Convert.ToInt32(shipPos.Z / Sector.EXPAND_FACTOR / Sector.SECTOR_SIZE);


        
        int minX = x - (SectorCount - 1) / 2;        
        int minY = y - (SectorCount - 1) / 2;        
        int minZ = z - (SectorCount - 1) / 2;


        ClosestSector = null;
        double minDistance = double.MaxValue;                
        foreach (ClientSector cSector in LoadedSectors.Values)
        {
            
            x = cSector.Coord.X;
            y = cSector.Coord.Y;
            z = cSector.Coord.Z;

            XnaGeometry.Vector3 sectorPos = new XnaGeometry.Vector3((float)(x * Sector.EXPAND_FACTOR * Sector.SECTOR_SIZE),(float)( y * Sector.EXPAND_FACTOR * Sector.SECTOR_SIZE),(float)( z * Sector.EXPAND_FACTOR * Sector.SECTOR_SIZE));

            double distance = XnaGeometry.Vector3.Distance(shipPos, sectorPos);

            
            if (distance > distanceThreshold) cSector.Dispose();
            else if (distance < minDistance)
            {
                minDistance = distance;
                ClosestSector = cSector;
            }
        }

        if (ClosestSector != null && ClosestSector.Active)
        {
            SolarSystem system = GetClosestSystem(ClosestSector.Sector, shipPos);
            double distance = XnaGeometry.Vector3.Distance(system.Pos * Sector.EXPAND_FACTOR, shipPos);
            
            if (distance < Simulator.WARP_DISTANCE_THRESHOLD)
            {
               
                DropOutOfWarp(system);
                 
            }
     //       Debug.Log(XnaGeometry.Vector3.Distance(system.Pos * Sector.EXPAND_FACTOR, NetworkManager.PlayerState.Location.Pos));

        }

        for ( x = minX; x < minX + SectorCount; x++)
        {
            for (y = minY; y < minY + SectorCount; y++)
            {
                for (z = minZ; z < minZ + SectorCount; z++)
                {

      //              Debug.Log("build new sectors xyz: (" + x + "," + y + "," + z + ")");
                    int hash = x + y * Sector.SECTOR_SIZE + z * Sector.SECTOR_SIZE * Sector.SECTOR_SIZE;
                    if (!LoadedSectors.ContainsKey(hash))
                    {
                        XnaGeometry.Vector3 sectorPos = new XnaGeometry.Vector3((float)(x * Sector.EXPAND_FACTOR * Sector.SECTOR_SIZE),(float)( y * Sector.EXPAND_FACTOR * Sector.SECTOR_SIZE),(float)( z * Sector.EXPAND_FACTOR * Sector.SECTOR_SIZE));
                        double distance = XnaGeometry.Vector3.Distance(shipPos, sectorPos);
                        
                        if (distance <= distanceThreshold)
                        {

                            ClientSector removeSector = null;
                            foreach (ClientSector sector in LoadedSectors.Values)
                            {
                                if (!sector.Active)
                                {
                                    removeSector = sector;
                                    break;
                                } //if sector active
                            } // foreach sector

                            if (removeSector != null)
                            {
                                LoadedSectors.Remove(removeSector.Hash);
                                Sector gSector = new Sector(new SectorCoord(x, y, z));
                                UnityEngine.Debug.Log("SECTOR GEN START:"+gSector.Coord.X+","+gSector.Coord.Y+","+gSector.Coord.Z);
                                gSector.GenerateSystems(1);
                                if (gSector != null)
                                {
                                    removeSector.Activate(gSector);
                                } else
                                {
                                    Debug.Log("no more sectors!");
                                }
                                
                                LoadedSectors.Add(removeSector.Hash, removeSector);
                            }
                            
                        } // if distance <= distanceThreshold
                    }// if sector doesn't already exist
                    
                }//z

            }//y

        }//x


    }//updatesectors


    public static SolarSystem GetClosestSystem(Sector sector, XnaGeometry.Vector3 pos)
    {
        double minDistance = double.MaxValue;
        SolarSystem closeSystem = null;
        foreach (SolarSystem s in sector.Systems)
        {
            double distance = XnaGeometry.Vector3.Distance(pos, s.Pos * Sector.EXPAND_FACTOR);
            if (distance < minDistance)
            {
                minDistance = distance;
                closeSystem = s;
            }
        }
        return closeSystem;
    }


    public void DropOutOfWarp(SolarSystem system)
    {


        DropOutOfWarpMessage msg = new DropOutOfWarpMessage();
        msg.SystemIndex = system.Index;
        NetworkManager.Send(msg);
        //tell the server we want to drop out of warp
    }

   
}
