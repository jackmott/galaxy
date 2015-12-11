using UnityEngine;
using System.Collections.Generic;
using Tuple;
using GalaxyShared;
public class ClientStar : MonoBehaviour, IHasInfo {

    Star Star;
	// Use this for initialization
	void Start () {
        
                                
        GetComponent<Renderer>().material.SetTexture("_ColorGradient", GenerateColorGradient());
        GetComponent<Renderer>().material.SetFloat("_Frequency", 10);
        GetComponent<Renderer>().material.SetFloat("_Lacunarity", 3);
        GetComponent<Renderer>().material.SetFloat("_Persistence", 2);
        

    }

    // Update is called once per frame
    void Update () {
	
	}

    public Texture2D GenerateColorGradient()
    {
        Color primeColor = new Color(Star.Color.R / 255.0f,Star.Color.G/255.0f,Star.Color.B/255.0f);
        Color secondColor = Color.Lerp(Color.white, primeColor, .5f);

        Texture2D tex = new Texture2D(256, 1, TextureFormat.ARGB32, false);
        for (int i = 0; i < tex.width;i++)
        {
            tex.SetPixel(i, 0, Color.Lerp(primeColor, secondColor, (float)i / (float)tex.width));
        }
        tex.Apply();
        return tex;
    }

    public void SetStar(Star star )
    {
        Star = star;
    }

    public Info GetInfo()
    {
        Info info;
        info.Title = "Star";
        
        Tuple<string, string> Size = new Tuple<string, string>("Size", Star.Size.ToString());
        Tuple<string, string> StarType = new Tuple<string, string>("Type", Star.Type.ToString());
        info.Specs = new List<Tuple<string, string>>();
        
        info.Specs.Add(Size);
        info.Specs.Add(StarType);
        info.Actions = new List<Tuple<string, string>>();
        return info;
    }
}
