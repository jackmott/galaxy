﻿using UnityEngine;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using GalaxyShared;




[AddComponentMenu("Camera/Warp ")]
public class Warp : MonoBehaviour {
    
    
    int SectorCount = 9; //must be odd
    
    public static ClientSector ClosestSector = null;

        
    Dictionary<int,ClientSector> LoadedSectors;            
    
    

    // Use this for initialization
    void Start () {


        // Thread thread = new Thread(new ThreadStart(NetworkLoop));
        // thread.Start();
    
        GameObject OriginalParticlePrefab = Resources.Load<GameObject>("StarParticles");


        Camera.main.farClipPlane = (float)(GalaxySector.SECTOR_SIZE * GalaxySector.EXPAND_FACTOR * (SectorCount / 2f));
        Camera.main.transform.rotation = Utility.UQuaternion(NetworkManager.PlayerState.Rotation);
        Camera.main.transform.position = Utility.UVector(NetworkManager.PlayerState.Location.Pos);
        Camera.main.transform.Translate(Vector3.forward * .2f);
       

        //warm up clientsectors
        LoadedSectors = new Dictionary<int, ClientSector>();        
        for (int i = 0; i < SectorCount*SectorCount*SectorCount; i++)
        {
            
            ClientSector c = new ClientSector();
            GameObject go = (GameObject)Instantiate(OriginalParticlePrefab,Vector3.zero, Quaternion.identity);            
            ParticleSystem p = go.GetComponent<ParticleSystem>();                    
            c.ParticleSystem = p;            
            c.Active = false;
            c.Hash = i+GalaxySector.GALAXY_SIZE_LIGHTYEARS+10;
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
        
        Vector3 cameraPos = Camera.main.transform.position;
        
        int x = Convert.ToInt32(cameraPos.x / GalaxySector.EXPAND_FACTOR / GalaxySector.SECTOR_SIZE);
        int y = Convert.ToInt32(cameraPos.y / GalaxySector.EXPAND_FACTOR / GalaxySector.SECTOR_SIZE);
        int z = Convert.ToInt32(cameraPos.z / GalaxySector.EXPAND_FACTOR / GalaxySector.SECTOR_SIZE);


        
        int minX = x - (SectorCount - 1) / 2;        
        int minY = y - (SectorCount - 1) / 2;        
        int minZ = z - (SectorCount - 1) / 2;


        ClosestSector = null;
        float minDistance = float.MaxValue;                
        foreach (ClientSector cSector in LoadedSectors.Values)
        {
            
            x = cSector.Coord.X;
            y = cSector.Coord.Y;
            z = cSector.Coord.Z;

            Vector3 sectorPos = new Vector3((float)(x * GalaxySector.EXPAND_FACTOR * GalaxySector.SECTOR_SIZE),(float)( y * GalaxySector.EXPAND_FACTOR * GalaxySector.SECTOR_SIZE),(float)( z * GalaxySector.EXPAND_FACTOR * GalaxySector.SECTOR_SIZE));

            float distance = Vector3.Distance(cameraPos, sectorPos);

            
            if (distance > distanceThreshold) cSector.Dispose();
            else if (distance < minDistance)
            {
                minDistance = distance;
                ClosestSector = cSector;
            }
        }

        if (ClosestSector != null && ClosestSector.Active)
        {
            SolarSystem system = Simulator.GetClosestSystem(ClosestSector.Sector, NetworkManager.PlayerState.Location.Pos);
            
            if (XnaGeometry.Vector3.Distance(system.Pos*GalaxySector.EXPAND_FACTOR,NetworkManager.PlayerState.Location.Pos) < Simulator.WARP_DISTANCE_THRESHOLD)
            {
                DropOutOfWarp();                
            }
     //       Debug.Log(XnaGeometry.Vector3.Distance(system.Pos * GalaxySector.EXPAND_FACTOR, NetworkManager.PlayerState.Location.Pos));

        }

        for ( x = minX; x < minX + SectorCount; x++)
        {
            for (y = minY; y < minY + SectorCount; y++)
            {
                for (z = minZ; z < minZ + SectorCount; z++)
                {

      //              Debug.Log("build new sectors xyz: (" + x + "," + y + "," + z + ")");
                    int hash = x + y * GalaxySector.SECTOR_SIZE + z * GalaxySector.SECTOR_SIZE * GalaxySector.SECTOR_SIZE;
                    if (!LoadedSectors.ContainsKey(hash))
                    {
                        Vector3 sectorPos = new Vector3((float)(x * GalaxySector.EXPAND_FACTOR * GalaxySector.SECTOR_SIZE),(float)( y * GalaxySector.EXPAND_FACTOR * GalaxySector.SECTOR_SIZE),(float)( z * GalaxySector.EXPAND_FACTOR * GalaxySector.SECTOR_SIZE));
                        float distance = Vector3.Distance(cameraPos, sectorPos);
                        
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
                                GalaxySector gSector = new GalaxySector(new SectorCoord(x, y, z));
                                gSector.GenerateSystems(1);
                                if (gSector != null)
                                {
                                    removeSector.Activate(gSector);
                                } else
                                {
                                    Debug.Log("no more sectors!");
                                }
                                Debug.Log("Sector created at:" + x + "," + y + "," + z);
                                LoadedSectors.Add(removeSector.Hash, removeSector);
                            }
                            
                        } // if distance <= distanceThreshold
                    }// if sector doesn't already exist
                    
                }//z

            }//y

        }//x


    }//updatesectors

   
    public void DropOutOfWarp()
    {
        
        
        DropOutOfWarpMessage msg = new DropOutOfWarpMessage();        
        NetworkManager.Send(msg);
        //tell the server we want to drop out of warp
    }

   
}
