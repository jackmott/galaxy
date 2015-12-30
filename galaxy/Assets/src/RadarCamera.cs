using UnityEngine;
using System.Collections;

public class RadarCamera : MonoBehaviour {

    

	
    

    // Update is called once per frame
    void Update () {
        transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
        transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
	}
}
