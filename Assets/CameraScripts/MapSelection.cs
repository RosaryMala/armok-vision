using UnityEngine;
using DFHack;
using UnityEngine.EventSystems;

// Pretty tightly coupled to GameMap, ehhh.
public class MapSelection : MonoBehaviour
{
    public CameraMovement cameraOrigin;
    public bool debugMode = false;

    public Vector3 dfCoord = new Vector3();
    public Vector3 unityCoord = new Vector3();

    //private Vector3 mouseWorldPosition = Vector3.zero;
    private Vector3 mouseWorldPositionPrevious = Vector3.zero;
    private float mouseWorldPlaneHeight = 0f;

    public Material highlightLineMaterial;

    //Handle mouse dragging here.
    void Update()
    {
        if (!DFConnection.Connected || GameMap.Instance == null || !GameMap.Instance.enabled) return;

        //mouseWorldPosition = GetMouseWorldPosition(Input.mousePosition);

        UpdateCameraPan();

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray mouseRay = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            DFCoord currentTarget;
            Vector3 currentTargetCoords;
            if (DFConnection.Connected && GameMap.Instance.enabled && MapDataStore.FindCurrentTarget(mouseRay, out currentTarget, out currentTargetCoords))
            {
                GameMap.Instance.cursX = currentTarget.x;
                GameMap.Instance.cursY = currentTarget.y;
                GameMap.Instance.cursZ = currentTarget.z;
                if (debugMode)
                    DebugHighlightTile(currentTarget, Color.red);
            }
            else
            {
                GameMap.Instance.cursX = -30000;
                GameMap.Instance.cursY = -30000;
                GameMap.Instance.cursZ = -30000;
            }
        }
        else
        {
            if (debugMode)
            {
                Ray mouseRay = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                DFCoord currentTarget;
                Vector3 currentTargetCoords;
                if (DFConnection.Connected && GameMap.Instance.enabled && MapDataStore.FindCurrentTarget(mouseRay, out currentTarget, out currentTargetCoords))
                {
                    DebugHighlightTile(currentTarget, Color.blue);
                }
            }
        }
    }

    private void UpdateCameraPan()
    {
        if (Input.GetMouseButtonDown(2))
        {

            //Initialize mouse drag pan

            mouseWorldPositionPrevious = GetMouseWorldPosition(Input.mousePosition);
            mouseWorldPlaneHeight = mouseWorldPositionPrevious.y;

        }
        if (Input.GetMouseButton(2))
        {

            //Mouse drag pan

            Vector3 mouseWorldPosition = GetMouseWorldPosition(Input.mousePosition, mouseWorldPlaneHeight);

            Vector3 current = new Vector3(mouseWorldPosition.x, 0f, mouseWorldPosition.z);
            Vector3 previous = new Vector3(mouseWorldPositionPrevious.x, 0f, mouseWorldPositionPrevious.z);
            Vector3 amount = previous - current;
            if (amount.sqrMagnitude > 0.001f)
                cameraOrigin.following = false;
            cameraOrigin.transform.Translate(previous - current, Space.World);
            GameMap.Instance.UpdateCenter(cameraOrigin.transform.position);

            mouseWorldPositionPrevious = GetMouseWorldPosition(Input.mousePosition, mouseWorldPlaneHeight);
        }
    }

    private Vector3 GetMouseWorldPosition(Vector3 mousePosition, float planeHeight)
    {
        Plane plane = new Plane(Vector3.up, new Vector3(0f, planeHeight, 0f));
        Ray ray = GetComponent<Camera>().ScreenPointToRay(mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return this.mouseWorldPositionPrevious;
    }

    Vector3 GetMouseWorldPosition(Vector3 mousePosition)
    {
        DFCoord dfTarget; //dummy coord to hold things for now.
        Vector3 WorldPos;
        Ray mouseRay = GetComponent<Camera>().ScreenPointToRay(mousePosition);
        if (!MapDataStore.FindCurrentTarget(mouseRay, out dfTarget, out WorldPos))
        {
            Plane currentPlane = new Plane(Vector3.up, GameMap.DFtoUnityCoord(0, 0, GameMap.Instance.PosZ));
            float distance;
            if (currentPlane.Raycast(mouseRay, out distance))
            {
                WorldPos = mouseRay.GetPoint(distance);
            }
            else
            {
                WorldPos = Vector3.zero;
            }
        }
        return WorldPos;
    }

    // If we're attached to a camera, highlight the cube we're pointing at
    // (For now)
    void OnPostRender()
    {
        if (!DFConnection.Connected || GameMap.Instance == null || !GameMap.Instance.enabled) return;
        Ray mouseRay = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        DFCoord currentTarget;
        Vector3 currentTargetCoords;
        if (MapDataStore.FindCurrentTarget(mouseRay, out currentTarget, out currentTargetCoords))
        {
            DebugHighlightTile(currentTarget, Color.white);
            unityCoord = currentTargetCoords;
            dfCoord = new Vector3(currentTarget.x, currentTarget.y, currentTarget.z);
        }
    }

    void DebugHighlightTile(DFCoord tile, Color color)
    {
        DebugHighlightRegion(tile, tile, color);
    }

    void DebugHighlightRegion(DFCoord a, DFCoord b, Color color)
    {
        highlightLineMaterial.SetPass(0);


        Vector3 aVec = GameMap.DFtoUnityBottomCorner(a);
        Vector3 bVec = GameMap.DFtoUnityBottomCorner(b);

        Vector3 lowC = new Vector3(
            Mathf.Min(aVec.x, bVec.x),
            Mathf.Min(aVec.y, bVec.y),
            Mathf.Min(aVec.z, bVec.z)
            );
        Vector3 upC = new Vector3(
            Mathf.Max(aVec.x, bVec.x) + GameMap.tileWidth,
            Mathf.Max(aVec.y, bVec.y) + GameMap.tileHeight,
            Mathf.Max(aVec.z, bVec.z) + GameMap.tileWidth
            );

        // Bottom square
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex3(lowC.x, lowC.y, lowC.z);
        GL.Vertex3(lowC.x, lowC.y, upC.z);
        GL.Vertex3(lowC.x, lowC.y, upC.z);
        GL.Vertex3(upC.x, lowC.y, upC.z);
        GL.Vertex3(upC.x, lowC.y, upC.z);
        GL.Vertex3(upC.x, lowC.y, lowC.z);
        GL.Vertex3(upC.x, lowC.y, lowC.z);
        GL.Vertex3(lowC.x, lowC.y, lowC.z);

        // Vertical lines
        GL.Vertex3(lowC.x, lowC.y, lowC.z);
        GL.Vertex3(lowC.x, upC.y, lowC.z);
        GL.Vertex3(lowC.x, lowC.y, upC.z);
        GL.Vertex3(lowC.x, upC.y, upC.z);
        GL.Vertex3(upC.x, lowC.y, upC.z);
        GL.Vertex3(upC.x, upC.y, upC.z);
        GL.Vertex3(upC.x, lowC.y, lowC.z);
        GL.Vertex3(upC.x, upC.y, lowC.z);

        // Upper square
        GL.Vertex3(lowC.x, upC.y, lowC.z);
        GL.Vertex3(lowC.x, upC.y, upC.z);
        GL.Vertex3(lowC.x, upC.y, upC.z);
        GL.Vertex3(upC.x, upC.y, upC.z);
        GL.Vertex3(upC.x, upC.y, upC.z);
        GL.Vertex3(upC.x, upC.y, lowC.z);
        GL.Vertex3(upC.x, upC.y, lowC.z);
        GL.Vertex3(lowC.x, upC.y, lowC.z);
        GL.End();
    }
}
