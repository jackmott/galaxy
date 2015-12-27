using UnityEngine;
using System.Collections;

public class GalaxyGlow : MonoBehaviour {

	// Use this for initialization
	void Start () {


    }

    // Update is called once per frame
    void Update () {

        transform.position = Utility.UVector(NetworkManager.PlayerState.Location.Pos);
	}
}
