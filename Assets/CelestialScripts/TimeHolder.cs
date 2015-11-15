using UnityEngine;
using System.Collections;

public class TimeHolder : MonoBehaviour {

    public int fixedHour = 11;
    public bool useFixedTime = true;

    public DFTime realTime;
    public DFTime displayedTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (useFixedTime == false)
                StopTime();
            else
                useFixedTime = false;
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            StopTime();
            fixedHour += 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            StopTime();
            fixedHour -= 1;
        }
        if (fixedHour >= 24)
            fixedHour -= 24;
        if (fixedHour < 0)
            fixedHour += 24;

        if (useFixedTime)
            displayedTime = new DFTime(realTime.Year, realTime.Month, realTime.Day, fixedHour, 0);
        else
            displayedTime = realTime;
    }

    void StopTime()
    {
        if (useFixedTime == true)
            return; //Nothingto do.
        if (realTime.Minute > 30)
            fixedHour = realTime.Hour + 1;
        else
            fixedHour = realTime.Hour;
        useFixedTime = true;
    }
}
