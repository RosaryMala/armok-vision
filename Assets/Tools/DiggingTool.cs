using UnityEngine;
using System.Collections;
using RemoteFortressReader;
using DFHack;
using UnityEngine.EventSystems;

public class DiggingTool : MonoBehaviour
{

    public Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    // Use this for initialization
    void Start()
    {

    }

    Vector3 lastTargetPos = Vector3.zero;
    bool dragging = false;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            DFCoord mapTargetPos;
            Vector3 unityTargetPos;

            if (MapDataStore.FindCurrentTarget(ray, out mapTargetPos, out unityTargetPos))
            {
                Vector3 mapFloatTargetPos = GameMap.UnityToFloatingDFCoord(unityTargetPos);

                if(dragging)
                {
                    var coordList = ToolBrush.raytrace(lastTargetPos, mapFloatTargetPos);
                    DigCommand command = new DigCommand();

                    command.designation = TileDigDesignation.DEFAULT_DIG;
                    foreach (var item in coordList)
                    {
                        MapDataStore.Tile tile = MapDataStore.Main[item];
                        if (tile == null)
                            continue;
                        if (tile.Hidden || (tile.isWall && tile.tiletypeMaterial != TiletypeMaterial.TREE_MATERIAL))
                            command.locations.Add(item);
                    }
                    if(command.locations.Count > 0)
                        DFConnection.Instance.EnqueueDigCommand(command);
                }
                lastTargetPos = mapFloatTargetPos;
                dragging = true;
            }
            else
                dragging = false;  
        }
        if(Input.GetMouseButtonUp(0) || EventSystem.current.IsPointerOverGameObject())
        {
            //dragging has stopped.
            dragging = false;
        }
    }
}
