using UnityEngine;

[CreateAssetMenu(fileName = "DayNightCycleSettings_Default", menuName = "DayNightCycle/Settings", order = 0)]
public class DayNightCycle_SettingsSO : ScriptableObject
{
    public MyTimePerDay startTime;
    public MyTimePerDay sunriseTime;
    public MyTimePerDay sunsetTime;
}
