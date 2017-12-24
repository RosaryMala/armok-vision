using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DFHack;

public class AdventureMovement : MonoBehaviour
{

    public Transform cameraCenter;

    public float repeatRate = 0.150f;
    public float repeatStart = 0.250f;

    float nexttick = 0;
    bool initialPress = true;
    
    // Update is called once per frame
    void Update()
    {
        Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("CamUpDown"), Input.GetAxis("Vertical"));
        if (dir.sqrMagnitude > 0.1)
        {
            nexttick -= Time.deltaTime;
            if (nexttick <= 0 || initialPress)
            {
                dir.Normalize();

                dir = cameraCenter.TransformDirection(dir);

                DFCoord outDir = new DFCoord(
                    Mathf.RoundToInt(dir.x),
                    Mathf.RoundToInt(-dir.z),
                    Mathf.RoundToInt(dir.y)
                    );
                DFConnection.Instance.SendMoveCommand(outDir);
                if(initialPress)
                {
                    nexttick = repeatStart;
                    initialPress = false;
                }
                else
                {
                    nexttick = repeatRate;
                }
            }
        }
        else
        {
            initialPress = true;
        }
    }
}
