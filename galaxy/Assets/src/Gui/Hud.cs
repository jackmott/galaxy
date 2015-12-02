using UnityEngine;
using System.Collections;
using GalaxyShared;
using Rewired;
using SLS.Widgets.Table;
using Tuple;
using System.Linq;

public class Hud : MonoBehaviour
{
    public InputCollector InputCollector;
    public GameObject Recticle;
    public GameObject ShipMenu;
    public Table MenuTable;
   
    enum Menu { Main, Build, Info, Inventory };

    Menu ActiveMenu;
    
    // Use this for initialization
    void Start()
    {
        InputCollector = GameObject.Find("NetworkManager").GetComponent<InputCollector>();        
        DontDestroyOnLoad(this);
        MenuTable = ShipMenu.GetComponent<Table>();
        GenerateMainMenu();
        
    }

   

    void GenerateBuildMenu()
    {
        ActiveMenu = Menu.Build;
        
        MenuTable.reset();
        MenuTable.addTextColumn();
        MenuTable.addTextColumn();
        MenuTable.initialize();

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
            MenuTable.data.Add(d);
        }

        MenuTable.startRenderEngine();
        
    }

    void GenerateMainMenu()
    {
        ActiveMenu = Menu.Main;        
        MenuTable.reset();
        MenuTable.addTextColumn();
       

        MenuTable.initialize();

        Rewired.Player inputPlayer = ReInput.players.GetPlayer(0);
        IEnumerable actions = ReInput.mapping.ActionsInCategory("System");

        foreach (InputAction action in actions)
        {
            ActionElementMap buttonMap = inputPlayer.controllers.maps.GetFirstButtonMapWithAction(action.id, true);
            Datum d = Datum.Body(action.name);
            d.elements.Add(GenerateMenuItem(buttonMap.elementIdentifierName, action.descriptiveName));
            MenuTable.data.Add(d);
        }
        MenuTable.startRenderEngine();        
        
    }

    void GenerateInfoMenu(IHasInfo infoObject)
    {
        ActiveMenu = Menu.Info;
        Info info = infoObject.GetInfo();        
        MenuTable.reset();

        Column c = MenuTable.addTextColumn();
        c.headerValue = info.Title;
        MenuTable.addTextColumn();
        MenuTable.initialize();

        foreach (Tuple<string, string> item in info.Specs)
        {
            Datum d = Datum.Body(item.Item1);
            d.elements.Add(item.Item1);
            d.elements.Add(item.Item2);
            MenuTable.data.Add(d);
        }
        MenuTable.startRenderEngine();
                
    }

    void GenerateInventoryMenu()
    {
        ActiveMenu = Menu.Inventory;
        /*
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
        ActivateInventoryMenu();*/
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

        
        if (InputCollector.UIVerticle > 0)
        {
            MenuTable.moveSelectionUp();
        }

        if (InputCollector.UIVerticle < 0)
        {
            MenuTable.moveSelectionDown();
        }

        if (InputCollector.UIHorizontal > 0)
        {
            MenuTable.moveSelectionRight();
        }

        if (InputCollector.UIHorizontal < 0)
        {
            MenuTable.moveSelectionLeft();
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
                    if (ActiveMenu != Menu.Inventory)
                    {
                        IHasInfo i = o.GetComponent<IHasInfo>();
                        if (i != null)
                        {
                            GenerateInfoMenu(i);
                        }
                    }
                    else
                    {
                        GenerateMainMenu();
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
