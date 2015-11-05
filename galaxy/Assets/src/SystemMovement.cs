using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GalaxyShared.Networking.Messages;
using GalaxyShared;
    

public class SystemMovement : MonoBehaviour
{
    float throttle = 0;
    List<InputMessage> inputs;
    
    float timeSinceSend = 0; //s
    

    void Start()
    {
        inputs = new List<InputMessage>();
        InvokeRepeating("SampleInput", 0, GalaxyClient.TICK_RATE);
    }

    void Update()
    {

      

    }

    private void SampleInput()
    {
        bool anyInput = false;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        //mouse x and y are swapped
        float xDelta = mousePos.x - screenCenter.x;
        float yDelta = mousePos.y - screenCenter.y;

        
        InputMessage input = new InputMessage();


        //rotation
        yDelta = Mathf.Clamp(yDelta, -70, 70);
        xDelta = Mathf.Clamp(xDelta, -70, 70);
        if (System.Math.Abs(xDelta) > 10 || System.Math.Abs(yDelta) > 10)
        {
            Camera.main.transform.Rotate(new Vector3(-yDelta * Time.deltaTime, xDelta  * Time.deltaTime, 0));        
           input.XTurn = xDelta/4000f;
           input.YTurn = -yDelta/4000f;
           anyInput = true;
        }
        else
        {
            input.XTurn = 0;
            input.YTurn = 0;
        }

        //throttle    
        if (Input.GetKey("w"))
        {
            if (throttle < 100)
            {
                throttle = Mathf.Clamp(throttle + 1, 0, 100);
                anyInput = true;
            }

        }
        else if (Input.GetKey("s"))
        {
            if (throttle > 0)
            {
                throttle = Mathf.Clamp(throttle - 1, 0, 100);
                anyInput = true;
            }
        }
        else if (Input.GetKeyDown("space"))
        {
            if (throttle != 0)
            {
                throttle = 0;
                anyInput = true;
            }

        }


        //do a barrel roll
        if (Input.GetKey("q"))
        {
            //Camera.main.transforinput.Rotate(new Vector3(0, 0, -50*Time.deltaTime));
            input.RollTurn = 1;
            anyInput = true;
        }
        else if (Input.GetKey("e"))
        {
            //Camera.main.transforinput.Rotate(new Vector3(0, 0, 50*Time.deltaTime));
            input.RollTurn = -1;
            anyInput = true;
        }

        Camera.main.transform.Translate(Vector3.forward * throttle * 40 *  Time.deltaTime);

        if (anyInput)
        {
            input.Throttle = throttle;
            inputs.Add(input);
        }
        timeSinceSend += Time.deltaTime;

        if (timeSinceSend >= GalaxyClient.TICK_RATE*3)
        {
            if (inputs.Count > 0)
            {
                NetworkManager.SendInputs(inputs);
                inputs = new List<InputMessage>();
            }
            timeSinceSend = 0;
        }
        

    }
}
