using UnityEngine;
using System.Collections;
using System.IO;
using UnityExtension;

[RequireComponent(typeof(MeshFilter))]
public class MeshTest : MonoBehaviour {
    //------------------------------------------------------------------------------------------------------------	
    private const string INPUT_PATH = @"Assets/OBJ-IO/Examples/Meshes/Teapot.obj";
    private const string OUTPUT_PATH = @"Assets/OBJ-IO/Examples/Meshes/Teapot_Modified.obj";

    //------------------------------------------------------------------------------------------------------------	

	// Use this for initialization
	void Start () 
    {
        //	Load the OBJ in
        var lStream = new FileStream(INPUT_PATH, FileMode.Open);
        var lOBJData = OBJLoader.LoadOBJ(lStream);
        var lMeshFilter = GetComponent<MeshFilter>();
        foreach (OBJGroup item in lOBJData.m_Groups)
        {
            Debug.Log(item.m_Name);
        }
        lMeshFilter.mesh.LoadOBJ(lOBJData, "Teapot002");
        lStream.Close();

        lStream = null;
        lOBJData = null;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
