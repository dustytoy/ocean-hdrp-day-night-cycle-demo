using UnityEngine;

[CreateAssetMenu(fileName = "DayNightCycle_SkySettings_Default", menuName = "DayNightCycle/Components/Sky", order = 1)]
public class DayNightCycle_SkySettingsSO : ScriptableObject
{
    public Gradient horizonTint;
    public Gradient zenithTint;
}
