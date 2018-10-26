using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOnceScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (GameMap.Instance == null)
            return;
        if (GameMap.Instance.FollowPos.Max <= 0)
            return;
        transform.position = GameMap.DFtoUnityCoord(GameMap.Instance.FollowPos) + new Vector3(0, GameMap.floorHeight,0);
        //enabled = false;
    }
}
