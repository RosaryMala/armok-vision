using UnityEditor;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
    [CustomEditor(typeof(DepthOfField))]
    class DepthOfFieldEditor : Editor
    {
        private SerializedProperty m_VisualizeFocus;
        private SerializedProperty m_TweakMode;
        private SerializedProperty m_FilteringQuality;
        private SerializedProperty m_ApertureShape;
        private SerializedProperty m_ApertureOrientation;

        private SerializedProperty m_Transform;
        private SerializedProperty m_FocusPlane;
        private SerializedProperty m_Range;
        private SerializedProperty m_NearPlane;
        private SerializedProperty m_NearFalloff;
        private SerializedProperty m_FarPlane;
        private SerializedProperty m_FarFalloff;
        private SerializedProperty m_NearBlurRadius;
        private SerializedProperty m_FarBlurRadius;

        private SerializedProperty m_Texture;
        private SerializedProperty m_Scale;
        private SerializedProperty m_Intensity;
        private SerializedProperty m_Threshold;
        private SerializedProperty m_SpawnHeuristic;

        private void OnEnable()
        {
            var o = serializedObject;

            m_VisualizeFocus = o.FindProperty("settings.visualizeFocus");
            m_TweakMode = o.FindProperty("settings.tweakMode");
            m_FilteringQuality = o.FindProperty("settings.filteringQuality");
            m_ApertureShape = o.FindProperty("settings.apertureShape");
            m_ApertureOrientation = o.FindProperty("settings.apertureOrientation");

            m_Transform = o.FindProperty("focus.transform");
            m_FocusPlane = o.FindProperty("focus.focusPlane");
            m_Range = o.FindProperty("focus.range");
            m_NearPlane = o.FindProperty("focus.nearPlane");
            m_NearFalloff = o.FindProperty("focus.nearFalloff");
            m_FarPlane = o.FindProperty("focus.farPlane");
            m_FarFalloff = o.FindProperty("focus.farFalloff");
            m_NearBlurRadius = o.FindProperty("focus.nearBlurRadius");
            m_FarBlurRadius = o.FindProperty("focus.farBlurRadius");

            m_Texture = o.FindProperty("bokehTexture.texture");
            m_Scale = o.FindProperty("bokehTexture.scale");
            m_Intensity = o.FindProperty("bokehTexture.intensity");
            m_Threshold = o.FindProperty("bokehTexture.threshold");
            m_SpawnHeuristic = o.FindProperty("bokehTexture.spawnHeuristic");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_VisualizeFocus);
            EditorGUILayout.PropertyField(m_TweakMode);
            EditorGUILayout.PropertyField(m_FilteringQuality);
            EditorGUILayout.PropertyField(m_ApertureShape);

            if (m_ApertureShape.intValue != (int)DepthOfField.ApertureShape.Circular)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_ApertureOrientation, new GUIContent("Orientation"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Focus", EditorStyles.boldLabel);

            var falloff = new GUIContent("Falloff");
            var blurRadius = new GUIContent("Blur Radius");

            EditorGUI.indentLevel++;

            if (m_TweakMode.intValue == (int)DepthOfField.TweakMode.Range)
            {
                EditorGUILayout.PropertyField(m_Transform);

                using (new EditorGUI.DisabledGroupScope(m_Transform.objectReferenceValue != null))
                {
                    EditorGUILayout.PropertyField(m_FocusPlane);
                }

                EditorGUILayout.PropertyField(m_Range);

                EditorGUILayout.LabelField(m_NearPlane.displayName);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_NearFalloff, falloff);
                EditorGUILayout.PropertyField(m_NearBlurRadius, blurRadius);
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField(m_FarPlane.displayName);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_FarFalloff, falloff);
                EditorGUILayout.PropertyField(m_FarBlurRadius, blurRadius);
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.PropertyField(m_NearPlane);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_NearFalloff, falloff);
                EditorGUILayout.PropertyField(m_NearBlurRadius, blurRadius);
                EditorGUI.indentLevel--;

                EditorGUILayout.PropertyField(m_FarPlane);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_FarFalloff, falloff);
                EditorGUILayout.PropertyField(m_FarBlurRadius, blurRadius);
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Bokeh", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_Texture);
            if (m_Texture.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(m_Scale);
                EditorGUILayout.PropertyField(m_Intensity);
                EditorGUILayout.PropertyField(m_Threshold);
                EditorGUILayout.PropertyField(m_SpawnHeuristic);
            }
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
