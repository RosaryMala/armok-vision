// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledLayers))]
    public class StyledLayersAttributeDrawer : PropertyDrawer
    {
        StyledLayers a;
        private int index;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            a = (StyledLayers)attribute;

            index = property.intValue;

            string[] allLayers = new string[32];

            for (int i = 0; i < 32; i++)
            {
                if (LayerMask.LayerToName(i).Length < 1)
                {
                    allLayers[i] = "Missing";
                }
                else 
                {
                    allLayers[i] = LayerMask.LayerToName(i);
                }
            }

            if (a.display == "")
            {
                a.display = property.displayName;
            }

            index = EditorGUILayout.Popup(a.display, index, allLayers);

            property.intValue = index;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return -2;
        }
    }
}
