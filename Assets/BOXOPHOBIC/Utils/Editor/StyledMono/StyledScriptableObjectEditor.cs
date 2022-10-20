//#if UNITY_EDITOR
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    [CustomEditor(typeof(StyledScriptableObject), true)]
    [CanEditMultipleObjects]
    public class StyledScriptableObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
//#endif