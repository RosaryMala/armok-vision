// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledButton))]
    public class StyledButtonAttributeDrawer : PropertyDrawer
    {
        StyledButton a;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            a = (StyledButton)attribute;

            GUILayout.Space(a.Top);

            if (GUILayout.Button(a.Text))
            {
                property.boolValue = true;
            }

            GUILayout.Space(a.Down);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return -2;
        }
    }
}

