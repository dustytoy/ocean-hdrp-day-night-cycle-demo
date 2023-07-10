#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Rendering.HighDefinition;

[CustomEditor(typeof(DayNightCycle_Sky)), CanEditMultipleObjects]
public class DayNightCycle_SkyEditor : DayNightCycle_BaseComponentEditor<DayNightCycle_Sky, PhysicallyBasedSky, DayNightCycle_SkySettingsSO>
{
}
#endif
