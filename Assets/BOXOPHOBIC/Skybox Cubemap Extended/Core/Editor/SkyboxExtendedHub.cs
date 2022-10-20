// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.StyledGUI;
using Boxophobic.Utils;

public class SkyboxExtendedHub : EditorWindow
{
    string assetFolder = "Assets/BOXOPHOBIC/Atmospheric Height Fog";

    int assetVersion;
    string bannerVersion;

    Color bannerColor;
    string bannerText;
    static SkyboxExtendedHub window;

    [MenuItem("Window/BOXOPHOBIC/Skybox Cubemap Extended/Hub", false, 1070)]
    public static void ShowWindow()
    {
        window = GetWindow<SkyboxExtendedHub>(false, "Skybox Cubemap Extended", true);
        window.minSize = new Vector2(300, 200);
    }

    void OnEnable()
    {
        //Safer search, there might be many user folders
        string[] searchFolders;

        searchFolders = AssetDatabase.FindAssets("Skybox Cubemap Extended");

        for (int i = 0; i < searchFolders.Length; i++)
        {
            if (AssetDatabase.GUIDToAssetPath(searchFolders[i]).EndsWith("Skybox Cubemap Extended.pdf"))
            {
                assetFolder = AssetDatabase.GUIDToAssetPath(searchFolders[i]);
                assetFolder = assetFolder.Replace("/Skybox Cubemap Extended.pdf", "");
            }
        }

        assetVersion = SettingsUtils.LoadSettingsData(assetFolder + "/Core/Editor/Version.asset", -99);
        bannerVersion = assetVersion.ToString();
        bannerVersion = bannerVersion.Insert(1, ".");
        bannerVersion = bannerVersion.Insert(3, ".");

        bannerColor = new Color(0.95f, 0.61f, 0.46f);
        bannerText = "Skybox Cubemap Extended " + bannerVersion;
    }

    void OnGUI()
    {
        DrawToolbar();
        StyledGUI.DrawWindowBanner(bannerColor, bannerText);

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);

        EditorGUILayout.HelpBox("The included shader is compatible by default with Standard and Universal Render Pipelines!", MessageType.Info, true);

        GUILayout.Space(13);
        GUILayout.EndHorizontal();
    }

    void DrawToolbar()
    {
        var GUI_TOOLBAR_EDITOR_WIDTH = this.position.width / 4.0f + 1;

        var styledToolbar = new GUIStyle(EditorStyles.toolbarButton)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Normal,
            fontSize = 11,
        };

        GUILayout.Space(1);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Discord Server", styledToolbar, GUILayout.Width(GUI_TOOLBAR_EDITOR_WIDTH)))
        {
            Application.OpenURL("https://discord.com/invite/znxuXET");
        }
        GUILayout.Space(-1);

        if (GUILayout.Button("Documentation", styledToolbar, GUILayout.Width(GUI_TOOLBAR_EDITOR_WIDTH)))
        {
            Application.OpenURL("https://docs.google.com/document/d/1ughK58Aveoet6hpdfYxY5rzkOcIkjEoR0VdN2AhngSc/edit#heading=h.gqix7il7wlwd");
        }
        GUILayout.Space(-1);

        if (GUILayout.Button("Changelog", styledToolbar, GUILayout.Width(GUI_TOOLBAR_EDITOR_WIDTH)))
        {
            Application.OpenURL("https://docs.google.com/document/d/1ughK58Aveoet6hpdfYxY5rzkOcIkjEoR0VdN2AhngSc/edit#heading=h.1rbujejuzjce");
        }
        GUILayout.Space(-1);

        if (GUILayout.Button("Write A Review", styledToolbar, GUILayout.Width(GUI_TOOLBAR_EDITOR_WIDTH)))
        {
            Application.OpenURL("https://assetstore.unity.com/packages/vfx/shaders/free-skybox-extended-shader-107400#reviews");
        }
        GUILayout.Space(-1);

        GUILayout.EndHorizontal();
        GUILayout.Space(4);
    }
}


