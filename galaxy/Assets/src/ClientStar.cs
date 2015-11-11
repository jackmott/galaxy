using UnityEngine;
using System.Collections;

public class ClientStar : MonoBehaviour, IClickable {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnLeftClick()
    {
        Debug.Log("Left Click");
    }

    public void OnRightClick()
    {
        Debug.Log("Right Click");
    }
}
