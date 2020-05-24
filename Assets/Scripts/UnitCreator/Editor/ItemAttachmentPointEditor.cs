using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemAttachmentPoint))]
public class ItemAttachmentPointEditor : Editor
{
    private ItemAttachmentPoint attach;
    private Transform handleTransform;
    private Quaternion handleRotation;

    List<Transform> previewObjects = new List<Transform>();

    private void OnSceneGUI()
    {
        attach = target as ItemAttachmentPoint;
        handleTransform = attach.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = handleTransform.position;
        Vector3 p1 = ShowPoint(0);
        Vector3 p2 = ShowPoint(1);
        Vector3 p3 = ShowPoint(2);

        Handles.color = Color.grey;
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p2, p3);

        Vector3 linestart = attach.transform.TransformPoint(attach.GetLocalPoint(0));
        for (int i = 0; i <= (attach.numerOfItems - 1); i++)
        {
            Vector3 lineEnd = attach.transform.TransformPoint(attach.GetLocalPoint(i / (float)(attach.numerOfItems - 1)));
            Handles.color = Color.white;
            Handles.DrawLine(linestart, lineEnd);
            Handles.color = Color.green;
            Handles.DrawLine(lineEnd, lineEnd + attach.GetVelocity(i / (float)(attach.numerOfItems - 1))*0.5f);
            linestart = lineEnd;
        }
    }

    private Vector3 ShowPoint(int index)
    {
        Vector3 point = handleTransform.TransformPoint(attach.points[index]);
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(attach, "Move Point");
            EditorUtility.SetDirty(attach);
            attach.points[index] = handleTransform.InverseTransformPoint(point);
        }
        return point;
    }
}
