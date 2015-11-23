using UnityEngine;
using System.Collections;
using GalaxyShared;

public class ClientAsteroid : MonoBehaviour, IClickable {

    
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

    public void OnLeftClick()
    {
        Debug.Log("Ateroid Left Click");
    }

    public void OnRightClick()
    {
        Debug.Log("Asteroid Right Click");
       
    }
}
