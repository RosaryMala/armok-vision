using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class Startup
{
    static Startup()
    {
        Shader.SetGlobalVector("_ViewMin", new Vector3(-9999, -9999, -9999));
        Shader.SetGlobalVector("_ViewMax", new Vector3(9999, 9999, 9999));
    }
}
