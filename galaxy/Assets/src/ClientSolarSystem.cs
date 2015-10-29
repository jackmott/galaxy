using UnityEngine;
using System.Collections;
using GalaxyShared;

public class ClientSolarSystem : MonoBehaviour {

    SolarSystem system;
    float speed = 0;
    // Use this for initialization
    void Start() {
        if (Warp.systemToLoad != null)
        {
            system = Warp.systemToLoad;
        } else
        {
            GalaxyGen gen = new GalaxyGen();
            GalaxySector sector = gen.GetSector(new SectorCoord(0, 0, 0), 1);
            system = sector.systems[0];
        }

        GameObject star = (GameObject)GameObject.Instantiate(Resources.Load<GameObject>("Star"), Vector3.zero, Quaternion.identity);
        star.transform.position = Vector3.zero;
        GameObject light = (GameObject)GameObject.Instantiate(Resources.Load<GameObject>("StarLight"), Vector3.zero, Quaternion.identity);
        light.transform.position = Vector3.zero;

        
        if (Warp.cubemap != null)
        {
            Shader skyshader = Shader.Find("Skybox/Cubemap");
            Material mat = new Material(skyshader);
            mat.SetTexture("_Tex", Warp.cubemap);
            Camera.main.GetComponent<Skybox>().material = mat;
            Camera.main.transform.rotation = Warp.cameraRotation;
            Camera.main.transform.Translate(Vector3.back * 200);
            //RenderSettings.skybox = mat;
        }



    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            speed = speed + .05f;

        }
        else if (Input.GetKeyDown("p"))
        {
            speed = 0f;
        }

        Camera.main.transform.Translate(Vector3.forward * speed);

    }
}
