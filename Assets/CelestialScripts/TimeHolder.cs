using UnityEngine;
using System.Collections;

public class TimeHolder : MonoBehaviour {

    public int fixedTime = 11;
    public bool useFixedTime = true;

    public DFTime realTime;
    public DFTime displayedTime;

    void FixedUpdate()
    {
        displayedTime = realTime;

        if (useFixedTime)
            displayedTime.SetHour(fixedTime);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            useFixedTime = !useFixedTime;
            fixedTime = realTime.Hour;
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            fixedTime += 1;
            useFixedTime = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            fixedTime -= 1;
            useFixedTime = true;
        }
    }
}
