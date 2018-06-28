using DwarfControl;
using proto.enums.ui_sidebar_mode;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DwarfModeMenu : MonoBehaviour
{
    public ui_sidebar_mode mode = ui_sidebar_mode.Default;

    public Button buttonPrefab;

    public RectTransform menuPanel;

    // Use this for initialization
    void Start()
    {
        BuildDefaultMenu(null);
    }

    // Update is called once per frame
    void Update()
    {
        var sidebar = DFConnection.Instance.SidebarState;
        if (sidebar != null)
            UpdateMenu(sidebar);
    }

    public void SetSidebar(string mode)
    {
        SidebarState sidebar = new SidebarState
        {
            mode = (ui_sidebar_mode)Enum.Parse(typeof(ui_sidebar_mode), mode)
        };
        DFConnection.Instance.EnqueueSidebarSet(sidebar);
    }



    void UpdateMenu(SidebarState sidebar)
    {
        if (mode != sidebar.mode)
        {
            EmptyMenu();
        }
        switch (sidebar.mode)
        {
            case ui_sidebar_mode.Default:
                BuildDefaultMenu(sidebar);
                    break;
            case ui_sidebar_mode.Build:
                BuildBuildMenu(sidebar);
                break;
            case ui_sidebar_mode.Squads:
            case ui_sidebar_mode.DesignateMine:
            case ui_sidebar_mode.DesignateRemoveRamps:
            case ui_sidebar_mode.DesignateUpStair:
            case ui_sidebar_mode.DesignateDownStair:
            case ui_sidebar_mode.DesignateUpDownStair:
            case ui_sidebar_mode.DesignateUpRamp:
            case ui_sidebar_mode.DesignateChannel:
            case ui_sidebar_mode.DesignateGatherPlants:
            case ui_sidebar_mode.DesignateRemoveDesignation:
            case ui_sidebar_mode.DesignateSmooth:
            case ui_sidebar_mode.DesignateCarveTrack:
            case ui_sidebar_mode.DesignateEngrave:
            case ui_sidebar_mode.DesignateCarveFortification:
            case ui_sidebar_mode.Stockpiles:
            case ui_sidebar_mode.QueryBuilding:
            case ui_sidebar_mode.Orders:
            case ui_sidebar_mode.OrdersForbid:
            case ui_sidebar_mode.OrdersRefuse:
            case ui_sidebar_mode.OrdersWorkshop:
            case ui_sidebar_mode.OrdersZone:
            case ui_sidebar_mode.BuildingItems:
            case ui_sidebar_mode.ViewUnits:
            case ui_sidebar_mode.LookAround:
            case ui_sidebar_mode.DesignateItemsClaim:
            case ui_sidebar_mode.DesignateItemsForbid:
            case ui_sidebar_mode.DesignateItemsMelt:
            case ui_sidebar_mode.DesignateItemsUnmelt:
            case ui_sidebar_mode.DesignateItemsDump:
            case ui_sidebar_mode.DesignateItemsUndump:
            case ui_sidebar_mode.DesignateItemsHide:
            case ui_sidebar_mode.DesignateItemsUnhide:
            case ui_sidebar_mode.DesignateChopTrees:
            case ui_sidebar_mode.DesignateToggleEngravings:
            case ui_sidebar_mode.DesignateToggleMarker:
            case ui_sidebar_mode.Hotkeys:
            case ui_sidebar_mode.DesignateTrafficHigh:
            case ui_sidebar_mode.DesignateTrafficNormal:
            case ui_sidebar_mode.DesignateTrafficLow:
            case ui_sidebar_mode.DesignateTrafficRestricted:
            case ui_sidebar_mode.Zones:
            case ui_sidebar_mode.ZonesPenInfo:
            case ui_sidebar_mode.ZonesPitInfo:
            case ui_sidebar_mode.ZonesHospitalInfo:
            case ui_sidebar_mode.ZonesGatherInfo:
            case ui_sidebar_mode.DesignateRemoveConstruction:
            case ui_sidebar_mode.DepotAccess:
            case ui_sidebar_mode.NotesPoints:
            case ui_sidebar_mode.NotesRoutes:
            case ui_sidebar_mode.Burrows:
            case ui_sidebar_mode.Hauling:
            case ui_sidebar_mode.ArenaWeather:
            case ui_sidebar_mode.ArenaTrees:
            default:
                mode = sidebar.mode;
                break;
        }
    }

    private void EmptyMenu()
    {
        foreach (Transform child in menuPanel.transform)
        {
            Destroy(child.gameObject);
        }

    }

    private void BuildDefaultMenu(SidebarState sidebar)
    {
        if (sidebar != null && mode == sidebar.mode)
            return;
        AddMenuButton("View Announcements");
        AddMenuButton("Building", delegate { SetSidebar(ui_sidebar_mode.Build.ToString()); });
        AddMenuButton("Reports");
        AddMenuButton("Civilizations/World Info");
        AddMenuButton("Designations", delegate { SetSidebar(ui_sidebar_mode.DesignateMine.ToString()); });
        AddMenuButton("Set Order", delegate { SetSidebar(ui_sidebar_mode.Orders.ToString()); });
        AddMenuButton("Unit List");
        AddMenuButton("Lob List");
        AddMenuButton("Military");
        AddMenuButton("Squads", delegate { SetSidebar(ui_sidebar_mode.Squads.ToString()); });
        AddMenuButton("Points/Routes/Notes", delegate { SetSidebar(ui_sidebar_mode.NotesPoints.ToString()); });
        AddMenuButton("Make Burrows", delegate { SetSidebar(ui_sidebar_mode.Burrows.ToString()); });
        AddMenuButton("Hauling", delegate { SetSidebar(ui_sidebar_mode.Hauling.ToString()); });
        AddMenuButton("Stockpiles", delegate { SetSidebar(ui_sidebar_mode.Stockpiles.ToString()); });
        AddMenuButton("Zones", delegate { SetSidebar(ui_sidebar_mode.Zones.ToString()); });
        AddMenuButton("Set Building Tasks/Prefs", delegate { SetSidebar(ui_sidebar_mode.QueryBuilding.ToString()); });
        AddMenuButton("View Rooms/Buildings");
        AddMenuButton("View Items in Buildings", delegate { SetSidebar(ui_sidebar_mode.BuildingItems.ToString()); });
        AddMenuButton("View Units", delegate { SetSidebar(ui_sidebar_mode.ViewUnits.ToString()); });
        AddMenuButton("Hot Keys", delegate { SetSidebar(ui_sidebar_mode.Hotkeys.ToString()); });
        AddMenuButton("Locations and Occupations");
        AddMenuButton("Nobles and Administrators");
        AddMenuButton("Status");
        AddMenuButton("Look", delegate { SetSidebar(ui_sidebar_mode.LookAround.ToString()); });
        AddMenuButton("Help");
        AddMenuButton("Options");
        AddMenuButton("Depot Access", delegate { SetSidebar(ui_sidebar_mode.DepotAccess.ToString()); });
        AddMenuButton("Resume");
        if(sidebar != null)
            mode = sidebar.mode;
    }

    private void BuildBuildMenu(SidebarState sidebar)
    {
        if(menuPanel.childCount != sidebar.menu_items.Count)
        {
            EmptyMenu();
            foreach (var item in sidebar.menu_items)
            {
                if (item.building_type != null)
                    AddMenuButton(GameMap.buildings[item.building_type].id);
                else
                    AddMenuButton(item.build_category.ToString());
            }
        }
        mode = sidebar.mode;
    }


    private void AddMenuButton(string label, UnityAction action = null)
    {
        Button button = Instantiate(buttonPrefab);
        button.GetComponentInChildren<Text>().text = label;
        button.name = label;
        if(action != null)
            button.onClick.AddListener(action);
        button.transform.SetParent(menuPanel, false);
    }
}
