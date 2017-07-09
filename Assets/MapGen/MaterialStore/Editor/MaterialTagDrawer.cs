using MaterialStore;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MaterialTag))]
public class MaterialTagDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        
        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        bool tag1Enabled = false;
        bool tag2Enabled = false;
        var typeRect = new Rect(position.x, position.y, 30, position.height);
        var tag1Rect = new Rect(position.x + 35, position.y, 50, position.height);
        var tag2Rect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

        MaterialType type = (MaterialType)property.FindPropertyRelative("type").enumValueIndex;

        switch (type)
        {
            case MaterialType.INORGANIC:
            case MaterialType.COAL:
                typeRect = new Rect(position.x, position.y, (position.width / 2) - 2.5f, position.height);
                tag1Rect = new Rect(position.x + (position.width / 2) + 2.5f, position.y, (position.width / 2) - 2.5f, position.height);
                tag1Enabled = true;
                tag2Enabled = false;
                break;
            case MaterialType.CREATURE:
            case MaterialType.PLANT:
                typeRect = new Rect(position.x, position.y, (position.width / 3) - 2.5f, position.height);
                tag1Rect = new Rect(position.x + (position.width / 3) + 2.5f, position.y, (position.width / 3) - 5, position.height);
                tag2Rect = new Rect(position.x + (2 * position.width / 3)+2.5f, position.y, (position.width / 3) - 2.5f, position.height);
                tag1Enabled = true;
                tag2Enabled = true;
                break;
            default:
                typeRect = new Rect(position.x, position.y, position.width, position.height);
                tag1Enabled = false;
                tag2Enabled = false;
                break;
        }

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("type"), GUIContent.none);
        if (tag1Enabled)
            EditorGUI.PropertyField(tag1Rect, property.FindPropertyRelative("tag1"), GUIContent.none);
        else
            property.FindPropertyRelative("tag1").stringValue = "";
        if (tag2Enabled)
            EditorGUI.PropertyField(tag2Rect, property.FindPropertyRelative("tag2"), GUIContent.none);
        else
            property.FindPropertyRelative("tag2").stringValue = "";


        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
