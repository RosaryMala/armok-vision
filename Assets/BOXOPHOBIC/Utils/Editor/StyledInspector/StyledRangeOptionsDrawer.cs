// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledRangeOptions))]
    public class StyledRangeOptionsAttributeDrawer : PropertyDrawer
    {
        StyledRangeOptions a;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            a = (StyledRangeOptions)attribute;            

            GUIStyle styleMid = new GUIStyle();
            styleMid.alignment = TextAnchor.MiddleCenter;
            styleMid.normal.textColor = Color.gray;
            styleMid.fontSize = 7;

            if (a.display.Length > 0)
            {
                EditorGUI.PropertyField(position, property, label, true);
                GUILayout.Space(5);
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            property.floatValue = GUILayout.HorizontalSlider(property.floatValue, a.min, a.max);
            property.floatValue = Mathf.Clamp(property.floatValue, a.min, a.max);
            property.floatValue = Mathf.Round(property.floatValue * 1000f) / 1000f;
            GUILayout.Space(8);
            GUILayout.EndHorizontal();

#if UNITY_2019_3_OR_NEWER
            GUILayout.Space(15);
#endif
            GUILayout.BeginHorizontal();

            int maxWidth = 20;

#if UNITY_2019_3_OR_NEWER
            maxWidth = 28;
#endif
            for (int i = 0; i < a.options.Length - 1; i++)
            {
                GUILayout.Label(a.options[i], styleMid, GUILayout.Width(maxWidth));
                GUILayout.Label("", styleMid);
            }

            GUILayout.Label(a.options[a.options.Length - 1], styleMid, GUILayout.Width(maxWidth));
            GUILayout.EndHorizontal();

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            a = (StyledRangeOptions)attribute;

            if (a.display.Length > 0)
            {
                return 18;
            }
            else
            {
                return -2;
            }
        }
    }
}
