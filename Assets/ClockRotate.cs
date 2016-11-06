using UnityEngine;
using System.Collections;

public class ClockRotate : MonoBehaviour
{
    public enum TimeUnit
    {
        Year,
        Month,
        Day,
        HalfDay,
        Hour,
        Minute
    }
    RectTransform rect;
    public bool counterClockwise;
    public float offset;
    public TimeUnit timeUnit;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    // Update is called once per frame
    void Update()
    {
        float amount = 0f;
        switch (timeUnit)
        {
            case TimeUnit.Year:
                amount = TimeHolder.DisplayedTime.YearFraction;
                break;
            case TimeUnit.Month:
                amount = TimeHolder.DisplayedTime.MonthFraction;
                break;
            case TimeUnit.Day:
                amount = TimeHolder.DisplayedTime.DayFraction;
                break;
            case TimeUnit.HalfDay:
                amount = TimeHolder.DisplayedTime.DayFraction * 2;
                break;
            case TimeUnit.Hour:
                amount = TimeHolder.DisplayedTime.HourFraction;
                break;
            case TimeUnit.Minute:
                amount = TimeHolder.DisplayedTime.MinuteFraction;
                break;
            default:
                break;
        }
        if (counterClockwise)
            rect.localEulerAngles = new Vector3(0, 0, (amount * 360) + offset);
        else
            rect.localEulerAngles = new Vector3(0, 0, (-amount * 360) - offset);
    }
}
