using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DayNightCycle_LensFlareSettings_Default", menuName = "DayNightCycle/Components/LensFlare", order = 5)]
public class DayNightCycle_LensFlareSettingsSO : ScriptableObject
{
    public AnimationCurve intensity;
    public AnimationCurve scale;
}
