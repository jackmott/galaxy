﻿using UnityEngine;
using System.Collections.Generic;
using GalaxyShared;

public class ClientSector  {

    public ParticleSystem ParticleSystem;
    public SectorCoord Coord = new SectorCoord();
    public bool Active = false;
    public int Hash;
    public GalaxySector Sector;

    public float UnityX, UnityY, UnityZ;

    public void Activate(GalaxySector galaxySector)
    {
        Debug.Log("Activate");
        Sector = galaxySector;
        int x = galaxySector.Coord.X;
        int y = galaxySector.Coord.Y;
        int z = galaxySector.Coord.Z;

        Hash = x + y * GalaxySector.SECTOR_SIZE + z * GalaxySector.SECTOR_SIZE * GalaxySector.SECTOR_SIZE;
        Coord.X = x;
        Coord.Y = y;
        Coord.Z = z;
        Active = true;


        UnityX = x * GalaxySector.SECTOR_SIZE * Warp.ExpandFactor;
        UnityY = y * GalaxySector.SECTOR_SIZE * Warp.ExpandFactor;
        UnityZ = z * GalaxySector.SECTOR_SIZE * Warp.ExpandFactor;


        ParticleSystem.Particle[] particles = GenStars();
        ParticleSystem.SetParticles(particles,particles.Length);
       
        
        
    }

    public SolarSystem GetClosestSystem()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        float minDistance = float.MaxValue;
        SolarSystem closestSystem = null;
        foreach (SolarSystem system in Sector.Systems)
        {
            Vector3 systemPos = (Vector3)system.ClientCoord;
            float distance = Vector3.Distance(cameraPos, systemPos);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestSystem = system;
            }

        }
        closestSystem.ClientDistance = minDistance;
        return closestSystem;
    }

    public ParticleSystem.Particle[] GenStars()
    {
        Debug.Log("GenSTars");
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[Sector.Systems.Count];
        int i = 0;
        Debug.Log(Sector.Systems.Count);
        foreach (SolarSystem system in Sector.Systems)
        {

            Vector3 clientPos = new Vector3(system.Coord.X * Warp.ExpandFactor + UnityX, system.Coord.Y * Warp.ExpandFactor + UnityY, system.Coord.Z * Warp.ExpandFactor + UnityZ);
            system.ClientCoord = clientPos;
            particles[i].position = clientPos;
            particles[i].size = system.Star.Size / 5f;
            particles[i].color = new Color(system.Star.Color.R / 255f, system.Star.Color.G / 255f, system.Star.Color.B / 255f);      
            i++;
        }



        return particles;
    }


    public void Dispose()
    {
        Active = false;
        //particleSystem.Stop();
        ParticleSystem.Clear();
        
    }
    	
    
}
