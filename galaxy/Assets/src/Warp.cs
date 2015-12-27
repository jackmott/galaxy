using UnityEngine;
using System.Collections.Generic;
using System;
using GalaxyShared;
using System.Diagnostics;



[AddComponentMenu("Camera/Warp ")]
public class Warp : MonoBehaviour
{
    const int SectorCount = 9; //must be odd

    ClientSector SectorToRemove = null;
    public GameObject Ship;
    Dictionary<int, ClientSector> LoadedSectors;
    double distanceThreshold;


    // Use this for initialization
    void Start()
    {
        Galaxy.Init();
        GameObject OriginalParticlePrefab = Resources.Load<GameObject>("StarParticles");


        Camera.main.farClipPlane = (float)(Galaxy.SECTOR_SIZE * Galaxy.EXPAND_FACTOR * (SectorCount / 2f));
        distanceThreshold = (double)Camera.main.farClipPlane;
        Ship.transform.rotation = Utility.UQuaternion(NetworkManager.PlayerState.Rotation);
        Ship.transform.position = Utility.UVector(NetworkManager.PlayerState.Location.Pos);
        Ship.transform.Translate(Vector3.forward * .2f);


        //warm up clientsectors
        LoadedSectors = new Dictionary<int, ClientSector>();
        for (int i = 0; i < SectorCount * SectorCount * SectorCount; i++)
        {

            ClientSector c = new ClientSector();
            GameObject go = (GameObject)Instantiate(OriginalParticlePrefab, Vector3.zero, Quaternion.identity);
            ParticleSystem p = go.GetComponent<ParticleSystem>();
            c.ParticleSystem = p;
            c.Active = false;
            c.Hash = i + Galaxy.GALAXY_SIZE_LIGHTYEARS + 10;
            LoadedSectors.Add(c.Hash, c);
        }


        GalaxyGlow();

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
        FastRandom rand = new FastRandom(1.0, 2.0, 3.0);
        int POINT_COUNT = 5000;
        Vector3[] points = new Vector3[POINT_COUNT];
        SectorCoord s = NetworkManager.PlayerState.Location.SectorCoord;
        Vector3 sector = new Vector3(s.X, s.Y, s.Z);
        Vector3 player = Utility.UVector(NetworkManager.PlayerState.Location.Pos);
        List<ParticleSystem.Particle> particleList = new List<ParticleSystem.Particle>(10000);
        for (int i = 0; i < points.Length; i++)
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
                // UnityEngine.Debug.Log(pos.x + "," + pos.y + "," + pos.z);
                if (pos.x < 0 || pos.y < 0 || pos.z < 0 || pos.x >= Galaxy.GALAXY_SIZE_SECTORS || pos.y >= Galaxy.GALAXY_SIZE_SECTORS || pos.z >= Galaxy.GALAXY_THICKNESS_SECTORS)
                {
                    // UnityEngine.Debug.Log("break");
                    break;
                }

                System.Drawing.Color c = Galaxy.GetColorAt(Convert.ToInt32(pos.x), Convert.ToInt32(pos.y), Convert.ToInt32(pos.z));
                r = r + c.R;
                g = g + c.G;
                b = b + c.B;

            }

            if (r + g + b > .01f)
            {
               
                float scale = 10000f;
                ParticleSystem.Particle p = new ParticleSystem.Particle();
                p.startColor = new Color(r/scale, g / scale, b / scale);               
                p.startSize = 10;
                p.position = player + unit * 20;
               // UnityEngine.Debug.Log(player);
                particleList.Add(p);
            }
            //  UnityEngine.Debug.Log(p[i].startColor);                      

        }

        GameObject glowGO = (GameObject)Instantiate(Resources.Load<GameObject>("GlowParticles"),Vector3.zero, Quaternion.identity);
        ParticleSystem psystem = glowGO.GetComponent<ParticleSystem>();
        psystem.SetParticles(particleList.ToArray(),particleList.Count);
        UnityEngine.Debug.Log(particleList.Count + "particles generated for glow");
        /*Cubemap skybox = new Cubemap(4096, TextureFormat.ARGB32, false);
        Camera.main.RenderToCubemap(skybox);
        Camera.main.GetComponent<Skybox>().material.SetTexture("_Tex", skybox);
        Destroy(psystem);*/
        UnityEngine.Debug.Log("GenerateGlow:" + stopwatch.ElapsedMilliseconds);
    }


    // Update is called once per frame
    void Update()
    {

        UpdateSectors();

    }

    void UpdateSectors()
    {

        XnaGeometry.Vector3 shipPos = NetworkManager.PlayerState.Location.Pos;

        int x = Convert.ToInt32(shipPos.X / Galaxy.EXPAND_FACTOR / Galaxy.SECTOR_SIZE);
        int y = Convert.ToInt32(shipPos.Y / Galaxy.EXPAND_FACTOR / Galaxy.SECTOR_SIZE);
        int z = Convert.ToInt32(shipPos.Z / Galaxy.EXPAND_FACTOR / Galaxy.SECTOR_SIZE);


        int range = (SectorCount - 1) / 2;
        int minX = x - range;
        int minY = y - range;
        int minZ = z - range;
        int maxX = x + range;
        int maxY = y + range;
        int maxZ = z + range;

        double secMult = Galaxy.EXPAND_FACTOR * Galaxy.SECTOR_SIZE;

        double minDistance = double.MaxValue;
        ClientSector closestSector = null;

        for (z = minZ; z <= maxZ; z++)
        {
            int zHash = z * Galaxy.SECTOR_SIZE * Galaxy.SECTOR_SIZE;
            for (y = minY; y <= maxY; y++)
            {
                int yHash = y * Galaxy.SECTOR_SIZE;
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

                        }
                        else if (distance < minDistance)
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
                            LoadedSectors.Add(SectorToRemove.Hash, SectorToRemove);
                            SectorToRemove = null;
                            //   UnityEngine.Debug.Log("ChunkGenerated");
                            return;
                        }
                        else
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
            double distance = GetClosestSystem(closestSector.Sector, shipPos, out s);
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
            double distance = XnaGeometry.Vector3.Distance(pos, s.Pos * Galaxy.EXPAND_FACTOR);
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
        msg.SystemKey = system.Key();
        msg.SectorCoord = system.ParentSector.Coord;
        NetworkManager.Send(msg);
        //tell the server we want to drop out of warp
    }



}
