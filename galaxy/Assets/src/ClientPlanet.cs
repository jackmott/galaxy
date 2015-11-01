using UnityEngine;
using System.Collections;
using GalaxyShared;

public class ClientPlanet : MonoBehaviour {

    public Planet Planet;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Planet != null)
        {
           transform.Rotate(Vector3.up, 100* Planet.RotationRate * Time.deltaTime);
        }
	
	}

    
}
