using UnityEngine;

[CreateAssetMenu(fileName = "DayNightCycle_FogSettings_Default", menuName = "DayNightCycle/Components/Fog", order = 3)]
public class DayNightCycle_FogSettingsSO : ScriptableObject
{
    public AnimationCurve baseHeight;
    public AnimationCurve maxHeight;
}
