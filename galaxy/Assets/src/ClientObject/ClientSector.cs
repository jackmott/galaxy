using UnityEngine;
using GalaxyShared;

public class ClientSector  {

    public ParticleSystem ParticleSystem;
    public SectorCoord Coord = new SectorCoord();
    public bool Active = false;
    public int Hash;
    public Sector Sector;

    public double UnityX, UnityY, UnityZ;

    public void Activate(Sector sector)
    {
        
        Sector = sector;
        int x = Sector.Coord.X;
        int y = Sector.Coord.Y;
        int z = Sector.Coord.Z;

        Hash = x + y * Galaxy.SECTOR_SIZE + z * Galaxy.SECTOR_SIZE * Galaxy.SECTOR_SIZE;
        Coord = new SectorCoord(x, y, z);
        
        Active = true;

        UnityX = x * Galaxy.SECTOR_SIZE * Galaxy.EXPAND_FACTOR;
        UnityY = y * Galaxy.SECTOR_SIZE * Galaxy.EXPAND_FACTOR;
        UnityZ = z * Galaxy.SECTOR_SIZE * Galaxy.EXPAND_FACTOR;


        ParticleSystem.Particle[] particles = GenStars();
        ParticleSystem.SetParticles(particles,particles.Length);                                               
        
    }

    

    public ParticleSystem.Particle[] GenStars()
    {

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[Sector.Systems.Count];
        int i = 0;
        
        foreach (SolarSystem system in Sector.Systems)
        {
            particles[i].position = Utility.UVector(system.Pos*Galaxy.EXPAND_FACTOR);
            particles[i].startSize = system.Star.Size / 37.5f;
            particles[i].startColor = new Color(system.Star.Color.R / 255f, system.Star.Color.G / 255f, system.Star.Color.B / 255f);                  
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
