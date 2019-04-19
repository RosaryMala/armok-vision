using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;
using System.Collections.Generic;

public class FindMissingScriptsEditor : EditorWindow
{
    [MenuItem("Window/Utilities/Find Missing Scripts")]
    public static void FindMissingScripts()
    {
        EditorWindow.GetWindow(typeof(FindMissingScriptsEditor));
    }

    [MenuItem("Window/Utilities/Clear Progressbar")]
    public static void ClearProgressbar()
    {
        EditorUtility.ClearProgressBar();
    }

    static List<GameObject> objectList = new List<GameObject>();

    static int missingCount = -1;
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Missing Scripts:");
            EditorGUILayout.LabelField("" + (missingCount == -1 ? "---" : missingCount.ToString()));
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Find missing scripts"))
        {
            objectList.Clear();
            missingCount = 0;
            EditorUtility.DisplayProgressBar("Searching Prefabs", "", 0.0f);

            string[] files = System.IO.Directory.GetFiles(Application.dataPath, "*.prefab", System.IO.SearchOption.AllDirectories);
            EditorUtility.DisplayCancelableProgressBar("Searching Prefabs", "Found " + files.Length + " prefabs", 0.0f);

            Scene currentScene = EditorSceneManager.GetActiveScene();
            string scenePath = currentScene.path;
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

            for (int i = 0; i < files.Length; i++)
            {
                string prefabPath = files[i].Replace(Application.dataPath, "Assets");
                if (EditorUtility.DisplayCancelableProgressBar("Processing Prefabs " + i + "/" + files.Length, prefabPath, (float)i/(float)files.Length))
                    break;

                GameObject go = UnityEditor.AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

                if (go != null)
                {
                    FindInGO(go);
                    go = null;
                    EditorUtility.UnloadUnusedAssetsImmediate(true);
                }
            }

            EditorUtility.DisplayProgressBar("Cleanup", "Cleaning up", 1.0f);
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            EditorUtility.UnloadUnusedAssetsImmediate(true);
            GC.Collect();

            EditorUtility.ClearProgressBar();
        }
        foreach (var item in objectList)
        {
            EditorGUILayout.ObjectField(item, typeof(GameObject), true);
        }
    }

    private static void FindInGO(GameObject go, string prefabName = "")
    {
        Component[] components = go.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                objectList.Add(go);
                missingCount++;
                Transform t = go.transform;

                string componentPath = go.name;
                while (t.parent != null)
                {
                    componentPath = t.parent.name + "/" + componentPath;
                    t = t.parent;
                }
                Debug.LogWarning("Prefab " + prefabName + " has an empty script attached:\n" + componentPath , go);
            }
        }

        foreach (Transform child in go.transform)
        {
            FindInGO(child.gameObject, prefabName);
        }
    }
}
