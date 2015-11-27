using UnityEngine;
using System.Collections;
using System.Text;
using GalaxyShared;
using UnityEngine.UI;
using Rewired;
using SLS.Widgets.Table;

public class Hud : MonoBehaviour
{
    public InputCollector InputCollector;
    public GameObject Recticle;
    public GameObject ShipMenu;
    public GameObject Inventory;

    Table ShipMenuTable;
    
    // Use this for initialization
    void Start()
    {
        InputCollector = GameObject.Find("NetworkManager").GetComponent<InputCollector>();
        ShipMenuTable = ShipMenu.GetComponent<Table>();
        DontDestroyOnLoad(this);
        GenerateShipMenu();
        
    }

    void GenerateShipMenu()
    {
        ShipMenuTable.reset();

        ShipMenuTable.addTextColumn();
       

        ShipMenuTable.initialize();

        Rewired.Player inputPlayer = ReInput.players.GetPlayer(0);
        IEnumerable actions = ReInput.mapping.ActionsInCategory("System");

        foreach (InputAction action in actions)
        {
            ActionElementMap buttonMap = inputPlayer.controllers.maps.GetFirstButtonMapWithAction(action.id, true);
            Datum d = Datum.Body(action.name);
            d.elements.Add(GenerateMenuItem(buttonMap.elementIdentifierName, action.descriptiveName));
            ShipMenuTable.data.Add(d);
        }
        ShipMenuTable.startRenderEngine();
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

    public string TextColor(string text, string color)
    {
        return "<color=\"" + color + "\">" + text + "</color>";
    }

    public string GenerateMenuItem(string key, string description)
    {
        string result = "";
        result += TextColor("<", "magenta");
        result += TextColor(key, "green");
        result += TextColor(">", "magenta");
        result += TextColor(description, "cyan");
        return result;
    }
}
