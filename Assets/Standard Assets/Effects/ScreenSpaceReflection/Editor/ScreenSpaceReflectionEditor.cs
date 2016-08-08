using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
    [CustomPropertyDrawer(typeof(ScreenSpaceReflection.SSRSettings.LayoutAttribute))]
    public class LayoutDrawer : PropertyDrawer
    {
        private const float kHeadingSpace = 22.0f;

        static Styles m_Styles;

        private class Styles
        {
            public readonly GUIStyle header = "ShurikenModuleTitle";

            internal Styles()
            {
                header.font = (new GUIStyle("Label")).font;
                header.border = new RectOffset(15, 7, 4, 4);
                header.fixedHeight = kHeadingSpace;
                header.contentOffset = new Vector2(20f, -2f);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
                return kHeadingSpace;

            var count = property.CountInProperty();
            return EditorGUIUtility.singleLineHeight * count  + 15;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_Styles == null)
                m_Styles = new Styles();

            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = Header(position, property.displayName, property.isExpanded);
            position.y += kHeadingSpace;

            if (!property.isExpanded)
                return;

            foreach (SerializedProperty child in property)
            {
                EditorGUI.PropertyField(position, child);
                position.y += EditorGUIUtility.singleLineHeight;
            }
        }

        private bool Header(Rect position, String title, bool display)
        {
            Rect rect = position;
            position.height = EditorGUIUtility.singleLineHeight;
            GUI.Box(rect, title, m_Styles.header);

            Rect toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (Event.current.type == EventType.Repaint)
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);

            Event e = Event.current;
            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                e.Use();
            }
            return display;
        }
    }

    [CustomEditor(typeof(ScreenSpaceReflection))]
    internal class ScreenSpaceReflectionEditor : Editor
    {
        [NonSerialized]
        private List<SerializedProperty> m_Properties = new List<SerializedProperty>();

        void OnEnable()
        {
            var settings = FieldFinder<ScreenSpaceReflection>.GetField(x => x.settings);
            foreach (var setting in settings.FieldType.GetFields())
            {
                var prop = settings.Name + "." + setting.Name;
                m_Properties.Add(serializedObject.FindProperty(prop));
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            // move into the m_Settings fields...
            foreach (var property in m_Properties)
                EditorGUILayout.PropertyField(property);

            serializedObject.ApplyModifiedProperties();
        }

        private void Apply(ScreenSpaceReflection.SSRSettings settings)
        {
            var validTargets = targets.Where(x => x is ScreenSpaceReflection).Cast<ScreenSpaceReflection>().ToArray();

            Undo.RecordObjects(validTargets, "Apply SSR Settings");
            foreach (var validTarget in validTargets)
                validTarget.settings = settings;
        }
    }
}
