using UnityEngine;
using GalaxyShared;
using UnityEngine.UI;
using System.Diagnostics;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {

    [SerializeField]
    private InputField _userField;
    [SerializeField]
    private InputField _passField;
    [SerializeField]
    private Toggle _newuserToggle;

    // Use this for initialization
    void Start () {
      
        NewUserMessage msg = new NewUserMessage("jack", "mott");
        NetworkManager.Send(msg);
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
            LoginMessage msg; 
            msg.UserName = _userField.text;
            msg.Password = _passField.text;
            NetworkManager.Send(msg);
        }

    }
}
