using UnityEngine;
using System.Collections;

[System.Serializable]
public class DFTime
{
    public const int ticksPerYear = 403200;
    public const int ticksPerDay = 1200;
    public const int daysPerMonth = 28;
    public const int ticksPerMonth = ticksPerDay * daysPerMonth;
    public const int ticksPerHour = ticksPerDay / 24;
    public const float ticksPerMinute = ticksPerHour / 60.0f;

    public enum MonthName
    {
        Granite = 0,
        Slate,
        Felsite,
        Hematite,
        Malachite,
        Galena,
        Limestone,
        Sandstone,
        Timber,
        Moonstone,
        Opal,
        Obsidian
    }

    [SerializeField]
    int year;
    [SerializeField]
    int currentYearTicks;
    
    public int CurrentYearTicks
    {
        get
        {
            return currentYearTicks;
        }
        set
        {
            if(value >= 0)
            {
                year += value / ticksPerYear;
                currentYearTicks = value % ticksPerYear;
            }
            else
            {
                year += (value / ticksPerYear) - 1;
                currentYearTicks = (value % ticksPerYear) + ticksPerYear;
            }
        }
    }

    public int Year
    {
        get { return year; }
        set { year = value; }
    }

    public int DayTicks
    {
        get
        {
            return currentYearTicks % ticksPerDay;
        }
        set
        {
            CurrentYearTicks = CurrentYearTicks - (CurrentYearTicks % ticksPerDay) + value;
        }
    }

    public MonthName MonthEnum
    {
        get
        {
            return (MonthName)(Month-1);
        }
        set
        {
            Month = ((int)value) + 1;
        }
    }

    public int Month
    {
        get
        {
            return (CurrentYearTicks / ticksPerMonth) + 1;
        }
        set
        {
            int currentMonthTicks = CurrentYearTicks % ticksPerMonth;
            CurrentYearTicks = ((value - 1) * ticksPerMonth) + currentMonthTicks;
        }
    }

    public int Day
    {
        get
        {
            return ((CurrentYearTicks / ticksPerDay) - (Month - 1) * daysPerMonth) + 1;
        }
        set
        {
            int currentMonth = CurrentYearTicks / ticksPerMonth;
            int currentDayTicks = CurrentYearTicks % ticksPerDay;
            CurrentYearTicks = (currentMonth * ticksPerMonth) + ((value - 1) * ticksPerDay) + currentDayTicks;
        }
    }

    public int Hour
    {
        get
        {
            return DayTicks / ticksPerHour;
        }
    }

    public int Minute
    {
        get
        {
            return (int)((DayTicks % ticksPerHour) / ticksPerMinute);
        }
    }

    public string DateString
    {
        get
        {
            return Day.ToString() + ", " + MonthEnum + ", " + Year;
        }
    }

    public float SunAngle
    {
        get
        {
            return 270-(((float)DayTicks / (float)ticksPerDay) * 360);
        }
    }

    public string TimeString
    {
        get
        {
            if(Hour == 0)
                return "12" + ":" + Minute.ToString("D2") + " am";
            else if (Hour == 12)
                return "12" + ":" + Minute.ToString("D2") + " pm";
            else if (Hour > 12)
                return (Hour - 12).ToString("D") + ":" + Minute.ToString("D2") + " pm";
            else
                return Hour.ToString("D") + ":" + Minute.ToString("D2") + " am";
        }
    }

    public override string ToString()
    {
        return DateString + ", " + TimeString;
    }
}
