using UnityEngine;
using System.Collections.Generic;
using GalaxyShared;

public class ClientSector  {

    public ParticleSystem particleSystem;
    public SectorCoord coord = new SectorCoord();
    public bool active = false;
    public int hash;
    public GalaxySector sector;

    public float unityX, unityY, unityZ;

    public void Activate(GalaxySector galaxySector)
    {
        sector = galaxySector;
        int x = galaxySector.coord.x;
        int y = galaxySector.coord.y;
        int z = galaxySector.coord.z;

        hash = x + y * GalaxySector.SECTOR_SIZE + z * GalaxySector.SECTOR_SIZE * GalaxySector.SECTOR_SIZE;
        coord.x = x;
        coord.y = y;
        coord.z = z;
        active = true;


        unityX = x * GalaxySector.SECTOR_SIZE * Warp.expandFactor;
        unityY = y * GalaxySector.SECTOR_SIZE * Warp.expandFactor;
        unityZ = z * GalaxySector.SECTOR_SIZE * Warp.expandFactor;


        ParticleSystem.Particle[] particles = GenStars();
        particleSystem.SetParticles(particles,particles.Length);
       
        
        
    }

    public SolarSystem GetClosestSystem()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        float minDistance = float.MaxValue;
        SolarSystem closestSystem = null;
        foreach (SolarSystem system in sector.systems)
        {
            Vector3 systemPos = (Vector3)system.clientCoord;
            float distance = Vector3.Distance(cameraPos, systemPos);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestSystem = system;
            }

        }
        closestSystem.clientDistance = minDistance;
        return closestSystem;
    }

    public ParticleSystem.Particle[] GenStars()
    {


        

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[sector.systems.Count];
        int i = 0;
        foreach (SolarSystem system in sector.systems)
        {

            Vector3 clientPos = new Vector3(system.coord.x * Warp.expandFactor + unityX, system.coord.y * Warp.expandFactor + unityY, system.coord.z * Warp.expandFactor + unityZ);
            system.clientCoord = clientPos;
            particles[i].position = clientPos;
            particles[i].size = system.star.size / 5f;

            switch (system.star.type)
            {
                case Star.O:
                    particles[i].color = new Color(149 / 255f, 71 / 255f, 254 / 255f);
                    break;
                case Star.B:
                    particles[i].color = new Color(123 / 255f, 109 / 255f, 252 / 255f);
                    break;
                case Star.A:
                    particles[i].color = new Color(186 / 255f, 179 / 255f, 253 / 255f);
                    break;
                case Star.F:
                    particles[i].color = new Color(255 / 255f, 255 / 255f, 255 / 255f);
                    break;
                case Star.G:
                    particles[i].color = new Color(255 / 255f, 247 / 255f, 85 / 255f);
                    break;
                case Star.K:
                    particles[i].color = new Color(240 / 255f, 96 / 255f, 0 / 255f);
                    break;
                case Star.M:
                    particles[i].color = new Color(250 / 255f, 18 / 255f, 5 / 255f);
                    break;
            }
            i++;
        }



        return particles;
    }


    public void Dispose()
    {
        active = false;
        //particleSystem.Stop();
        particleSystem.SetParticles(new ParticleSystem.Particle[0], 0);
    }
    	
    
}
