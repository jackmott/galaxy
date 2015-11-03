using UnityEngine;
using System.Collections;
using GalaxyShared.Networking;
using GalaxyShared.Networking.Messages;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [SerializeField]
    private InputField _userField;
    [SerializeField]
    private InputField _passField;
    [SerializeField]
    private Toggle _newuserToggle;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Login()
    {
        if (_newuserToggle.isOn)
        {

            NewUserMessage msg = new NewUserMessage(_userField.text, _passField.text);            
            NetworkManager.Send(msg);
        }
        else
        {
            LoginMessage msg = new LoginMessage(_userField.text, _passField.text);            
            NetworkManager.Send(msg);
        }

    }
}
