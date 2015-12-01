using UnityEngine;
using Rewired;

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
        Pitch = PlayerInput.GetAxis("Pitch");
        Yaw = PlayerInput.GetAxis("Yaw");

        PrimaryButton = PlayerInput.GetButton("PrimaryButton");
        SecondaryButton = PlayerInput.GetButtonUp("SecondaryButton");

        Inventory = PlayerInput.GetButtonUp("Inventory");

        Build = PlayerInput.GetButtonUp("Build");
        JumpToWarp = PlayerInput.GetButtonUp("Jump");



    }
}
