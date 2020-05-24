using RemoteFortressReader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAttachmentPoint : MonoBehaviour
{
    public string ItemToken;
    [Tooltip("Number of objects to make a full line in condensed mode, or number to preview for evenly spaced.")]
    public int numerOfItems = 10;
    public Transform previewObject;
    public Vector3[] points;
    private void Reset()
    {
        points = new Vector3[] {
            new Vector3(0, 0.1f, 0),
            new Vector3(0, 0.2f, 0),
            new Vector3(0, 0.3f, 0)
        };
    }

    List<ItemModel> placedObjects = new List<ItemModel>();

    public ItemModel Add(Item item)
    {
        var thing = ItemManager.InstantiateItem(item, transform.parent);
        float t = placedObjects.Count / (float)numerOfItems;
        thing.transform.position = GetPoint(t);
        thing.transform.rotation = transform.rotation * Quaternion.Inverse(GetLocalRotation(t));
        thing.transform.localScale = transform.localScale;
        placedObjects.Add(thing);
        return thing;
    }

    public void Clear()
    {
        for(int i = 0; i < placedObjects.Count; i++)
        {
            Destroy(placedObjects[i]);
        }
        placedObjects.Clear();
    }

    public void OnValidate()
    {
        if (numerOfItems < 2)
            numerOfItems = 2;
    }

    public Vector3 GetPoint(float t)
    {
        return transform.TransformPoint(GetLocalPoint(t));
    }

    public Vector3 GetLocalPoint(float t)
    {
        return Bezier.GetPoint(Vector3.zero, points[0], points[1], points[2], t);
    }

    public Vector3 GetLocalVelocity(float t)
    {
        return Bezier.GetFirstDerivative(Vector3.zero, points[0], points[1], points[2], t);
    }

    public Vector3 GetVelocity(float t)
    {
        return transform.TransformPoint(Bezier.GetFirstDerivative(Vector3.zero, points[0], points[1], points[2], t)) - transform.position;
    }

    public Vector3 GetLocalDirection(float t)
    {
        return GetLocalVelocity(t).normalized;
    }

    public Quaternion GetLocalRotation(float t)
    {
        return Quaternion.Inverse(Quaternion.LookRotation(GetLocalVelocity(0), Vector3.forward)) * Quaternion.LookRotation(GetLocalVelocity(t), Vector3.forward);
    }

    public Quaternion GetRotation(float t)
    {
        return Quaternion.Inverse(Quaternion.LookRotation(GetVelocity(0), transform.forward)) * Quaternion.LookRotation(GetVelocity(t), transform.forward);
    }

    private void OnDrawGizmosSelected()
    {
        Mesh mesh = null;
        Quaternion rotation = Quaternion.identity;
        Vector3 scale = Vector3.one;
        if (previewObject != null)
        {
            MeshFilter meshFilter = previewObject.GetComponentInChildren<MeshFilter>();
            if (meshFilter != null)
            {
                mesh = meshFilter.sharedMesh;
                rotation = transform.rotation * meshFilter.transform.rotation;
                scale = transform.lossyScale * meshFilter.transform.lossyScale.magnitude;
            }
        }
        if (mesh != null)
        {
            for (int i = 0; i <= (numerOfItems - 1); i++)
            {
                float t = i / (float)(numerOfItems - 1);
                Gizmos.color = Color.Lerp(Color.white, Color.black, t);
                Gizmos.DrawWireMesh(mesh, transform.TransformPoint(GetLocalPoint(t)), rotation * GetLocalRotation(t), scale);
            }
        }
    }
}
