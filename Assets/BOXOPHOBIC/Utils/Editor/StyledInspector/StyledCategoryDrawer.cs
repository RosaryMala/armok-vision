// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledCategory))]
    public class StyledCategoryAttributeDrawer : PropertyDrawer
    {
        StyledCategory a;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            a = (StyledCategory)attribute;

            property.boolValue = StyledGUI.DrawInspectorCategory(a.category, property.boolValue, a.top, a.down, a.colapsable);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return -2;
        }
    }
}
