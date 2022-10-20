//#if UNITY_EDITOR
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    [CustomEditor(typeof(StyledMonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class StyledMonoBehaviourEditor : Editor
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