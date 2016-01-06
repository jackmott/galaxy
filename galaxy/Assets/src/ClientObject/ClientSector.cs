using UnityEngine;
using GalaxyShared;

public class ClientSector  {

    public ParticleSystem ParticleSystem;        
    public Sector Sector;
    public GameObject GameObject;
    
  
    public ClientSector(Sector sector, ParticleSystem p,GameObject gameObject)
    {
        GameObject = gameObject;
        ParticleSystem = p;
        Activate(sector);
    }

    public void Activate(Sector sector)
    {
        
        Sector = sector;    
        ParticleSystem.Particle[] particles = GenStars();
        ParticleSystem.SetParticles(particles,particles.Length);                                               
        
    }
    
    public ParticleSystem.Particle[] GenStars()
    {
        int starCount = (int)(Sector.StellarDensity * Galaxy.SECTOR_SIZE_CUBED);
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[starCount];                
        for (int i = 0; i < starCount;i++)
        {
            SolarSystem system = Sector.GenerateSystem(i);
            particles[i].position = Utility.UVector(system.Pos*Galaxy.EXPAND_FACTOR);
            particles[i].startSize = system.Star.Size / 37.5f;
            particles[i].startColor = new Color(system.Star.Color.r, system.Star.Color.g, system.Star.Color.b );                              
        }



        return particles;
    }


    public void ClearParticles()
    {             
        ParticleSystem.Clear();        
    }
    	
    
}
