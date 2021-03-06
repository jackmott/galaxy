﻿using UnityEngine;
using System.Collections.Generic;
using Tuple;
using GalaxyShared;
public class ClientStar : MonoBehaviour, IHasInfo {

    Star Star;
	// Use this for initialization
	void Start () {

            

    }

    
    

    public Texture2D GenerateColorGradient()
    {
        Color primeColor = new Color(Star.Color.r,Star.Color.g,Star.Color.b);
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
