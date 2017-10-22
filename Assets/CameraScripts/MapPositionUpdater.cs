using UnityEngine;
using System.Collections;
using DFHack;

public class MapPositionUpdater : MonoBehaviour
{
    GameMap gameMap;

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
        gameMap = FindObjectOfType<GameMap>();
    }

    Vector3 oldPos;

    public void Update()
    {
        Vector3 newPos = transform.TransformPoint(offset);
        Vector3 scaledOffset = transform.position - newPos;

        if (gameMap != null && (oldPos - newPos).sqrMagnitude > 0.01)
        {
            gameMap.UpdateCenter(newPos);
            oldPos = newPos;
        }

    }

    public void OnEnable()
    {
        if(firstPerson != FirstPerson.DontCare)
            gameMap.firstPerson = firstPerson == FirstPerson.Yes;
    }
}
