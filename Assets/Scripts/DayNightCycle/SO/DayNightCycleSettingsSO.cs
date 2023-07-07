using UnityEngine;

[CreateAssetMenu(fileName = "DayNightCycleSettings_Default", menuName = "DayNightCycle/Settings", order = 0)]
public class DayNightCycleSettingsSO : ScriptableObject
{
    public MyTimePerDay startTime;
    public MyTimePerDay sunriseTime;
    public MyTimePerDay sunsetTime;
}
