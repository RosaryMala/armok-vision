using DwarfControl;
using proto.enums.ui_sidebar_mode;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DwarfModeMenu : MonoBehaviour
{
    public ui_sidebar_mode mode = ui_sidebar_mode.Default;

    public Button buttonPrefab;
    public RectTransform labelPrefab;
    public RectTransform errorPrefab;
    public RectTransform spacerPrefab;
    public Mesh previewMesh;
    public Material previewMaterial;

    public RectTransform menuPanel;

    // Use this for initialization
    void Start()
    {
        BuildDefaultMenu(null, true);
    }

    // Update is called once per frame
    void Update()
    {
        var sidebar = DFConnection.Instance.SidebarState;
        if (sidebar != null)
            UpdateMenu(sidebar);
        if(prevBuildSelector != null)
        {
            DrawBuildLocation(prevBuildSelector);
        }
    }

    private void DrawBuildLocation(BuildSelector prevBuildSelector)
    {
        if (!(prevBuildSelector.stage == BuildSelectorStage.StagePlace || prevBuildSelector.stage == BuildSelectorStage.StageItemSelect))
            return;

        var mouseCenter = GetMouseCenterDF();
        if (prevBuildSelector.stage == BuildSelectorStage.StageItemSelect)
            mouseCenter = prevBuildSelector.cursor;
        for (int y = mouseCenter.y - prevBuildSelector.radius_y_low; y <= mouseCenter.y + prevBuildSelector.radius_y_high; y++)
            for (int x = mouseCenter.x - prevBuildSelector.radius_x_low; x <= mouseCenter.x + prevBuildSelector.radius_x_high; x++)
            {
                var drawCenter = GameMap.DFtoUnityCoord(x, y, mouseCenter.z);
                Graphics.DrawMesh(previewMesh, Matrix4x4.TRS(drawCenter, Quaternion.identity, Vector3.one), previewMaterial, 0);
            }
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        SidebarCommand sidebarCommand = new SidebarCommand
        {
            selection_coord = mouseCenter
        };
        if (Input.GetMouseButtonDown(0) && prevBuildSelector.stage == BuildSelectorStage.StagePlace)
            sidebarCommand.action = MenuAction.MenuSelect;
        DFConnection.Instance.EnqueueSidebarSet(sidebarCommand);
    }

    Vector3 GetMouseCenter()
    {
        Vector3 viewCenter = GameMap.DFtoUnityCoord(GameMap.Instance.PosXTile, GameMap.Instance.PosYTile, GameMap.Instance.PosZ - 1) + new Vector3(0, GameMap.floorHeight, 0);
        var groundPlane = new Plane(Vector3.up, viewCenter);
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        groundPlane.Raycast(mouseRay, out float distance);
        return mouseRay.GetPoint(distance);
    }

    DFHack.DFCoord GetMouseCenterDF()
    {
        return GameMap.UnityToDFCoord(GetMouseCenter());
    }

    #region Button Callbacks

    public void SetSidebar(string mode)
    {
        SidebarCommand sidebar = new SidebarCommand
        {
            mode = (ui_sidebar_mode)Enum.Parse(typeof(ui_sidebar_mode), mode)
        };
        DFConnection.Instance.EnqueueSidebarSet(sidebar);
    }

    public void BuildButton(string index)
    {
        SidebarCommand sidebar = new SidebarCommand
        {
            mode = ui_sidebar_mode.Build,
            menu_index = int.Parse(index),
            action = MenuAction.MenuSelect
        };
        DFConnection.Instance.EnqueueSidebarSet(sidebar);
    }

    public void CancelButton()
    {
        SidebarCommand sidebar = new SidebarCommand
        {
            action = MenuAction.MenuCancel
        };
        DFConnection.Instance.EnqueueSidebarSet(sidebar);
    }

    #endregion

    void UpdateMenu(SidebarState sidebar)
    {
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
                if (mode != sidebar.mode)
                {
                    EmptyMenu();
                    AddMenuButton("Cancel", CancelButton);
                    mode = sidebar.mode;
                }
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

    private void BuildDefaultMenu(SidebarState sidebar, bool force = false)
    {
        if (mode == ui_sidebar_mode.Default && !force)
            return;
        EmptyMenu();
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
        mode = ui_sidebar_mode.Default;
    }

    int numBuildingOptions = 0;

    private void BuildBuildMenu(SidebarState sidebar)
    {
        if (
            numBuildingOptions != sidebar.menu_items.Count
            || SelectorChanged(sidebar.build_selector)
            || mode != sidebar.mode
            )
        {
            EmptyMenu();
            AddMenuButton("Cancel", CancelButton);

            numBuildingOptions = sidebar.menu_items.Count;
            for (int i = 0; i < sidebar.menu_items.Count; i++)
            {
                string index = i.ToString(); //They all end up with i being Count if we don't do this.
                var item = sidebar.menu_items[i];
                if (item.building_type != null)
                    AddMenuButton(GameMap.buildings[item.building_type].name, delegate { BuildButton(index); });
                else
                    AddMenuButton(item.build_category.ToString(), delegate { BuildButton(index); });
            }

            if(sidebar.build_selector != null)
            {
                AddHeader(GameMap.buildings[sidebar.build_selector.building_type].name);
                AddSpacer();
                foreach (var error in sidebar.build_selector.errors)
                {
                    AddError(error);
                }
                switch (sidebar.build_selector.stage)
                {
                    case BuildSelectorStage.StageNoMat:
                        break;
                    case BuildSelectorStage.StagePlace:
                        break;
                    case BuildSelectorStage.StageItemSelect:
                        for (int i = 0; i < sidebar.build_selector.choices.Count; i++)
                        {
                            var choice = sidebar.build_selector.choices[i];
                            string index = i.ToString(); //They all end up with i being Count if we don't do this.
                            AddMenuButton(choice.name + " " + choice.distance + " " + choice.used_count + "/" + choice.num_candidates, delegate { BuildButton(index); });
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        mode = sidebar.mode;
    }

    BuildSelector prevBuildSelector = null;

    private bool SelectorChanged(BuildSelector theirs)
    {
        var ours = prevBuildSelector;
        prevBuildSelector = theirs;
        if (ours == null)
        {
            if (theirs == null)
                return false;
            return true;
        }
        else
        {
            if (theirs == null)
                return true;
            if ((BuildingStruct)ours.building_type != theirs.building_type)
                return true;
            if (ours.stage != theirs.stage)
                return true;
            if (ours.req_index != theirs.req_index)
                return true;
            if (ours.requirements.Count != theirs.requirements.Count)
                return true;
            if (ours.choices.Count != theirs.choices.Count)
                return true;
            if (ours.errors.Count != theirs.errors.Count)
                return true;
            for (int i = 0; i < ours.choices.Count; i++)
            {
                if (ours.choices[i].used_count != theirs.choices[i].used_count)
                    return true;
                if (ours.choices[i].distance != theirs.choices[i].distance)
                    return true;
                if (ours.choices[i].num_candidates != theirs.choices[i].num_candidates)
                    return true;
                if (ours.choices[i].name != theirs.choices[i].name)
                    return true;
            }
            for (int i = 0; i < ours.requirements.Count; i++)
            {
                if (ours.requirements[i].count_provided != theirs.requirements[i].count_provided)
                    return true;
                if (ours.requirements[i].count_required != theirs.requirements[i].count_required)
                    return true;
                if (ours.requirements[i].count_max != theirs.requirements[i].count_max)
                    return true;
            }
            for (int i = 0; i < ours.errors.Count; i++)
            {
                if (ours.errors[i] != theirs.errors[i])
                    return true;
            }
        }
        return false;
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

    private void AddHeader(string label)
    {
        var prefab = Instantiate(labelPrefab);
        var text = prefab.GetComponentInChildren<Text>();
        text.text = label;
        prefab.name = label;
        prefab.transform.SetParent(menuPanel, false);
    }

    private void AddError(string label)
    {
        var prefab = Instantiate(errorPrefab);
        var text = prefab.GetComponentInChildren<Text>();
        text.text = label;
        prefab.name = label;
        prefab.transform.SetParent(menuPanel, false);
    }

    private void AddSpacer()
    {
        Instantiate(spacerPrefab).transform.SetParent(menuPanel, false);
    }
}
