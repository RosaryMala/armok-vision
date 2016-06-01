using UnityEngine;
using System.Collections;
using System.Threading;

public class TimeHolder : MonoBehaviour {

    public int fixedHour = 11;
    public bool useFixedTime = true;
    
    public DFTime realTime;
    static long _displayedTimeTicks;

    public static DFTime DisplayedTime
    {
        get
        {
            return new DFTime(_displayedTimeTicks);
        }
        set
        {
            Interlocked.Exchange(ref _displayedTimeTicks, value.Ticks);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("PauseTime"))
        {
            if (useFixedTime == false)
                StopTime();
            else
                useFixedTime = false;
        }
        if (Input.GetButtonDown("TimeForward"))
        {
            StopTime();
            fixedHour += 1;
        }
        if (Input.GetButtonDown("TimeReverse"))
        {
            StopTime();
            fixedHour -= 1;
        }
        if (fixedHour >= 24)
            fixedHour -= 24;
        if (fixedHour < 0)
            fixedHour += 24;

        if (useFixedTime)
            DisplayedTime = new DFTime(realTime.Year, realTime.Month, realTime.Day, fixedHour, 0);
        else
            DisplayedTime = realTime;
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
