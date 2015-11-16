using UnityEngine;
using System.Collections;
using GalaxyShared;

public class RadarPlayer : MonoBehaviour {

    GameObject Ship;
	// Use this for initialization
	void Start () {
        Ship = GameObject.FindGameObjectWithTag("Ship");
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(Ship.transform.position.x, 0, Ship.transform.position.z);
	}
}
