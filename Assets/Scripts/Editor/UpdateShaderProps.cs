using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class UpdateShaderProps
{
    static UpdateShaderProps()
    {
        Shader.SetGlobalColor("_MatColor", new Color(0.75f, 0.25f, 0.75f, 0.5f));
        Shader.SetGlobalColor("_JobColor", new Color(0f, 0.5f, 0.5f, 1f));
    }
}
