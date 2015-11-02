using UnityEngine;
using System.Collections;

public class RadarPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z);
	}
}
