using UnityEngine;
using System.Collections.Generic;
using Tuple;
using GalaxyShared;
public class ClientStar : MonoBehaviour, IHasInfo {

    Star Star;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
