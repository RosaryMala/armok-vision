// Editor script for the motion blur effect

// Suppress "assigned but never used" warning
#pragma warning disable 414

// Show fancy graphs
#define SHOW_GRAPHS

using UnityEngine;
using UnityEditor;

namespace UnityStandardAssets.CinematicEffects
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MotionBlur))]
    public class MotionBlurEditor : Editor
    {
        #if UNITY_5_4_OR_NEWER

        MotionBlurGraphDrawer _graph;

        SerializedProperty _shutterAngle;
        SerializedProperty _sampleCount;
        SerializedProperty _frameBlending;

        [SerializeField] Texture2D _blendingIcon;

        static GUIContent _textStrength = new GUIContent("Strength");

        void OnEnable()
        {
            _shutterAngle = serializedObject.FindProperty("_settings.shutterAngle");
            _sampleCount = serializedObject.FindProperty("_settings.sampleCount");
            _frameBlending = serializedObject.FindProperty("_settings.frameBlending");
        }

        public override void OnInspectorGUI()
        {
            if (_graph == null) _graph = new MotionBlurGraphDrawer(_blendingIcon);

            serializedObject.Update();

            EditorGUILayout.LabelField("Shutter Speed Simulation", EditorStyles.boldLabel);

            #if SHOW_GRAPHS
            _graph.DrawShutterGraph(_shutterAngle.floatValue);
            #endif

            EditorGUILayout.PropertyField(_shutterAngle);
            EditorGUILayout.PropertyField(_sampleCount);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Multiple Frame Blending", EditorStyles.boldLabel);

            #if SHOW_GRAPHS
            _graph.DrawBlendingGraph(_frameBlending.floatValue);
            #endif

            EditorGUILayout.PropertyField(_frameBlending, _textStrength);

            serializedObject.ApplyModifiedProperties();
        }

        #else

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This effect requires Unity 5.4 or later.", MessageType.Error);
        }

        #endif
    }
}
