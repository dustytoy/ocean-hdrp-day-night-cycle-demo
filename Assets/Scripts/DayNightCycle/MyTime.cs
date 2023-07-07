using System;

[Serializable]
public struct MyTimePerDay
{
    public static readonly int TotalSeconds = 86400;
    public static readonly int TotalMinutes = 60;
    public static readonly int TotalHours = 24;
    public static readonly long TicksPerDay = 864000000000;
    public static readonly long TicksPerHour = 36000000000;
    public static readonly long TicksPerMinute = 600000000;
    public static readonly long TicksPerSecond = 10000000;

    public int hour;
    public int minute;
    public int second;
    
    public float GetT()
    {
        return ((float)(long)this % TicksPerDay) / TicksPerDay;
    }
    public override string ToString()
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
    }
    public static float GetT(long currentTick)
    {
        return (float)(currentTick % TicksPerDay) / TicksPerDay;
    }
    public static explicit operator MyTimePerDay(long ticks)
    {
        int hour = (int)(ticks / TicksPerHour);
        int minute = (int)((ticks - hour * TicksPerHour) / TicksPerMinute);
        int second = (int)((ticks - hour * TicksPerHour - minute * TicksPerMinute) / TicksPerSecond);
        return new MyTimePerDay()
        {
            hour = hour,
            minute = minute,
            second = second
        };
    }
    public static explicit operator long(MyTimePerDay time)
    {
        return time.hour * TicksPerHour + time.minute * TicksPerMinute + time.second * TicksPerSecond;
    }
}