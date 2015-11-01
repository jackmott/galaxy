using UnityEngine;
using GalaxyShared;

public class ClientSolarSystem : MonoBehaviour
{

    SolarSystem SolarSystem;

    const float EARTH_CONSANT = 100;

    // Use this for initialization
    void Start()
    {
        if (Warp.SystemToLoad != null)
        {
            SolarSystem = Warp.SystemToLoad;
        }
        else
        {
            GalaxyGen gen = new GalaxyGen();
            GalaxySector sector = gen.GetSector(new SectorCoord(0, 0, 0), 1);
            SolarSystem = sector.Systems[0];
        }

        GameObject star = (GameObject)Instantiate(Resources.Load<GameObject>("Star"), Vector3.zero, Quaternion.identity);
        star.transform.position = Vector3.zero;
        star.transform.localScale *= SolarSystem.Star.Size * EARTH_CONSANT * 3;
        GameObject light = (GameObject)Instantiate(Resources.Load<GameObject>("StarLight"), Vector3.zero, Quaternion.identity);
        light.transform.position = Vector3.zero;
        star.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(SolarSystem.Star.Color.R / 255f, SolarSystem.Star.Color.G / 255f, SolarSystem.Star.Color.B / 255f));
        SolarSystem.Generate();
        GameObject resourcePlanet = Resources.Load<GameObject>("Planet");
        foreach (Planet p in SolarSystem.Planets)
        {
            GameObject planetGO = (GameObject)Instantiate(resourcePlanet, Vector3.zero, Quaternion.AngleAxis(p.OrbitAngle, Vector3.up));
            planetGO.transform.Translate(Vector3.forward * (p.Orbit + 1) * EARTH_CONSANT * 50);
            planetGO.transform.localScale *= p.Size * EARTH_CONSANT;
            SetPlanetTexture(p, planetGO, 512, 256);

            LineRenderer lr = planetGO.GetComponent<LineRenderer>();
            Color c = new Color(1, 1, 1);
            lr.material = new Material(Shader.Find("Particles/Additive"));
            lr.SetColors(c, c);
            lr.SetWidth(25f, 25f);
            int vertexCount = 50;
            lr.SetVertexCount(vertexCount+1);

            for (int i = 0; i < vertexCount+1; i++)
            {
                
                float angle = ((float)i / (float)vertexCount) * Mathf.PI * 2f;
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (p.Orbit + 1) * EARTH_CONSANT * 50;
                //pos = new Vector3(pos.x + planetGO.transform.position.x, pos.y + planetGO.transform.position.y, pos.z + planetGO.transform.position.z);
                lr.SetPosition(i, pos);
            }



        }

        if (Warp.Cubemap != null)
        {
            Shader skyshader = Shader.Find("Skybox/Cubemap");
            Material mat = new Material(skyshader);
            mat.SetTexture("_Tex", Warp.Cubemap);
            Camera.main.GetComponent<Skybox>().material = mat;
            Camera.main.transform.rotation = Warp.CameraRotation;
            Camera.main.transform.Translate(Vector3.back * EARTH_CONSANT * 50);
            //RenderSettings.skybox = mat;
        }
        else
        {
            Camera.main.transform.Translate(Vector3.back * EARTH_CONSANT * 50);
        }




    }

    public void SetPlanetTexture(Planet p, GameObject planetGO, int width, int height)
    {
        Texture2D planetTex = new Texture2D(width, height, TextureFormat.ARGB32, false);

        PlanetTextureGenerator pg = new PlanetTextureGenerator(p, width, height);
        pg.generatePlanet();
        ClientPlanet cp = planetGO.GetComponent<ClientPlanet>();
        cp.Planet = p;


        //SetWater(planetInfo.colorRamp.gradient[0]);


        planetTex.SetPixels(pg.GetPlanetColors());
        planetTex.Apply();
        Texture2D normalMap = Resources.Load<Texture2D>("PlanetNormals/" + pg.NormalMap);
        Material material = planetGO.GetComponent<Renderer>().material;
        material.SetTexture("_BumpMap", normalMap);
        material.mainTexture = planetTex;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
