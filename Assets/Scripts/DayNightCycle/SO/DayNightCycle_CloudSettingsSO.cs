using UnityEngine;

[CreateAssetMenu(fileName = "DayNightCycle_CloudSettings_Default", menuName = "DayNightCycle/Components/Cloud", order = 2)]
public class DayNightCycle_CloudSettingsSO : ScriptableObject
{
    public AnimationCurve ambientDimmer;
    public AnimationCurve lightDimmer;
    public Gradient scatteringTint;
}
