using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BodyPartModel))]
public class BodyPartModelEditor : Editor
{
    private void OnSceneGUI()
    {
        BodyPartModel part = target as BodyPartModel;

        foreach (var childPart in part.childParts)
        {
            EditorGUI.BeginChangeCheck();
            var pos = Handles.PositionHandle(childPart.pos1, Quaternion.Euler(childPart.rot1));
            var rot = Handles.RotationHandle(Quaternion.Euler(childPart.rot1), childPart.pos1);
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Adjust child marker");
                childPart.pos1 = pos;
                childPart.rot1 = rot.eulerAngles;
            }
        }
    }
}
