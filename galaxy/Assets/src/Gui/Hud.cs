using UnityEngine;
using System.Collections;
using System.Text;
using GalaxyShared;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public InputCollector InputCollector;
    public GameObject Recticle;
    public GameObject ShipMenu;
    public GameObject Inventory;

    // Use this for initialization
    void Start()
    {
        InputCollector = GameObject.Find("NetworkManager").GetComponent<InputCollector>();
     
        DontDestroyOnLoad(this);
    }


    // Update is called once per frame
    void Update()
    {

        if (InputCollector.Inventory)
        {

            ShipMenu.SetActive(!ShipMenu.activeSelf);
            Inventory.SetActive(!Inventory.activeSelf);

            if (Inventory.activeSelf)
            {
                UpdateInventory();
            }
        }


        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        Recticle.SetActive(false);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            GameObject o = hit.collider.gameObject;
            if (o.tag == "Planet" || o.tag == "Asteroid" || o.tag == "Star")
            {
                Vector3 ScreenPos = Camera.main.WorldToScreenPoint(o.transform.position);
                Recticle.SetActive(true);
                Recticle.transform.position = new Vector3(ScreenPos.x, ScreenPos.y, 0);
                if (InputCollector.SecondaryButton)
                {
                    IClickable i = o.GetComponent<IClickable>();
                    i.OnRightClick();
                }
                else if (InputCollector.PrimaryButton)

                {
                    IClickable i = o.GetComponent<IClickable>();
                    i.OnLeftClick();
                }
            }
        }
    }

    public void UpdateInventory()
    {
        StringBuilder sb = new StringBuilder(32);
        sb.Append("<color=\"cyan\">Inventory</color>\n");
        foreach (Item i in NetworkManager.PlayerState.Ship.Cargo)
        {
            sb.Append("<color=\"magenta\"><</color><color=\"green\">");
            sb.Append(i.Count);
            sb.Append("</color><color=\"magenta\">></color><color=\"cyan\">");
            sb.Append(i.Name);
            sb.Append("</color>\n");
        }
        Inventory.GetComponent<Text>().text = sb.ToString();
    }
}
