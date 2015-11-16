using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Speedo : MonoBehaviour {
    Text Text;
    
	// Use this for initialization
	void Start () {

        Text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        Text.text = "T:" + NetworkManager.PlayerState.Throttle + " V:" + NetworkManager.PlayerState.Throttle * NetworkManager.PlayerState.Ship.TopSpeed;

	}
}
