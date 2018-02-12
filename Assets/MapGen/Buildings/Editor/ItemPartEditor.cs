using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Building
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ItemPart))]
    public class ItemPartEditor : Editor
    {
        public int startIndex = 1;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            GUILayout.Space(5);
            startIndex = EditorGUILayout.IntField("Range Start", startIndex);
            if(GUILayout.Button("Apply Range"))
            {
                for(int i = 0; i < targets.Length; i++)
                {
                    var serial = new SerializedObject(targets[i]);
                    serial.Update();
                    var index = serial.FindProperty("itemIndex");
                    index.intValue = i + startIndex;
                    serial.ApplyModifiedProperties();
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
