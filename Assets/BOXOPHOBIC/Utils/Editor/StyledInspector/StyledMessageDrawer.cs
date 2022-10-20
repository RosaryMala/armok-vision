// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledMessage))]
    public class StyledMessageAttributeDrawer : PropertyDrawer
    {
        StyledMessage a;

        bool show;
        MessageType messageType;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            show = property.boolValue;

            if (show)
            {
                a = (StyledMessage)attribute;

                if (a.Type == "None")
                {
                    messageType = MessageType.None;
                }
                else if (a.Type == "Info")
                {
                    messageType = MessageType.Info;
                }
                else if (a.Type == "Warning")
                {
                    messageType = MessageType.Warning;
                }
                else if (a.Type == "Error")
                {
                    messageType = MessageType.Error;
                }

                GUILayout.Space(a.Top);
                EditorGUILayout.HelpBox(a.Message, messageType);
                GUILayout.Space(a.Down);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return -2;
        }
    }
}
