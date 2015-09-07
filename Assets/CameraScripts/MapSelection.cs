using UnityEngine;
using System.Collections;
using DFHack;

// Pretty tightly coupled to GameMap, ehhh.
public class MapSelection : MonoBehaviour
{
    GameMap gameMap;
    public GameObject cameraOrigin;
    public bool debugMode = false;

    public Vector3 dfCoord = new Vector3();
    public Vector3 unityCoord = new Vector3();

    //private Vector3 mouseWorldPosition = Vector3.zero;
    private Vector3 mouseWorldPositionPrevious = Vector3.zero;
    private float mouseWorldPlaneHeight = 0f; 

    const int MAXIMUM_CHECKS = 5000;

    public Material highlightLineMaterial;

    void Awake()
    {
        gameMap = FindObjectOfType<GameMap>();
    }

    //Handle mouse dragging here.
    void Update()
    {
        if (!DFConnection.Connected || !gameMap.enabled) return;

        //mouseWorldPosition = GetMouseWorldPosition(Input.mousePosition);

        UpdateCameraPan();

        if(Input.GetMouseButton(0))
        {
            Ray mouseRay = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            DFCoord currentTarget;
            Vector3 currentTargetCoords;
            if (DFConnection.Connected && gameMap.enabled && FindCurrentTarget(mouseRay, out currentTarget, out currentTargetCoords))
            {
                gameMap.cursX = currentTarget.x;
                gameMap.cursY = currentTarget.y;
                gameMap.cursZ = currentTarget.z;
            }
            else
            {
                gameMap.cursX = -30000;
                gameMap.cursY = -30000;
                gameMap.cursZ = -30000;
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

            cameraOrigin.transform.Translate(previous - current, Space.World);
            gameMap.UpdateCenter(cameraOrigin.transform.position);

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
        if (!FindCurrentTarget(mouseRay, out dfTarget, out WorldPos))
        {
            Plane currentPlane = new Plane(Vector3.up, GameMap.DFtoUnityCoord(0, 0, gameMap.PosZ));
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
        if (!DFConnection.Connected || !gameMap.enabled) return;
        Ray mouseRay = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        DFCoord currentTarget;
        Vector3 currentTargetCoords;
        if (FindCurrentTarget(mouseRay, out currentTarget, out currentTargetCoords))
        {
            //DebugHighlightTile(currentTarget, Color.white);
            unityCoord = currentTargetCoords;
            dfCoord = new Vector3(currentTarget.x, currentTarget.y, currentTarget.z);
        }
    }

    // A big method, but pretty simple.
    // Walk through tiles (starting in current one);
    // in each tile, check if the ray is actively hitting something.
    // If it's not, find the wall of the tile the ray exits through,
    // go to that tile, and repeat.
    bool FindCurrentTarget(Ray ray, out DFCoord tileCoord, out Vector3 unityCoord)
    {
        if (!HitsMapCube(ray))
        {
            tileCoord = default(DFCoord);
            unityCoord = default(Vector3);
            return false;
        }

        // In each tile, we find its bottom corner, and then add these
        // values to find the coordinates of the walls.
        // If the ray increases on this axis, the offset will be the
        // width of the tile along that axis; if the ray decreases,
        // the offset will be 0 (since we're already on that wall.)
        float xWallOffset, yWallOffset, zWallOffset;
        // When we pass through a tile and hit this wall, how do we increment
        // our DFCoord?
        DFCoord xHitIncrement, yHitIncrement, zHitIncrement;
        if (ray.direction.x > 0)
        {
            xWallOffset = GameMap.tileWidth;
            xHitIncrement = new DFCoord(1, 0, 0);
        }
        else
        {
            xWallOffset = 0;
            xHitIncrement = new DFCoord(-1, 0, 0);
        }
        if (ray.direction.z > 0)
        {
            zWallOffset = GameMap.tileWidth;
            zHitIncrement = new DFCoord(0, -1, 0);
        }
        else
        {
            zWallOffset = 0;
            zHitIncrement = new DFCoord(0, 1, 0);
        }
        if (ray.direction.y > 0)
        {
            yWallOffset = GameMap.tileHeight;
            yHitIncrement = new DFCoord(0, 0, 1);
        }
        else
        {
            yWallOffset = 0;
            yHitIncrement = new DFCoord(0, 0, -1);
        }

        // If this is true and we go onto a tile outside the map,
        // we stop iterating (since we can't hit anything.)
        bool haveHitMap = false;

        // The coordinate we start at.
        DFCoord currentCoord = GameMap.UnityToDFCoord(ray.origin);

        // The coordinate of the last tile wall intersection.
        Vector3 lastHit = ray.origin;

        // Cheap hack to keep from looping forever if we screw up somehow.
        for (int _insurance = 0; _insurance < MAXIMUM_CHECKS; _insurance++)
        {

            if (debugMode)
            {
                DebugHighlightTile(currentCoord, Color.blue);
            }

            // Make sure we don't move backwards somehow.
            if ((lastHit.x - ray.origin.x) / ray.direction.x < 0)
            {
                throw new UnityException("Negative distance multiplier?");
            }

            // Get the corner of the current tile.
            Vector3 cornerCoord = GameMap.DFtoUnityBottomCorner(currentCoord);

            // Are we in the selectable area of the map?
            if (!MapDataStore.InMapBounds(currentCoord) || gameMap.PosZ <= currentCoord.z)
            {
                // No.
                if (haveHitMap)
                {
                    // But we have been before;
                    // we've entered and exited the map without hitting anything.
                    tileCoord = default(DFCoord);
                    unityCoord = default(Vector3);
                    return false;
                }
            }
            else
            {
                // We are in the map.
                haveHitMap = true;

                MapDataStore.Tile? currentTile = MapDataStore.Main[currentCoord.x, currentCoord.y, currentCoord.z];
                // Are we in a real tile?
                if (currentTile != null)
                {
                    // Yes.
                    switch (currentTile.Value.shape)
                    {
                        case RemoteFortressReader.TiletypeShape.EMPTY:
                        case RemoteFortressReader.TiletypeShape.NO_SHAPE:
                            // We're not hitting anything, though.
                            break;
                        //case RemoteFortressReader.TiletypeShape.SHRUB:
                        //case RemoteFortressReader.TiletypeShape.SAPLING:
                        case RemoteFortressReader.TiletypeShape.WALL:
                        case RemoteFortressReader.TiletypeShape.FORTIFICATION:
                        //case RemoteFortressReader.TiletypeShape.TRUNK_BRANCH:
                        case RemoteFortressReader.TiletypeShape.TWIG:
                            // We must be hitting things.
                            // (maybe adjust shrub, saplings out of this group?)
                            tileCoord = currentCoord;
                            unityCoord = lastHit;
                            return true;
                        case RemoteFortressReader.TiletypeShape.RAMP:
                        case RemoteFortressReader.TiletypeShape.FLOOR:
                        case RemoteFortressReader.TiletypeShape.BOULDER:
                        case RemoteFortressReader.TiletypeShape.PEBBLES:
                        case RemoteFortressReader.TiletypeShape.BROOK_TOP:
                        case RemoteFortressReader.TiletypeShape.SAPLING:
                        case RemoteFortressReader.TiletypeShape.SHRUB:
                        case RemoteFortressReader.TiletypeShape.BRANCH:
                        case RemoteFortressReader.TiletypeShape.TRUNK_BRANCH:
                            // Check if we're in the floor.
                            // (that we're in the tile is implied.)
                            if (Between(cornerCoord.y, lastHit.y, cornerCoord.y + GameMap.floorHeight))
                            {
                                tileCoord = currentCoord;
                                unityCoord = lastHit;
                                return true;
                            }
                            // Check if we enter the floor; same way we check wall intersections.
                            float floorY = cornerCoord.y + GameMap.floorHeight;
                            float toFloorMult = (floorY - ray.origin.y) / ray.direction.y;
                            Vector3 floorIntercept = ray.origin + ray.direction * toFloorMult;
                            if (Between(cornerCoord.x, floorIntercept.x, cornerCoord.x + GameMap.tileWidth) &&
                                Between(cornerCoord.z, floorIntercept.z, cornerCoord.z + GameMap.tileWidth))
                            {
                                tileCoord = currentCoord;
                                unityCoord = lastHit;
                                return true;
                            }
                            break;
                    }
                }
            }
            // Didn't hit anything in the tile; figure out which wall we're hitting & walk to that tile.
            {
                float xMult = (cornerCoord.x + xWallOffset - ray.origin.x) / ray.direction.x;
                Vector3 xIntercept = ray.origin + ray.direction * xMult;
                if (Between(cornerCoord.z, xIntercept.z, cornerCoord.z + GameMap.tileWidth) &&
                    Between(cornerCoord.y, xIntercept.y, cornerCoord.y + GameMap.tileHeight))
                {
                    lastHit = xIntercept;
                    currentCoord += xHitIncrement;
                    continue;
                }
            }
            {
                float zMult = (cornerCoord.z + zWallOffset - ray.origin.z) / ray.direction.z;
                Vector3 zIntercept = ray.origin + ray.direction * zMult;
                if (Between(cornerCoord.x, zIntercept.x, cornerCoord.x + GameMap.tileWidth) &&
                    Between(cornerCoord.y, zIntercept.y, cornerCoord.y + GameMap.tileHeight))
                {
                    lastHit = zIntercept;
                    currentCoord += zHitIncrement;
                    continue;
                }
            }
            {
                float yMult = (cornerCoord.y + yWallOffset - ray.origin.y) / ray.direction.y;
                Vector3 yIntercept = ray.origin + ray.direction * yMult;
                if (cornerCoord.x <= yIntercept.x && yIntercept.x <= cornerCoord.x + GameMap.tileWidth &&
                    cornerCoord.z <= yIntercept.z && yIntercept.z <= cornerCoord.z + GameMap.tileWidth)
                {
                    lastHit = yIntercept;
                    currentCoord += yHitIncrement;
                    continue;
                }
            }
            // We haven't found a wall to hit.
            // This shouldn't happen, but occasionally does.
            //throw new UnityException("Didn't hit any tile walls?");
        }

        // We went the maximum amount of time without hitting anything
        tileCoord = default(DFCoord);
        unityCoord = default(Vector3);
        return false;
    }

    bool Between(float lower, float t, float upper)
    {
        return lower <= t && t <= upper;
    }

    // Check if a ray could possibly hit the game map at all
    bool HitsMapCube(Ray ray)
    {
        Vector3 lowerLimits = GameMap.DFtoUnityBottomCorner(new DFCoord(0, 0, 0));
        Vector3 upperLimits = GameMap.DFtoUnityBottomCorner(new DFCoord(
            MapDataStore.MapSize.x - 1,
            MapDataStore.MapSize.y - 1,
            MapDataStore.MapSize.z - 1
            )) + new Vector3(GameMap.tileWidth, GameMap.tileHeight, GameMap.tileWidth);

        // Multipliers to scale the ray to hit the different walls of the cube
        float tx1 = (lowerLimits.x - ray.origin.x) / ray.direction.x;
        float tx2 = (upperLimits.x - ray.origin.x) / ray.direction.x;
        float ty1 = (lowerLimits.y - ray.origin.y) / ray.direction.y;
        float ty2 = (upperLimits.y - ray.origin.y) / ray.direction.y;
        float tz1 = (lowerLimits.z - ray.origin.z) / ray.direction.z;
        float tz2 = (upperLimits.z - ray.origin.z) / ray.direction.z;

        float tMin = Mathf.Min(tx1, tx2, ty1, ty2, tz1, tz2);
        float tMax = Mathf.Max(tx1, tx2, ty1, ty2, tz1, tz2);

        // If tMax < 0, cube is entirely behind us; 
        // if tMin > tMax, we don't intersect the cube at all
        return tMin < tMax && 0 < tMax;
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
