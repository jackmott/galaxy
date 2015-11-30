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

        Hash = x + y * Sector.SECTOR_SIZE + z * Sector.SECTOR_SIZE * Sector.SECTOR_SIZE;
        Coord = new SectorCoord(x, y, z);
        
        Active = true;

        UnityX = x * Sector.SECTOR_SIZE * Sector.EXPAND_FACTOR;
        UnityY = y * Sector.SECTOR_SIZE * Sector.EXPAND_FACTOR;
        UnityZ = z * Sector.SECTOR_SIZE * Sector.EXPAND_FACTOR;


        ParticleSystem.Particle[] particles = GenStars();
        ParticleSystem.SetParticles(particles,particles.Length);                                               
        
    }

    

    public ParticleSystem.Particle[] GenStars()
    {

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[Sector.Systems.Count];
        int i = 0;
        
        foreach (SolarSystem system in Sector.Systems)
        {
            particles[i].position = Utility.UVector(system.Pos*Sector.EXPAND_FACTOR);
            particles[i].size = system.Star.Size / 37.5f;
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
