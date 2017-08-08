using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSpriteTester : MonoBehaviour
{
    public int count;
    private int layerColorID;
    private int layerIndexID;
    public Material mat;
    private Matrix4x4[] matrices = new Matrix4x4[16];
    public Mesh mesh;
    public MaterialPropertyBlock properties;
    public int[] indices = new int[16];
    public Color[] colors = new Color[16];
    private Vector4[] vectors = new Vector4[16];
    private float[] floatIndices = new float[16];


    // Use this for initialization
    void Start()
    {
        layerColorID = Shader.PropertyToID("_LayerColor");
        layerIndexID = Shader.PropertyToID("_LayerIndex");
        properties = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < matrices.Length; i++)
        {
            matrices[i] = transform.localToWorldMatrix;
            vectors[i] = colors[i];
            floatIndices[i] = indices[i];
        }
        properties.SetFloatArray(layerIndexID, floatIndices);
        properties.SetVectorArray(layerColorID, vectors);
        Graphics.DrawMeshInstanced(mesh, 0, mat, matrices, count, properties);
    }
}
