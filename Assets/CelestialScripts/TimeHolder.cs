using UnityEngine;
using System.Collections;

public class TimeHolder : MonoBehaviour {

    public float fixedTime = 11.0f;
    public bool useFixedTime = true;

    public DFTime realTime;
    public DFTime displayedTime;

    void FixedUpdate()
    {
        displayedTime.Year = realTime.Year;
        displayedTime.CurrentYearTicks = realTime.CurrentYearTicks;

        if(useFixedTime)
            displayedTime.DayTicks = (int)(fixedTime * 50);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            useFixedTime = !useFixedTime;
            fixedTime = realTime.DayTicks / 50.0f;
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            fixedTime = Mathf.Round(fixedTime);
            fixedTime += 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            fixedTime = Mathf.Round(fixedTime);
            fixedTime -= 1;
        }
    }
}
