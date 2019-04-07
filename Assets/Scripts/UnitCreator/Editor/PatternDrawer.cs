using RemoteFortressReader;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PatternDescriptor))]
public class PatternDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var patternTypeRect = new Rect(position.x, position.y, 70, position.height);
        var colorsRect = new Rect(position.x + 75, position.y, position.width - 75, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(patternTypeRect, property.FindPropertyRelative("_pattern"), GUIContent.none);

        var colors = property.FindPropertyRelative("_colors");
        float width = 0;
        if(colors.arraySize > 0)
            width = colorsRect.width / colors.arraySize;

        for (int i = 0; i < colors.arraySize; i++)
        {
            var colorRect = new Rect(colorsRect.x + (i * width), colorsRect.y, width - 5, colorsRect.height);
            EditorGUI.PropertyField(colorRect, colors.GetArrayElementAtIndex(i), GUIContent.none);
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;


        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(ColorDefinition))]
public class ColorDescriptorPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        var redProp = property.FindPropertyRelative("_red");
        var greenProp = property.FindPropertyRelative("_green");
        var blueProp = property.FindPropertyRelative("_blue");

        Color color = new Color(redProp.intValue / 255.0f, greenProp.intValue / 255.0f, blueProp.intValue / 255.0f, 1);

        EditorGUI.BeginChangeCheck();

        color = EditorGUI.ColorField(position, color);

        if(EditorGUI.EndChangeCheck())
        {
            redProp.intValue = Mathf.RoundToInt(color.r * 255);
            greenProp.intValue = Mathf.RoundToInt(color.g * 255);
            blueProp.intValue = Mathf.RoundToInt(color.b * 255);
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();

    }
}

