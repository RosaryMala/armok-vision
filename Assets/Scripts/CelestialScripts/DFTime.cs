using UnityEngine;
using System;

[System.Serializable]
public struct DFTime : IComparable, IFormattable,
    IComparable<DFTime>, IEquatable<DFTime>
{
    // Number of 100ns ticks per time unit 
    private const long TicksPerMillisecond = 10000;
    private const long TicksPerSecond = TicksPerMillisecond * 1000;
    private const long TicksPerMinute = TicksPerSecond * 60;
    private const long TicksPerHour = TicksPerMinute * 60;
    private const long TicksPerDay = TicksPerHour * 24;
    private const long TicksPerMonth = TicksPerDay * 28;
    private const long TicksPerYear = TicksPerMonth * 12;

    private const long TicksPerDfTick = TicksPerDay / 1200;

    // Number of milliseconds per time unit 
    private const int MillisPerSecond = 1000;
    private const int MillisPerMinute = MillisPerSecond * 60;
    private const int MillisPerHour = MillisPerMinute * 60;
    private const int MillisPerDay = MillisPerHour * 24;

    // Number of days in a non-leap year 
    private const int DaysPerYear = 336;
    private const int DaysPerMonth = 28;

    internal const long MinTicks = 0;
    internal const long MaxTicks = long.MaxValue;


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
    private readonly long _ticks;

    public DFTime(long ticks)
    {
        _ticks = ticks;
    }

    /// <summary>
    /// Constructs a DF-compatible time object from a given year and df-length ticks.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="cur_year_ticks"></param>
    public DFTime(int year, int cur_year_ticks)
    {
        _ticks = year * TicksPerYear + cur_year_ticks * TicksPerDfTick;
    }

    public DFTime(int year, int month, int day, int hour, int minute, int second = 0, int millisecond = 0)
    {

        _ticks = year * TicksPerYear + (month - 1) * TicksPerMonth + (day - 1) * TicksPerDay + hour * TicksPerHour + minute * TicksPerMinute + second * TicksPerSecond + millisecond * TicksPerMillisecond;
    }

    public long Ticks
    {
        get { return _ticks; }
    }
    
    public int CurrentYearTicks
    {
        get
        {
            return (int)((_ticks % TicksPerYear) / TicksPerDfTick);
        }
    }

    public float YearFraction
    {
        get
        {
            return (float)(_ticks % TicksPerYear) / TicksPerYear;
        }
    }

    public int Year
    {
        get { return (int)(_ticks / TicksPerYear); }
    }

    public MonthName MonthEnum
    {
        get
        {
            return (MonthName)(Month-1);
        }
    }

    public int Month
    {
        get
        {
            return (int)((Ticks / TicksPerMonth) % 12) + 1;
        }
    }

    public int Day
    {
        get { return (int)((_ticks / TicksPerDay) % 28) + 1; }
    }

    public int Hour
    {
        get { return (int)((_ticks / TicksPerHour) % 24); }
    }

    public int Millisecond
    {
        get { return (int)((_ticks / TicksPerMillisecond) % 1000); }
    }

    public int Minute
    {
        get { return (int)((_ticks / TicksPerMinute) % 60); }
    }

    public int Second
    {
        get { return (int)((_ticks / TicksPerSecond) % 60); }
    }

    public string DateString
    {
        get
        {
            return Day.ToString() + ", " + MonthEnum + ", " + Year;
        }
    }

    public float DayFraction
    {
        get
        {
            return (Ticks % TicksPerDay) / (float)TicksPerDay;
        }
    }

    public float MonthFraction
    {
        get
        {
            return (Ticks % TicksPerMonth) / (float)TicksPerMonth;
        }
    }
    public float HourFraction
    {
        get
        {
            return (Ticks % TicksPerHour) / (float)TicksPerHour;
        }
    }
    public float MinuteFraction
    {
        get
        {
            return (Ticks % TicksPerMinute) / (float)TicksPerMinute;
        }
    }

    public float SunAngle
    {
        get
        {
            return 270-(((float)DayFraction) * 360);
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

    public int CompareTo(object obj)
    {
        return _ticks.CompareTo(obj);
    }

    public int CompareTo(DFTime other)
    {
        return _ticks.CompareTo(other.Ticks);
    }

    public bool Equals(DFTime other)
    {
        return _ticks.Equals(other.Ticks);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return ((DateTime)this).ToString(format, formatProvider);
    }

    // Checks if this DFTime is equal to a given object. Returns
    // true if the given object is a boxed DFTime and its value 
    // is equal to the value of this DFTime. Returns false
    // otherwise. 
    // 
    public override bool Equals(object value)
    {
        if (value is DFTime)
        {
            return Ticks == ((DFTime)value).Ticks;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return _ticks.GetHashCode();
    }

    // Compares two DFTime values for equality. Returns true if
    // the two DFTime values are equal, or false if they are
    // not equal.
    // 
    public static bool Equals(DFTime t1, DFTime t2)
    {
        return t1.Ticks == t2.Ticks;
    }

    public static explicit operator DateTime(DFTime dfTime)
    {
        return new DateTime(dfTime.Year, dfTime.Month, dfTime.Day, dfTime.Hour, dfTime.Minute, dfTime.Second, dfTime.Millisecond);
    }

    public static DFTime operator +(DFTime d, TimeSpan t)
    {
        long ticks = d.Ticks;
        long valueTicks = t.Ticks;
        if (valueTicks > MaxTicks - ticks || valueTicks < MinTicks - ticks)
        {
            throw new ArgumentOutOfRangeException();
        }
        return new DFTime((Int64)(ticks + valueTicks));
    }

    public static DFTime operator -(DFTime d, TimeSpan t)
    {
        long ticks = d.Ticks;
        long valueTicks = t.Ticks;
        if (ticks - MinTicks < valueTicks || ticks - MaxTicks > valueTicks)
        {
            throw new ArgumentOutOfRangeException();
        }
        return new DFTime((Int64)(ticks - valueTicks));
    }

    public static TimeSpan operator -(DFTime d1, DFTime d2)
    {
        return new TimeSpan(d1.Ticks - d2.Ticks);
    }

    public static bool operator ==(DFTime d1, DFTime d2)
    {
        return d1.Ticks == d2.Ticks;
    }

    public static bool operator !=(DFTime d1, DFTime d2)
    {
        return d1.Ticks != d2.Ticks;
    }

    public static bool operator <(DFTime t1, DFTime t2)
    {
        return t1.Ticks < t2.Ticks;
    }

    public static bool operator <=(DFTime t1, DFTime t2)
    {
        return t1.Ticks <= t2.Ticks;
    }

    public static bool operator >(DFTime t1, DFTime t2)
    {
        return t1.Ticks > t2.Ticks;
    }

    public static bool operator >=(DFTime t1, DFTime t2)
    {
        return t1.Ticks >= t2.Ticks;
    }

    public float SolsticeAngle
    {
        get
        {
            float angle = (_ticks % TicksPerYear) * 360.0f / TicksPerYear;
            angle -= 105;
            if (angle < 0)
                angle += 360;
            return angle;
        }
    }
}
