using UnityEngine;
using System.Collections.Generic;
using System;
using GalaxyShared;
using System.Diagnostics;



[AddComponentMenu("Camera/Warp ")]
public class Warp : MonoBehaviour
{
    
  
    const int SectorCount = 7; //must be odd 
    const float distanceThreshold = (float)(Galaxy.SECTOR_SIZE * Galaxy.EXPAND_FACTOR * (SectorCount / 2f));
    //const float HalfSectorCount = SectorCount / 2f;
    //credit to Pythagoras
    
    public GameObject Ship;
    private Vector3 ShipOffset;
    ClientSector[] sectors = new ClientSector[SectorCount * SectorCount * SectorCount];

    int oldX, oldY, oldZ;
    

    // Use this for initialization
    void Start()
    {


        Galaxy.Init();
        GameObject OriginalParticlePrefab = Resources.Load<GameObject>("StarParticles");

        Ship.transform.rotation = Utility.UQuaternion(NetworkManager.PlayerState.Rotation);
        Ship.transform.position = Vector3.zero;
        ShipOffset = Utility.UVector(NetworkManager.PlayerState.Location.Pos);
        Ship.transform.Translate(Vector3.forward * .2f);

        GalaxyGlow();

        //warm up clientsectors
        XnaGeometry.Vector3 shipPos = NetworkManager.PlayerState.Location.Pos / Galaxy.EXPAND_FACTOR / Galaxy.SECTOR_SIZE;
        int x = (int)shipPos.X;
        int y = (int)shipPos.Y;
        int z = (int)shipPos.Z;
        oldX = x;
        oldY = y;
        oldZ = z;

        int range = (SectorCount - 1) / 2;
        int minX = x - range;
        int minY = y - range;
        int minZ = z - range;


        int count = 0;
        Vector3 offset = new Vector3(x, y, z) * Galaxy.EXPAND_FACTOR * Galaxy.SECTOR_SIZE;
        for (x = 0; x < SectorCount; x++)
        {
            for (y = 0; y < SectorCount; y++)
            {
                for (z = 0; z < SectorCount; z++)
                {

                    Sector s = new Sector(new SectorCoord(minX + x, minY + y, minZ + z));
                    //     s.GenerateSystems();
                    Vector3 sectorPos = new Vector3(s.Coord.X  * Galaxy.SECTOR_SIZE * Galaxy.EXPAND_FACTOR, 
                                                    s.Coord.Y  * Galaxy.SECTOR_SIZE * Galaxy.EXPAND_FACTOR, 
                                                    s.Coord.Z  * Galaxy.SECTOR_SIZE * Galaxy.EXPAND_FACTOR) - ShipOffset;
                    GameObject go = (GameObject)Instantiate(OriginalParticlePrefab, sectorPos, Quaternion.identity);
                    ParticleSystem p = go.GetComponent<ParticleSystem>();
                    ClientSector c = new ClientSector(s, p,go);
                    c.ParticleSystem = p;
                    sectors[count] = c;
                    count++;
                }
            }
        }

        Camera.main.farClipPlane = distanceThreshold;
        Camera.main.nearClipPlane = 0.5f;


    }

    public Vector3 RandomVector(FastRandom r)
    {
        Vector3 v = new Vector3(r.NextGaussianFloat(), r.NextGaussianFloat(), r.NextGaussianFloat());
        v.Normalize();
        return v;
    }

    public void GalaxyGlow()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        FastRandom rand = new FastRandom(1, 2, 3);
        int POINT_COUNT = 25000;
        SectorCoord s = NetworkManager.PlayerState.Location.SectorCoord;
        Vector3 sector = new Vector3(s.X, s.Y, s.Z);
        Vector3 player = Utility.UVector(NetworkManager.PlayerState.Location.Pos);
        List<ParticleSystem.Particle> particleList = new List<ParticleSystem.Particle>(POINT_COUNT / 4);
        for (int i = 0; i < POINT_COUNT; i++)
        {
            Vector3 unit = RandomVector(rand);
            Vector3 galaxyRay = unit * Galaxy.GALAXY_SIZE_SECTORS;
            float r = 0;
            float g = 0;
            float b = 0;
            int count = 0;
            for (float pct = .2f; pct <= 1; pct = pct + .025f)
            {
                count++;
                Vector3 pos = sector + galaxyRay * pct;
                if (pos.x < 0 || pos.y < 0 || pos.z < 0 || pos.x >= Galaxy.GALAXY_SIZE_SECTORS || pos.y >= Galaxy.GALAXY_SIZE_SECTORS || pos.z >= Galaxy.GALAXY_THICKNESS_SECTORS)
                {
                    break;
                }

                System.Drawing.Color c = Galaxy.GetColorAt(Convert.ToInt32(pos.x), Convert.ToInt32(pos.y), Convert.ToInt32(pos.z));
                r = r + c.R;
                g = g + c.G;
                b = b + c.B;

            }

            if (r + g + b > .01f)
            {

                float scale = 5000f;
                ParticleSystem.Particle p = new ParticleSystem.Particle();
                p.startColor = new Color(r / scale, g / scale, b / scale);
                p.startSize = 100;
                p.position = player + unit * 400;
                particleList.Add(p);
            }

        }

        GameObject glowGO = (GameObject)Instantiate(Resources.Load<GameObject>("GlowParticles"), player, Quaternion.identity);
        ParticleSystem psystem = glowGO.GetComponent<ParticleSystem>();
        psystem.SetParticles(particleList.ToArray(), particleList.Count);
        UnityEngine.Debug.Log(particleList.Count + "particles generated for glow");
        Camera.main.GetComponent<Skybox>().material.SetTexture("_Tex", GetCubemap(player, 2048));
        Destroy(glowGO);
        UnityEngine.Debug.Log("GenerateGlow:" + stopwatch.ElapsedMilliseconds);
    }


    void Update()
    {
        

        XnaGeometry.Vector3 shipPos = NetworkManager.PlayerState.Location.Pos / Galaxy.EXPAND_FACTOR / Galaxy.SECTOR_SIZE;
        
        int xDiff = (int)(shipPos.X) - oldX;
        int yDiff = (int)(shipPos.Y) - oldY;
        int zDiff = (int)(shipPos.Z) - oldZ;

        if (xDiff != 0 || yDiff != 0 || zDiff != 0)
        {
            ShipOffset = NetworkManager.ShipPos;
            //All sectors with these values are replaced
            int xToReplace = oldX - (SectorCount - 1)/2 * xDiff;
            int yToReplace = oldY - (SectorCount - 1)/2 * yDiff;
            int zToReplace = oldZ - (SectorCount - 1)/2 * zDiff;

            //With sectors that have this value
            int newX = xToReplace + SectorCount * xDiff;
            int newY = yToReplace + SectorCount * yDiff;
            int newZ = zToReplace + SectorCount * zDiff;
           
            for (int i = 0; i < sectors.Length; i++)
            {
                ClientSector cs = sectors[i];
                Vector3 originResetTranslation = new Vector3(xDiff * Galaxy.SECTOR_SIZE, yDiff * Galaxy.SECTOR_SIZE, zDiff * Galaxy.SECTOR_SIZE);
                cs.GameObject.transform.Translate(originResetTranslation);


                SectorCoord sc = cs.Sector.Coord;
                bool update = false;
                
                if (sc.X == xToReplace && xDiff != 0)
                {
                    sc.X = newX;
                    update = true;
                }

                if (sc.Y == yToReplace && yDiff != 0)
                {
                    sc.Y = newY;
                    update = true;
                }
                if (sc.Z == zToReplace && zDiff != 0)
                {
                    sc.Z = newZ;
                    update = true;
                }
                Sector s;
                if (update)
                {
                    s = new Sector(sc);                  
                    cs.Activate(s);
                }

                s = cs.Sector;
                Vector3 sectorPos = new Vector3( s.Coord.X * Galaxy.SECTOR_SIZE * Galaxy.EXPAND_FACTOR,
                                                 s.Coord.Y * Galaxy.SECTOR_SIZE * Galaxy.EXPAND_FACTOR,
                                                 s.Coord.Z * Galaxy.SECTOR_SIZE * Galaxy.EXPAND_FACTOR) - ShipOffset;
                cs.ParticleSystem.transform.position = sectorPos;
            }
            oldX += xDiff;
            oldY += yDiff;
            oldZ += zDiff;
        }
        Ship.transform.position = NetworkManager.ShipPos - ShipOffset;
        Camera.main.transform.position = Ship.transform.position;// Utility.UVector(NetworkManager.PlayerState.Location.Pos);
        Camera.main.transform.rotation = Ship.transform.rotation;// Utility.UQuaternion(NetworkManager.PlayerState.Rotation);

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

    Cubemap GetCubemap(Vector3 player, int size)
    {
        GameObject camLO = (GameObject)Instantiate(Resources.Load<GameObject>("GlowCamera"), player, Quaternion.identity);
        Camera camL = camLO.GetComponent<Camera>();
        GameObject camRO = (GameObject)Instantiate(Resources.Load<GameObject>("GlowCamera"), player, Quaternion.identity);
        Camera camR = camRO.GetComponent<Camera>();
        GameObject camUO = (GameObject)Instantiate(Resources.Load<GameObject>("GlowCamera"), player, Quaternion.identity);
        Camera camU = camUO.GetComponent<Camera>();
        GameObject camDO = (GameObject)Instantiate(Resources.Load<GameObject>("GlowCamera"), player, Quaternion.identity);
        Camera camD = camDO.GetComponent<Camera>();
        GameObject camFO = (GameObject)Instantiate(Resources.Load<GameObject>("GlowCamera"), player, Quaternion.identity);
        Camera camF = camFO.GetComponent<Camera>();
        GameObject camBO = (GameObject)Instantiate(Resources.Load<GameObject>("GlowCamera"), player, Quaternion.identity);
        Camera camB = camBO.GetComponent<Camera>();


        Cubemap skybox = new Cubemap(size, TextureFormat.ARGB32, false);

        camL.transform.LookAt(player + Vector3.left);
        camL.RenderToCubemap(skybox, 1 << (int)CubemapFace.NegativeX);

        camR.transform.LookAt(player + Vector3.right);
        camR.RenderToCubemap(skybox, 1 << (int)CubemapFace.PositiveX);

        camU.transform.LookAt(player + Vector3.up);
        camU.RenderToCubemap(skybox, 1 << (int)CubemapFace.PositiveY);

        camD.transform.LookAt(player + Vector3.down);
        camD.RenderToCubemap(skybox, 1 << (int)CubemapFace.NegativeY);

        camF.transform.LookAt(player + Vector3.forward);
        camF.RenderToCubemap(skybox, 1 << (int)CubemapFace.PositiveZ);

        camB.transform.LookAt(player + Vector3.back);
        camB.RenderToCubemap(skybox, 1 << (int)CubemapFace.NegativeZ);

        Destroy(camLO);
        Destroy(camRO);
        Destroy(camUO);
        Destroy(camDO);
        Destroy(camFO);
        Destroy(camBO);

        return skybox;

    }


    public void DropOutOfWarp(SolarSystem system)
    {

        ClientSolarSystem.Cubemap = GetCubemap(Utility.UVector(NetworkManager.PlayerState.Location.Pos), 2048);

        DropOutOfWarpMessage msg = new DropOutOfWarpMessage();
        msg.SystemIndex = system.Index;
        msg.SystemKey = system.Key();
        msg.SectorCoord = system.ParentSector.Coord;
        NetworkManager.Send(msg);
        //tell the server we want to drop out of warp
    }



}
