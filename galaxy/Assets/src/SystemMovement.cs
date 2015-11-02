using UnityEngine;

public class SystemMovement : MonoBehaviour
{
    float Speed = 0;

    void Start()
    {
       
    }

    void Update()
    {
        // Ensure the cursor is always locked when set
        //Cursor.lockState = CursorLockMode.Locked;
        
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        //mouse x and y are swapped
        float yDelta = mousePos.x - screenCenter.x;
        float xDelta = mousePos.y - screenCenter.y;
        xDelta = xDelta / 2f;
        yDelta = yDelta / 2f;

        yDelta = Mathf.Clamp(yDelta, -70, 70);
        xDelta = Mathf.Clamp(xDelta, -70, 70);
        if (System.Math.Abs(xDelta) > 10 || System.Math.Abs(yDelta) > 10)
        {
           Camera.main.transform.Rotate(new Vector3(-xDelta * Time.deltaTime, yDelta  * Time.deltaTime, 0));
        }

        if (Input.GetKey("w"))
        {
            Speed = Speed + 50f;

        }
        else if (Input.GetKey("s"))
        {
            Speed = Speed - 50f;
        }
        else if (Input.GetKeyDown("space"))
        {
            Speed = 0f;
        } else if (Input.GetKey("q"))
        {
            Camera.main.transform.Rotate(new Vector3(0, 0, -50*Time.deltaTime));
        }
        else if (Input.GetKey("e"))
        {
            Camera.main.transform.Rotate(new Vector3(0, 0, 50*Time.deltaTime));
        }

        Camera.main.transform.Translate(Vector3.forward * Speed * Time.deltaTime);


    }
}
