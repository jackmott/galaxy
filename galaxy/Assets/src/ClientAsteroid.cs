using UnityEngine;
using System.Collections;
using GalaxyShared;

public class ClientAsteroid : MonoBehaviour {

    public Asteroid Asteroid;

    // Use this for initialization
    void Start()
    {
        GameObject radarPlanet = Instantiate(Resources.Load<GameObject>("RadarAsteroid"));
        radarPlanet.transform.parent = this.transform;
        radarPlanet.transform.localPosition = Vector3.zero;
        radarPlanet.transform.localScale = Vector3.one * Asteroid.Size;
        radarPlanet.layer = LayerMask.NameToLayer("Radar");
    }

    // Update is called once per frame
    void Update()
    {
        
       transform.Rotate(Vector3.up, 100 * Asteroid.RotationRate * Time.deltaTime);        
    }

}
