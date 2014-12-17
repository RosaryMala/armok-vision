using UnityEngine;
using System.Collections;

//Generic class for returning a mesh for a tile. 
public class GenericTile : MonoBehaviour {
    public Mesh ThisMesh;
	public virtual Mesh GetMesh(GameMap map, int x, int y, int z)
    {
        return ThisMesh;
    }
}
