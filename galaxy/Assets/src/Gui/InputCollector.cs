using UnityEngine;
using Rewired;
using System;

public class InputCollector : MonoBehaviour
{
    Player PlayerInput;

    public float Throttle;
    public float Roll;
    public float Pitch;
    public float Yaw;

    public bool PrimaryButton;
    public bool SecondaryButton;
    public bool Inventory;
    public bool Build;
    public bool JumpToWarp;

    public float UIVerticle;
    public float UIHorizontal;
    public bool UISubmit;
    public bool UICancel;

   


    public bool GetButton(ref bool button)
    {
        if (button)
        {
            button = false;
            return true;
        }
        return false;
    }
   
    public void SetButton(ref bool button, bool value)
    {
        if (!button) button = value;
    }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput = ReInput.players.GetPlayer(0);
        //  PlayerInput.controllers.maps.SetAllMapsEnabled(false);
        //  PlayerInput.controllers.maps.SetMapsEnabled(true, "Warp");

        Throttle = Mathf.Clamp(Throttle + 100 * PlayerInput.GetAxis("Throttle"), -10, 100);
        Roll = PlayerInput.GetAxis("Roll");

        //if user not using mouse then
        //Pitch = PlayerInput.GetAxis("Pitch");        
        //Yaw = PlayerInput.GetAxis("Yaw");
        //else
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);


        float xDelta = mousePos.x - screenCenter.x;
        float yDelta = mousePos.y - screenCenter.y;
        
        //rotation
        yDelta = Mathf.Clamp(yDelta, -70, 70);
        xDelta = Mathf.Clamp(xDelta, -70, 70);
        if (Math.Abs(xDelta) > 10 || Math.Abs(yDelta) > 10)
        {
            //  Camera.main.transform.Rotate(new Vector3(-yDelta * Time.deltaTime, xDelta  * Time.deltaTime, 0));        
            Yaw = xDelta / 4000f;
            Pitch = -yDelta / 4000f;
        }
        else
        {
            Yaw = 0;
            Pitch = 0;
        }


        PrimaryButton = PlayerInput.GetButton("PrimaryButton");
        SecondaryButton = PlayerInput.GetButtonUp("SecondaryButton");

        Inventory = PlayerInput.GetButtonUp("Inventory");

        Build = PlayerInput.GetButtonUp("Build");
        JumpToWarp = PlayerInput.GetButtonUp("Jump");

        UIVerticle = PlayerInput.GetAxis("UIVertical");
        UIHorizontal = PlayerInput.GetAxis("UIHorizontal");
        UISubmit = PlayerInput.GetButtonUp("UISubmit");
        UICancel = PlayerInput.GetButtonUp("UICancel");

        if (PlayerInput.GetButtonUp("Stop"))
        {
            Throttle = 0;
        }


    }
}
