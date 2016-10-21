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

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            DFCoord MapTargetPos;
            Vector3 UnityTargetPos;

            if (MapDataStore.FindCurrentTarget(ray, out MapTargetPos, out UnityTargetPos))
            {
                MapDataStore.Tile tile = MapDataStore.Main[MapTargetPos];

                if (tile.Hidden || (tile.isWall && tile.tiletypeMaterial != TiletypeMaterial.TREE_MATERIAL))
                {
                    DigCommand command = new DigCommand();

                    command.designation = TileDigDesignation.DEFAULT_DIG;

                    command.locations.Add(MapTargetPos);

                    DFConnection.Instance.EnqueueDigCommand(command);
                }
            }   
        }
    }
}
