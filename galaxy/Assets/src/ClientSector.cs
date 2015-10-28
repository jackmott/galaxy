using UnityEngine;
using System.Collections.Generic;
using GalaxyShared;

public class ClientSector  {

    public ParticleSystem particleSystem;
    public SectorCoord coord = new SectorCoord();
    public bool active = false;
    public int hash;

    public void Activate(int x,int y, int z,ParticleSystem.Particle[] particles)
    {        
        hash = x + y * GalaxyGen.SECTOR_SIZE + z * GalaxyGen.SECTOR_SIZE * GalaxyGen.SECTOR_SIZE;
        coord.x = x;
        coord.y = y;
        coord.z = z;
        active = true;        
        particleSystem.SetParticles(particles,particles.Length);
        
        
    }

  

    public void Dispose()
    {
        active = false;
        //particleSystem.Stop();
        particleSystem.SetParticles(new ParticleSystem.Particle[0], 0);
    }
    	
    
}
