using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSerializer : MonoBehaviour
{
    public MeshContentSerialized outputMesh;

    private void Start()
    {
        outputMesh = new MeshContentSerialized();
    }
}
