using UnityEngine;
using System.Collections;
using GalaxyShared;
using Rewired;
using SLS.Widgets.Table;
using Tuple;
using System.Linq;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public InputCollector InputCollector;
    public GameObject Recticle;
    public GameObject ShipMenu;
    public GameObject MenuHeader;

    private Table MenuTable;
    private Text HeaderText;

    IHasInfo lastTargetedThing;

   
    enum Menu { Main, Build, Info, Inventory };

    Menu ActiveMenu;
    
    // Use this for initialization
    void Start()
    {
        InputCollector = GameObject.Find("NetworkManager").GetComponent<InputCollector>();        
        DontDestroyOnLoad(this);
        MenuTable = ShipMenu.GetComponent<Table>();
        HeaderText = MenuHeader.GetComponent<Text>();
        GenerateMainMenu();
        
    }

   

    void GenerateBuildMenu()
    {
        ActiveMenu = Menu.Build;
        HeaderText.text = "Build Menu";
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
        MenuTable.setSelected(0, 0, false);

    }

    void GenerateMainMenu()
    {
        ActiveMenu = Menu.Main;        
        MenuTable.reset();
        MenuTable.addTextColumn();
        HeaderText.text = "Main Menu";

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
        MenuTable.setSelected(0, 0, false);

    }

    void GenerateInfoMenu(IHasInfo infoObject)
    {
        ActiveMenu = Menu.Info;
        HeaderText.text = "Target Info";
        Info info = infoObject.GetInfo();

        MenuTable.reset();
        MenuTable.addTextColumn();
        MenuTable.addTextColumn();
        MenuTable.initialize();

        HeaderText.text = "Target Info: "+info.Title;                
        
        foreach (Tuple<string, string> item in info.Specs)
        {
            Datum d = Datum.Body(item.Item1);
            d.elements.Add(item.Item1);
            d.elements.Add(item.Item2);
            MenuTable.data.Add(d);
        }
        MenuTable.startRenderEngine();
        MenuTable.setSelected(0, 0, false);
                
    }

    void GenerateInventoryMenu()
    {
        ActiveMenu = Menu.Inventory;
        HeaderText.text = "Inventory";
        MenuTable.reset();

        Column c = MenuTable.addTextColumn();
        c.headerValue = "Item";
        c = MenuTable.addTextColumn();
        c.headerValue = "Mass";
        c = MenuTable.addTextColumn();
        c.headerValue = "Volume";
        c = MenuTable.addTextColumn();
        c.headerValue = "Qty";
        
        MenuTable.initialize();

        foreach (Item i in NetworkManager.PlayerState.Ship.Cargo)
        {
            Datum d = Datum.Body(i.Name);
            d.elements.Add(i.Name);
            d.elements.Add(i.Mass.ToString());
            d.elements.Add(i.Volume.ToString());
            d.elements.Add(i.Count.ToString());
            MenuTable.data.Add(d);
        }
        MenuTable.startRenderEngine();        
        MenuTable.setSelected(0, 0, false);
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
            MenuTable.moveSelectionUp(false);
        }

        if (InputCollector.UIVerticle < 0)
        {
            MenuTable.moveSelectionDown(false);
        }

        if (InputCollector.UIHorizontal > 0)
        {
            MenuTable.moveSelectionRight(false);
        }

        if (InputCollector.UIHorizontal < 0)
        {
            MenuTable.moveSelectionLeft(false);
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
                    if (ActiveMenu != Menu.Info)
                    {
                        lastTargetedThing = o.GetComponent<IHasInfo>();

                        if (lastTargetedThing != null)
                        {
                            GenerateInfoMenu(lastTargetedThing);
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
        if (ActiveMenu == Menu.Inventory) GenerateInventoryMenu();
    }

    public void UpdateInfo()
    {
        if (ActiveMenu == Menu.Info) GenerateInfoMenu(lastTargetedThing);

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
