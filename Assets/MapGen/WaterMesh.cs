using UnityEngine;
using System.Collections;
using DFHack;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class WaterMesh : MonoBehaviour 
{
    public bool magma = false;
    public DFCoord coord;

    List<Vector3> finalVertices = new List<Vector3>();
    List<int> finalFaces = new List<int>();
    List<Color32> finalVertexColors = new List<Color32>();
    List<Vector2> finalUVs = new List<Vector2>();

    
    public void UpdateLiquids()
    {
        MeshFilter mf = GetComponent("MeshFilter") as MeshFilter;
        Mesh mesh = mf.mesh;

    }

}
