using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DayNightCycle_WaterSettings_Default", menuName = "DayNightCycle/Components/Water", order = 4)]
public class DayNightCycle_WaterSettingsSO : ScriptableObject
{
    public AnimationCurve distantWindSpeed;
    public AnimationCurve localWindSpeed;
    public Gradient refractingColor;
    public Gradient scatteringColor;
}
