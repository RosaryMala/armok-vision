using UnityEngine;
using System.Collections;

public class MapPositionUpdater : MonoBehaviour
{
    GameMap map;

    public Vector3 offset;

    public enum FirstPerson
    {
        DontCare,
        Yes,
        No
        
    }

    public FirstPerson firstPerson;

    public void Awake()
    {
        map = FindObjectOfType<GameMap>();
    }

    Vector3 oldPos;

    public void Update()
    {
        Vector3 newPos = transform.TransformPoint(offset);

        if (map != null && (oldPos - newPos).sqrMagnitude > 0.01)
        {
            map.UpdateCenter(newPos);
            oldPos = newPos;
        }
    }

    public void OnEnable()
    {
        if(firstPerson != FirstPerson.DontCare)
            map.firstPerson = firstPerson == FirstPerson.Yes;
    }
}
