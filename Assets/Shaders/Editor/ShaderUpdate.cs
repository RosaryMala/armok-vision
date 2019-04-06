using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ShaderUpdate
{
    static ShaderUpdate()
    {
        Shader.SetGlobalColor("_MatColor", Color.white);
    }
}
