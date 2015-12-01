using UnityEngine;
using System.Collections;
using System.Text;
using GalaxyShared;
using UnityEngine.UI;
using Rewired;
using SLS.Widgets.Table;
using Tuple;
using System.Linq;

public class Hud : MonoBehaviour
{
    public InputCollector InputCollector;
    public GameObject Recticle;
    public GameObject ShipMenu;
    public GameObject Inventory;
    public GameObject Info;
    public GameObject BuildMenu;

    
    
    // Use this for initialization
    void Start()
    {
        InputCollector = GameObject.Find("NetworkManager").GetComponent<InputCollector>();
        
        DontDestroyOnLoad(this);
        GenerateShipMenu();
        
    }

    void ActivateBuildMenu()
    {
        Info.SetActive(false);
        Inventory.SetActive(false);
        ShipMenu.SetActive(false);
        BuildMenu.SetActive(true);
    }

    void ActivateShipMenu()
    {
        Info.SetActive(false);
        Inventory.SetActive(false);
        ShipMenu.SetActive(true);
        BuildMenu.SetActive(false);
    }

    void ActivateInventoryMenu()
    {
        Info.SetActive(false);
        Inventory.SetActive(true);
        ShipMenu.SetActive(false);
        BuildMenu.SetActive(false);
    }

    void ActivateInfoMenu()
    {
        Info.SetActive(true);
        Inventory.SetActive(false);
        ShipMenu.SetActive(false);
        BuildMenu.SetActive(false);
    }

    void GenerateBuildMenu()
    {
        Table BuildMenuTable = BuildMenu.GetComponent<Table>();
        BuildMenuTable.reset();
        BuildMenuTable.addTextColumn();
        BuildMenuTable.addTextColumn();
        BuildMenuTable.initialize();

        IEnumerable stationModules = typeof(StationModule).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(StationModule)));
        foreach (System.Type sType in stationModules)
        {
            StationModule sm = (StationModule)System.Activator.CreateInstance(sType);
            sm.SetDataFromJSON();
            Datum d = Datum.Body(sm.Name);
            d.elements.Add(sm.Name);
            if (sm.CanBuild(NetworkManager.PlayerState))
            {
                d.elements.Add(sm.BuildTime.ToString());
            }
            else
            {
                d.elements.Add("Missing materials");
            }
            BuildMenuTable.data.Add(d);
        }

        BuildMenuTable.startRenderEngine();
        ActivateBuildMenu();
    }

    void GenerateShipMenu()
    {
        Table ShipMenuTable = ShipMenu.GetComponent<Table>();
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
        ActivateShipMenu();
    }

    void GenerateInfoMenu(IHasInfo infoObject)
    {
        Info info = infoObject.GetInfo();
        Table InfoTable = Info.GetComponent<Table>();
        InfoTable.reset();

        Column c = InfoTable.addTextColumn();
        c.headerValue = info.Title;
        InfoTable.addTextColumn();
        InfoTable.initialize();


        foreach (Tuple<string, string> item in info.Specs)
        {
            Datum d = Datum.Body(item.Item1);
            d.elements.Add(item.Item1);
            d.elements.Add(item.Item2);
            InfoTable.data.Add(d);
        }
        InfoTable.startRenderEngine();

        ActivateInfoMenu();
    }

    void GenerateInventoryMenu()
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
        ActivateInventoryMenu();
    }

    // Update is called once per frame
    void Update()
    {

        if (InputCollector.Inventory)
        {
            GenerateInventoryMenu();
        }

        if (InputCollector.Build)
        {
            GenerateBuildMenu();
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
                    if (!Info.activeSelf)
                    {
                        IHasInfo i = o.GetComponent<IHasInfo>();
                        if (i != null)
                        {
                            GenerateInfoMenu(i);
                        }
                    }
                    else
                    {
                        GenerateShipMenu();
                    }
                }
               
            }
        }
    }

    public void UpdateInventory()
    {
        GenerateInventoryMenu();
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
