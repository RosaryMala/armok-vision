using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CPUMesh))]
public class CPUMeshDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        property.objectReferenceValue = (Mesh)EditorGUI.ObjectField(position, property.objectReferenceValue, typeof(Mesh), true);

        EditorGUI.EndProperty();
    }
}
