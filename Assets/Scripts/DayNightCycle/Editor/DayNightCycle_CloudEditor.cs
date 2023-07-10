#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Rendering.HighDefinition;

[CustomEditor(typeof(DayNightCycle_Cloud)), CanEditMultipleObjects]
public class DayNightCycle_CloudEditor : DayNightCycle_BaseComponentEditor<DayNightCycle_Cloud, VolumetricClouds, DayNightCycle_CloudSettingsSO>
{
}
#endif
