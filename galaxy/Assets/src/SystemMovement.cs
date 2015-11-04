using UnityEngine;
using GalaxyShared.Networking.Messages;
    

public class SystemMovement : MonoBehaviour
{
    float throttle = 0;
    float updateRate = .1f;

    void Start()
    {
       
    }

    void Update()
    {
        

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        //mouse x and y are swapped
        float yDelta = mousePos.x - screenCenter.x;
        float xDelta = mousePos.y - screenCenter.y;
        xDelta = xDelta / 2f;
        yDelta = yDelta / 2f;

        InputMessage input = new InputMessage();


        //rotation
        yDelta = Mathf.Clamp(yDelta, -70, 70);
        xDelta = Mathf.Clamp(xDelta, -70, 70);
        if (System.Math.Abs(xDelta) > 10 || System.Math.Abs(yDelta) > 10)
        {
            //Camera.main.transforinput.Rotate(new Vector3(-xDelta * Time.deltaTime, yDelta  * Time.deltaTime, 0));        
            input.XTurn = xDelta;
            input.YTurn = yDelta;
        }
        else
        {
            input.XTurn = 0;
            input.YTurn = 0;
        }
        
        //throttle    
        if (Input.GetKey("w"))
        {
           throttle = Mathf.Clamp(throttle + 1, 0, 100);
           
        }
        else if (Input.GetKey("s"))
        {
            throttle = Mathf.Clamp(throttle - 1, 0, 100);           
        }
        else if (Input.GetKeyDown("space"))
        {
            throttle = 0;
            
        }


        //do a barrel roll
        if (Input.GetKey("q"))
        {
            //Camera.main.transforinput.Rotate(new Vector3(0, 0, -50*Time.deltaTime));
            input.RollTurn = 1;
        }
        else if (Input.GetKey("e"))
        {
            //Camera.main.transforinput.Rotate(new Vector3(0, 0, 50*Time.deltaTime));
            input.RollTurn = -1;
        }

        //Camera.main.transform.Translate(Vector3.forward * ClientPlayerStateMessage.throttle * 40 *  Time.deltaTime);
        

        
        NetworkManager.SendInput(input);
        



    }
}
