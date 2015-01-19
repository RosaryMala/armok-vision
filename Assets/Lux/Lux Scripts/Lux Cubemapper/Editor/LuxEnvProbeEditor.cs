#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using System.

[CustomEditor(typeof(LuxEnvProbe))]
public class LuxEnvProbeEditor : Editor
{
    private SerializedObject LuxProbe;
    LuxEnvProbe Target;
    LuxCubeProcessor ConvolutedCubemap = new LuxCubeProcessor();
    private SerializedProperty DiffSize;
    private SerializedProperty SpecSize;
    private SerializedProperty UseHDR;
    private SerializedProperty Smooth;
    private SerializedProperty SmoothWidth;
    private SerializedProperty linear;

    // Camera
    private SerializedProperty ClearColor;
    private SerializedProperty ClearFlags;
    private SerializedProperty Culling;
    private SerializedProperty Near;
    private SerializedProperty Far;
    private SerializedProperty UseRTC;

    private SerializedProperty Mode;
    private SerializedProperty BoxSize;
    private SerializedProperty AssignedMeshes;
    private GameObject newAssignedMesh;

    private SerializedProperty ShowBoxSize;


    //private SerializedProperty init;

// objct to manually assign probes
    //private Object diffcubeObj;
    //private Object speccubeObj;

    private SerializedProperty diffcubeObj;
    private SerializedProperty speccubeObj;

    //if you want to show debug msg box
    //just change the value
    //bool ShowDebugMsg = true;

    public override void OnInspectorGUI()
    {

    //void OnEnable()
    //{
        Target = (LuxEnvProbe)target;
        LuxProbe = new SerializedObject(target);

        DiffSize = LuxProbe.FindProperty("DiffSize");
        SpecSize = LuxProbe.FindProperty("SpecSize");
        UseHDR = LuxProbe.FindProperty("HDR");
        Smooth = LuxProbe.FindProperty("SmoothEdges");
        SmoothWidth = LuxProbe.FindProperty("SmoothEdgePixel");
        linear = LuxProbe.FindProperty("Linear");
        
        //Camera
        ClearFlags = LuxProbe.FindProperty("ClearFlags");
        ClearColor = LuxProbe.FindProperty("ClearColor");
        Culling = LuxProbe.FindProperty("CullingMask");
        Near = LuxProbe.FindProperty("Near");
        Far = LuxProbe.FindProperty("Far");
        UseRTC = LuxProbe.FindProperty("UseRTC");

        // BoxProjection
        Mode = LuxProbe.FindProperty("Mode");
        // BoxSize = LuxProbe.FindProperty("BoxSize");
        // AssignedMeshes = LuxProbe.FindProperty("AssignedMeshes");
        // init = LuxProbe.FindProperty("init");
        ShowBoxSize = LuxProbe.FindProperty("ShowBoxSize");

        diffcubeObj = LuxProbe.FindProperty("DIFFCube");
        speccubeObj = LuxProbe.FindProperty("SPECCube");

    //}

    //public override void OnInspectorGUI()
    //{
        //  Debug 
        //  DrawDefaultInspector();

        // Update assigned Materials in BoxProjection Mode
        if (Mode.enumValueIndex == 1) {
            Target.SyncAssignedGameobjects();
        }

        /* Debugging only better leave it as it is
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Cubemaps will be saved as"+Target.cubeName.ToString());
        EditorGUILayout.EndVertical();
        */
        //DrawDefaultInspector();


        // Let's give it some styles \m/
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.margin = new RectOffset(4,4,10,10);
        buttonStyle.padding = new RectOffset(10, 10, 10, 10);
        GUIStyle smallbuttonStyle = new GUIStyle(GUI.skin.button);
        smallbuttonStyle.margin = new RectOffset(4,4,0,0);

        EditorGUILayout.BeginVertical("Box");
        GUILayout.Space(5);
        
        //Cubemap Settings
        GUILayout.Label("Probe Settings", "BoldLabel");
        //FaceSizes
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(DiffSize, new GUIContent("Diffuse Size"));
        EditorGUILayout.PropertyField(SpecSize, new GUIContent("Specular Size"));
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        // Baking Options
        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(114));
        EditorGUILayout.PropertyField(UseHDR, new GUIContent(""), GUILayout.Width(14));
        GUILayout.Label("Pull to HDR", GUILayout.MinWidth(96));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(linear, new GUIContent(""), GUILayout.Width(14));
        GUILayout.Label("Set to Linear Space");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(114));
        EditorGUILayout.PropertyField(Smooth, new GUIContent(""), GUILayout.Width(14));
        GUILayout.Label("Smooth Edges", GUILayout.MinWidth(96));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (Smooth.boolValue) {
            EditorGUILayout.PropertyField(SmoothWidth, new GUIContent(""), GUILayout.Width(14));
            GUILayout.Label("Edge Width");
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("Box");
        GUILayout.Space(4);
        EditorGUILayout.PropertyField(Mode, new GUIContent("Cube Mode"));
        if (Mode.enumValueIndex == 1) {
            GUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(ShowBoxSize, new GUIContent(""), GUILayout.Width(14));
                GUILayout.Label("Show Probe’s Size");
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            Target.ShowAssignedMeshes = EditorGUILayout.Foldout(Target.ShowAssignedMeshes,"Manage associated GameObjects");
            if (Target.ShowAssignedMeshes) {
                GUILayout.Space(4);
                for (int i = 0; i < Target.AssignedMeshes.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(4);
    //          TODO: Make it work with SerializedProperty
                    Target.AssignedMeshes[i] = (GameObject)EditorGUILayout.ObjectField("", Target.AssignedMeshes[i], typeof(GameObject), true);
                    if (GUILayout.Button("Remove", EditorStyles.miniButton, GUILayout.Width(50)) ) {
                        Target.AssignedMeshes.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(2);
                }

                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(4);
                newAssignedMesh = (GameObject)EditorGUILayout.ObjectField("", newAssignedMesh, typeof(GameObject), true);
                if (GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.Width(50)) ) {
                    if (newAssignedMesh) {
                        Target.AssignedMeshes.Add(newAssignedMesh);
                        newAssignedMesh = null;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(5);
        }
        EditorGUILayout.EndVertical();
        
        //Camera Settings
        GUILayout.Space(5);
        GUILayout.Label("Probe Camera Settings", "BoldLabel");
        EditorGUILayout.PropertyField(ClearFlags, new GUIContent("Clear Flags"));       
        EditorGUILayout.PropertyField(ClearColor, new GUIContent("Clear Color"));
        EditorGUILayout.PropertyField(Culling, new GUIContent("Culling Mask"));
        EditorGUILayout.PropertyField(Near, new GUIContent("Near Clip Plane"));
        EditorGUILayout.PropertyField(Far, new GUIContent("Far Clip Plane"));
        
        // In Box Mode we probably have to use the custom capture function
        if ( (Mode.enumValueIndex == 1) && (Target.ProbeRotation != Vector3.zero) )
        {   
            Target.UseRTC = false;
            GUI.enabled = false;
        }
        EditorGUILayout.PropertyField(UseRTC, new GUIContent("RenderToCubemap"));
        //
        GUI.enabled = true;

        if (GUILayout.Button("Bake Probe", buttonStyle))
        {
            if(UseRTC.boolValue) { 
                Target.RenderToCubeMap();
            }
            else {
              Target.PreSetup();
              Target.InitRenderCube();
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("Box");
        GUILayout.Space(5);
        GUILayout.Label("Process Cubemaps", "BoldLabel");

    //    EditorGUILayout.BeginHorizontal();
    //    EditorGUILayout.BeginVertical();
    //    GUILayout.Label("Diffuse Cubemap");
        
        // use object in order to enable manual assignment
        //   diffcubeObj = EditorGUILayout.ObjectField(Target.DIFFCube, typeof(Cubemap), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
        //   Target.DIFFCube = (Cubemap)diffcubeObj;
        EditorGUILayout.PropertyField(diffcubeObj, new GUIContent("Diffuse Cubemap"));

    //    EditorGUILayout.EndVertical();
    //    EditorGUILayout.BeginVertical();
    //    GUILayout.Label("Specular Cubemap");

        // use object in order to enable manual assignment
        //   speccubeObj = EditorGUILayout.ObjectField(Target.SPECCube, typeof(Cubemap), false, GUILayout.MinHeight(64), GUILayout.MinWidth(64), GUILayout.MaxWidth(64));
        //   Target.SPECCube = (Cubemap)speccubeObj;
        EditorGUILayout.PropertyField(speccubeObj, new GUIContent("Specular Cubemap"));

    //    EditorGUILayout.EndVertical();
    //    EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Get Generated Cubemaps", buttonStyle))
        {
            Target.init = false;
            Target.CleanUp();
            Target.RetrieveCubemaps();
            // Cubemap diff = Target.DIFFCube;
            // Cubemap spec = Target.SPECCube;
        }

        if (GUILayout.Button("Convolve Cubemaps", buttonStyle))
        {
            Target.init = false;
            Target.CleanUp();
            Cubemap diff = null;
            Cubemap spec = null;
            if (!EditorApplication.isPlaying && Target.DIFFCube != null && Target.SPECCube != null)
            {
                diff = Target.DIFFCube;
                spec = Target.SPECCube;
            }
            bool hdr = Target.HDR;
            if (diff == null || spec == null)
            {
                if (UnityEditor.EditorUtility.DisplayDialog("No Cubemaps found", "Please generate or assign cubemaps to the probe.", "OK"))
                {
                    return;
                }
            }
            if (UnityEditor.EditorUtility.DisplayDialog("Convolve Cubemaps", "Proceed to cubemap convolution?\nThis could take a while.", "Proceed", "Cancel"))
            {
                ConvolutedCubemap.ProcessCubemap(diff, true, hdr);
                ConvolutedCubemap.ProcessCubemap(spec, false, hdr);
            }

        }
        EditorGUILayout.EndVertical();


        if(GUI.changed){
  //       EditorUtility.SetDirty(Target);
         //EditorUtility.SetDirty(myTrackData);
        }


        LuxProbe.ApplyModifiedProperties();

        //Debugging only better leave it as it is
        /*
        if (ShowDebugMsg)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.ToggleLeft("Init",Target.init);
            GUILayout.Label("Cubemaps will be saved as:");
            if (Target.DiffPath != null && Target.SpecPath != null)
            {
                GUILayout.Label(Target.DiffPath.ToString(), EditorStyles.wordWrappedLabel);
                GUILayout.Label(Target.SpecPath.ToString(), EditorStyles.wordWrappedLabel);
            }
            EditorGUILayout.EndVertical();
        }*/
    }
}
#endif
