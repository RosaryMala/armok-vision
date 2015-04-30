using UnityEngine;
using System.Collections;
using DFHack;

// Pretty tightly coupled to GameMap, ehhh.
public class MapSelection : MonoBehaviour {
    public GameMap targetMap;
    public float projectionStartDistance = 5;
    public float projectionEndDistance = 300;

    const float AVOID_EDGE_INC = .001f;

    Ray mouseRay;

	void Start () {

	}
	
	void Update () {
        mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
	}

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(mouseRay.origin, mouseRay.direction * 300);
        DFCoord? currentTarget = FindCurrentTarget (mouseRay);
        if (currentTarget.HasValue) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                GameMap.DFtoUnityTileCenter(currentTarget.Value),
                new Vector3(GameMap.tileWidth, GameMap.tileHeight, GameMap.tileWidth)
                );
        }
    }

    DFCoord? FindCurrentTarget (Ray ray) {
        float currentDist = projectionStartDistance;

        // Offsets from the tile corners to check.
        float xOffsetCheck = ray.direction.x > 0 ? GameMap.tileWidth : 0;
        float zOffsetCheck = ray.direction.z > 0 ? GameMap.tileWidth : 0;
        float yOffsetCheck = ray.direction.y > 0 ? GameMap.tileHeight : 0;

        bool hitAnything = false;

        float lastCD = -10000000000000f;

        while (currentDist < projectionEndDistance) {
            if (currentDist < 0) {
                throw new UnityException("Negative distance");
            }
            if (currentDist == lastCD) {
                throw new UnityException("Unchanging distance");
            }
            DFCoord currentCoord = GameMap.UnityToDFCoord(
                ray.origin + ray.direction * (currentDist+AVOID_EDGE_INC)
                );

            Vector3 cornerCoord = GameMap.DFtoUnityBottomCorner(currentCoord);
            if (currentCoord.x < 0 || targetMap.tiles.GetLength(0) <= currentCoord.x ||
                currentCoord.y < 0 || targetMap.tiles.GetLength(1) <= currentCoord.y ||
                currentCoord.z < 0 || targetMap.tiles.GetLength(2) <= currentCoord.z) {
                if (hitAnything) {
                    // We've entered and exited the map.
                    return null;
                }
            } else {
                hitAnything = true;
                // In map bounds, check 
                MapTile currentTile = targetMap.tiles[currentCoord.x, currentCoord.y, currentCoord.z];
                if (currentTile != null) {
                    switch (currentTile.shape) {
                    case RemoteFortressReader.TiletypeShape.EMPTY:
                    case RemoteFortressReader.TiletypeShape.NO_SHAPE:
                        break;
                    case RemoteFortressReader.TiletypeShape.WALL:
                    case RemoteFortressReader.TiletypeShape.BOULDER:
                    case RemoteFortressReader.TiletypeShape.FORTIFICATION:
                    case RemoteFortressReader.TiletypeShape.TRUNK_BRANCH:
                    case RemoteFortressReader.TiletypeShape.TWIG:
                        return currentCoord;
                    case RemoteFortressReader.TiletypeShape.FLOOR:
                    case RemoteFortressReader.TiletypeShape.PEBBLES:
                        // Check if we're in the floor.
                        // (that x, y, and z are in the tile is implied.)
                        if (ray.origin.y + ray.direction.y*currentDist < cornerCoord.y+GameMap.floorHeight) {
                            cornerCoord.y += GameMap.floorHeight;
                            return currentCoord;
                        }
                        float floorY = cornerCoord.y+GameMap.floorHeight;
                        float toFloorMult = (floorY - ray.origin.y) / ray.direction.y;
                        Vector3 floorIntercept = ray.origin + ray.direction*toFloorMult;
                        if (cornerCoord.x < floorIntercept.x && floorIntercept.x < cornerCoord.x+GameMap.tileWidth &&
                            cornerCoord.z < floorIntercept.z && floorIntercept.z < cornerCoord.z+GameMap.tileWidth) {
                            return currentCoord;
                        }
                        break;
                    }
                }
            }
            // Tile is some sort of empty; find increment in distance
            {
                float xMult = (cornerCoord.x + xOffsetCheck - ray.origin.x) / ray.direction.x;
                Vector3 xIntercept = ray.origin + ray.direction*xMult;
                if (cornerCoord.z < xIntercept.z && xIntercept.z < cornerCoord.z+GameMap.tileWidth &&
                    cornerCoord.y < xIntercept.y && xIntercept.y < cornerCoord.y+GameMap.tileHeight) {
                    lastCD = currentDist;
                    currentDist = xMult;
                    continue;
                }
            }
            {
                float zMult = (cornerCoord.z + zOffsetCheck - ray.origin.z) / ray.direction.z;
                Vector3 zIntercept = ray.origin + ray.direction*zMult;
                if (cornerCoord.x < zIntercept.x && zIntercept.x < cornerCoord.x+GameMap.tileWidth &&
                    cornerCoord.y < zIntercept.y && zIntercept.y < cornerCoord.y+GameMap.tileHeight) {
                    lastCD = currentDist;
                    currentDist = zMult;
                    continue;
                }
            }
            {
                float yMult = (cornerCoord.y + yOffsetCheck - ray.origin.y) / ray.direction.y;
                Vector3 yIntercept = ray.origin + ray.direction*yMult;
                if (cornerCoord.x < yIntercept.x && yIntercept.x < cornerCoord.x+GameMap.tileWidth &&
                    cornerCoord.z < yIntercept.z && yIntercept.z < cornerCoord.z+GameMap.tileWidth) {
                    lastCD = currentDist;
                    currentDist = yMult;
                    continue;
                }
            }
            throw new UnityException("WTFF???");
        }

        return null;
    }
}
