using UnityEngine;
using System.Collections.Generic;
using System;
using GalaxyShared;




[AddComponentMenu("Camera/Warp ")]
public class Warp : MonoBehaviour {
    
    
    int SectorCount = 9; //must be odd
    ClientSector SectorToRemove = null;

    public static ClientSector ClosestSector = null;
    public GameObject Ship;
        
    Dictionary<int,ClientSector> LoadedSectors;

    double distanceThreshold; 


    // Use this for initialization
    void Start () {

       
        // Thread thread = new Thread(new ThreadStart(NetworkLoop));
        // thread.Start();

        GameObject OriginalParticlePrefab = Resources.Load<GameObject>("StarParticles");


        Camera.main.farClipPlane = (float)(Sector.SECTOR_SIZE * Sector.EXPAND_FACTOR * (SectorCount / 2f));
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

        

        XnaGeometry.Vector3 shipPos = NetworkManager.PlayerState.Location.Pos;

        int x = Convert.ToInt32(shipPos.X / Sector.EXPAND_FACTOR / Sector.SECTOR_SIZE);
        int y = Convert.ToInt32(shipPos.Y / Sector.EXPAND_FACTOR / Sector.SECTOR_SIZE);
        int z = Convert.ToInt32(shipPos.Z / Sector.EXPAND_FACTOR / Sector.SECTOR_SIZE);


        int range = (SectorCount - 1) / 2;
        int minX = x - range;
        int minY = y - range;
        int minZ = z - range;
        int maxX = x + range;
        int maxY = y + range;
        int maxZ = z + range;

        double secMult = Sector.EXPAND_FACTOR * Sector.SECTOR_SIZE;

        double minDistance = double.MaxValue;
        ClientSector closestSector = null;

        for (z = minZ; z <= maxZ; z++)
        {
            int zHash = z * Sector.SECTOR_SIZE * Sector.SECTOR_SIZE;
            for (y = minY; y <= maxY; y++)
            {
                int yHash = y * Sector.SECTOR_SIZE;
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
            double distance = XnaGeometry.Vector3.Distance(pos, s.Pos * Sector.EXPAND_FACTOR);
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
        NetworkManager.Send(msg);
        //tell the server we want to drop out of warp
    }

   
}
