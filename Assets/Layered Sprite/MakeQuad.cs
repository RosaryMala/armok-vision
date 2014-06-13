// I'd love to hear from you if you do anything cool with this or have any suggestions :)
// www.tenebrous.co.uk

using UnityEngine;
using System.Collections;

public class MeshUtils
{
    public static Mesh MakeQuad(float height, float ratio)
    {
        float originX = -height / ratio / 2;
        float originZ = 0;
        float sizeX = height / ratio;
        float sizeZ = height;
        Mesh newMesh = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
            new Vector3( originX,         originZ, 0 ),
            new Vector3( originX + sizeX, originZ, 0 ),
            new Vector3( originX + sizeX, originZ + sizeZ, 0 ),
            new Vector3( originX,         originZ + sizeZ, 0 )
        };

        int[] triangles = new int[]
        {
            2, 1, 0,
            0, 3, 2
        };

        Vector2[] uvs = new Vector2[]
        {
            new Vector2( 0, 0 ),
            new Vector2( 1, 0 ),
            new Vector2( 1, 1 ),
            new Vector2( 0, 1 )
        };

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        newMesh.uv = uvs;
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        return (newMesh);
    }
}