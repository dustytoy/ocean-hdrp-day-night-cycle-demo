using System;

[Serializable]
public struct MyTime
{
    public const long TotalTicks = 864000000000;
    public const int TotalSeconds = 86400;
    public const int TotalMinutes = 60;
    public const int TotalHours = 24;
    public const long TicksPerHour = 36000000000;
    public const long TicksPerMinute = 600000000;
    public const long TicksPerSecond = 10000000;

    public int hour;
    public int minute;
    public int second;

    public long ToTicks()
    {
        return hour * TicksPerHour + minute * TicksPerMinute + second * TicksPerSecond;
    }
    public float GetT()
    {
        return ((float)ToTicks() % TotalTicks) / TotalTicks;
    }
    public override string ToString()
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
    }
    public static MyTime ToOneDayTime(long ticks)
    {
        int hour = (int)(ticks / TicksPerHour);
        int minute = (int)((ticks - hour * TicksPerHour) / TicksPerMinute);
        int second = (int)((ticks - hour * TicksPerHour - minute * TicksPerMinute) / TicksPerSecond);
        return new MyTime()
        {
            hour = hour,
            minute = minute,
            second = second
        };
    }
}