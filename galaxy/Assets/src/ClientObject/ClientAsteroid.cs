using UnityEngine;
using GalaxyShared;
using System.Collections.Generic;
using Tuple;

public class ClientAsteroid : MonoBehaviour, IHasInfo {

    
    private Asteroid Asteroid;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetAsteroid(Asteroid a)
    {
        Asteroid = a;
    }


    public Info GetInfo()
    {
        Info info;
        info.Title = "Asteroid";
        Tuple<string, string> remainingOre = new Tuple<string, string>("Ore", Asteroid.Remaining.ToString());
        info.Specs = new List<Tuple<string, string>>();
        info.Specs.Add(remainingOre);
        info.Actions = new List<Tuple<string, string>>();
        return info;
    }
}
