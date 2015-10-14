using UnityEngine;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System;
using GalaxyShared;

public class SceneStart : MonoBehaviour {
    List<TcpClient> clients;
	// Use this for initialization
	void Start () {



        // Thread thread = new Thread(new ThreadStart(NetworkLoop));
        // thread.Start();


        GenSkybox(0, 0, 0);

        


  

    }

    ushort x = 0;
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            x = (ushort)(x + 1);
            GenSkybox(0, 0, x);
            
        }
    }

    public void GenSkybox(ushort x,ushort y,ushort z)
    {
        RenderSettings.skybox = null;
        GalaxyGen gen = new GalaxyGen();
        GalaxySector sector = new GalaxySector(new GalaxyCoord(x, y, z));
        gen.PopulateSector(sector);

        Stack<GameObject> systems = new Stack<GameObject>();
        foreach (SolarSystem system in sector.systems)
        {
            GameObject go = new GameObject();
            go.transform.position = new Vector3(system.coord.x / 200 - 160, system.coord.y / 200 - 160, system.coord.z / 200 - 160);
            go.transform.LookAt(Camera.main.transform.position, -Vector3.up);
            Sprite s = Resources.Load<Sprite>("star");
            go.AddComponent<SpriteRenderer>();
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = s;
            systems.Push(go);


        }

        Cubemap cubemap = new Cubemap(1024, TextureFormat.ARGB32, false);
        Camera.main.RenderToCubemap(cubemap);

        while (systems.Count > 0)
        {
            Destroy(systems.Pop());
        }


        Shader skyshader = Shader.Find("Skybox/Cubemap");
        Material mat = new Material(skyshader);
        mat.SetTexture("_Tex", cubemap);
        //sb.material = mat;
        
         
         RenderSettings.skybox = mat;
    }

    public void NetworkLoop()
    {
        TcpClient socket = new TcpClient("localhost", 8888);
        NetworkStream stream = socket.GetStream();
        
        byte[] buffer = new byte[1024];
        int pos = 0;
        while (true)
        {
            pos = 0;             
            while (pos < 2)
            {
                int bytesRead = stream.Read(buffer, pos, 2);
                if (bytesRead == 0) { 
                    //cleanup
                }
                pos += bytesRead;
            }

            int size = BitConverter.ToInt16(buffer, 0);



            while (pos < size)
            {
                int bytesRead = stream.Read(buffer, pos, size - 2);
                if (bytesRead == 0) {
                    //cleanup
                }
                pos += bytesRead;
            }
            

        }

    }
}
