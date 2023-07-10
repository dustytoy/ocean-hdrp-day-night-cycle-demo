#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Rendering.HighDefinition;

[CustomEditor(typeof(DayNightCycle_Water)), CanEditMultipleObjects]
public class DayNightCycle_WaterEditor : DayNightCycle_BaseComponentEditor<DayNightCycle_Water, WaterSurface, DayNightCycle_WaterSettingsSO>
{
}
#endif
