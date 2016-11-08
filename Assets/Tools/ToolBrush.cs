using DFHack;
using RemoteFortressReader;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolBrush : MonoBehaviour
{

    public Material cursorMaterial;
    public Material activeCursorMaterial;

    public Mesh cursorMesh;
    private Camera mainCam;
    private bool dragging;
    private Vector3 lastTargetPos = Vector3.zero;

    void Awake()
    {
        mainCam = Camera.main;
    }

    public DiggingTool diggingTool;

    // Update is called once per frame
    void Update()
    {
        if (diggingTool.digMode != DiggingTool.DigMode.None)
        {
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                DFCoord mapTargetPos;
                Vector3 unityTargetPos;

                if (MapDataStore.FindCurrentTarget(ray, out mapTargetPos, out unityTargetPos))
                {
                    unityTargetPos += (ray.direction * 0.001f);
                    Vector3 mapFloatTargetPos = GameMap.UnityToFloatingDFCoord(unityTargetPos);
                    if (dragging)
                    {
                        var coordList = raytrace(lastTargetPos, mapFloatTargetPos);
                        foreach (var item in coordList)
                        {
                            DrawCursor(item, true);
                        }
                        diggingTool.Apply(coordList);
                    }
                    lastTargetPos = mapFloatTargetPos;
                    dragging = true;
                }
                else
                    dragging = false;
            }
            else
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                DFCoord mapTargetPos;
                Vector3 unityTargetPos;
                if (MapDataStore.FindCurrentTarget(ray, out mapTargetPos, out unityTargetPos))
                {
                    unityTargetPos += ray.direction * 0.001f;
                    Vector3 pos = GameMap.UnityToFloatingDFCoord(unityTargetPos);
                    mapTargetPos = new DFCoord(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                    DrawCursor(mapTargetPos, false);
                }
            }
        }
        if (Input.GetMouseButtonUp(0) || EventSystem.current.IsPointerOverGameObject())
        {
            //dragging has stopped.
            dragging = false;
        }

    }

    void DrawCursor(DFCoord pos, bool active)
    {
        Matrix4x4 matrix = Matrix4x4.TRS(GameMap.DFtoUnityCoord(pos), Quaternion.identity, Vector3.one);
        Graphics.DrawMesh(cursorMesh, matrix, active ? activeCursorMaterial : cursorMaterial, 5);
    }

    public static List<DFCoord> raytrace(Vector3 start, Vector3 end)
    {
        float dx = Mathf.Abs(start.x - end.x);
        float dy = Mathf.Abs(start.y - end.y);
        float dz = Mathf.Abs(start.z - end.z);

        int x = Mathf.FloorToInt(start.x);
        int y = Mathf.FloorToInt(start.y);
        int z = Mathf.FloorToInt(start.z);

        float dt_dx = 1.0f / dx;
        float dt_dy = 1.0f / dy;
        float dt_dz = 1.0f / dz;

        int n = 1;
        int x_inc, y_inc, z_inc;
        float t_next_x, t_next_y, t_next_z;

        if (dx == 0)
        {
            x_inc = 0;
            t_next_x = dt_dx; // infinity
        }
        else if (end.x > start.x)
        {
            x_inc = 1;
            n += Mathf.FloorToInt(end.x) - x;
            t_next_x = (Mathf.Floor(start.x) + 1 - start.x) * dt_dx;
        }
        else
        {
            x_inc = -1;
            n += x - Mathf.FloorToInt(end.x);
            t_next_x = (start.x - Mathf.Floor(start.x)) * dt_dx;
        }

        if (dy == 0)
        {
            y_inc = 0;
            t_next_y = dt_dy; // infinity
        }
        else if (end.y > start.y)
        {
            y_inc = 1;
            n += Mathf.FloorToInt(end.y) - y;
            t_next_y = (Mathf.Floor(start.y) + 1 - start.y) * dt_dy;
        }
        else
        {
            y_inc = -1;
            n += y - Mathf.FloorToInt(end.y);
            t_next_y = (start.y - Mathf.Floor(start.y)) * dt_dy;
        }

        if (dz == 0)
        {
            z_inc = 0;
            t_next_z = dt_dz; // infinity
        }
        else if (end.z > start.z)
        {
            z_inc = 1;
            n += Mathf.FloorToInt(end.z) - z;
            t_next_z = (Mathf.Floor(start.z) + 1 - start.z) * dt_dz;
        }
        else
        {
            z_inc = -1;
            n += z - Mathf.FloorToInt(end.z);
            t_next_z = (start.z - Mathf.Floor(start.z)) * dt_dz;
        }

        List<DFCoord> coordList = new List<DFCoord>();

        for (; n > 0; --n)
        {
            coordList.Add(new DFCoord(x, y, z));

            if ((t_next_z < t_next_y) && (t_next_z < t_next_x))
            {
                z += z_inc;
                t_next_z += dt_dz;
            }
            else if (t_next_y < t_next_x)
            {
                y += y_inc;
                t_next_y += dt_dy;
            }
            else
            {
                x += x_inc;
                t_next_x += dt_dx;
            }
        }

        return coordList;
    }
}
