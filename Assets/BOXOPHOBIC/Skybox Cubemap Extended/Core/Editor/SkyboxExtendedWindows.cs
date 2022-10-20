using UnityEditor;
using UnityEngine;

public static class SkyboxExtendedWindows
{
    [MenuItem("Window/BOXOPHOBIC/Skybox Cubemap Extended/Publisher Page", false, 8000)]
    public static void MoreAssets()
    {
        Application.OpenURL("https://assetstore.unity.com/publishers/20529");
    }

    [MenuItem("Window/BOXOPHOBIC/Skybox Cubemap Extended/Discord Server", false, 8001)]
    public static void Discord()
    {
        Application.OpenURL("https://discord.com/invite/znxuXET");
    }

    [MenuItem("Window/BOXOPHOBIC/Skybox Cubemap Extended/Documentation", false, 8002)]
    public static void Documentation()
    {
        Application.OpenURL("https://docs.google.com/document/d/1ughK58Aveoet6hpdfYxY5rzkOcIkjEoR0VdN2AhngSc/edit#heading=h.gqix7il7wlwd");
    }

    [MenuItem("Window/BOXOPHOBIC/Skybox Cubemap Extended/Changelog", false, 8003)]
    public static void Changelog()
    {
        Application.OpenURL("https://docs.google.com/document/d/1ughK58Aveoet6hpdfYxY5rzkOcIkjEoR0VdN2AhngSc/edit#heading=h.1rbujejuzjce");
    }

    [MenuItem("Window/BOXOPHOBIC/Skybox Cubemap Extended/Write A Review", false, 9999)]
    public static void WriteAReview()
    {
        Application.OpenURL("https://assetstore.unity.com/packages/vfx/shaders/free-skybox-extended-shader-107400#reviews");
    }
}


