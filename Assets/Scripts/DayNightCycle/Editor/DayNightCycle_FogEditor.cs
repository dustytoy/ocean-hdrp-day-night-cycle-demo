#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Rendering.HighDefinition;

[CustomEditor(typeof(DayNightCycle_Fog)), CanEditMultipleObjects]
public class DayNightCycle_FogEditor : DayNightCycle_BaseComponentEditor<DayNightCycle_Fog, Fog, DayNightCycle_FogSettingsSO>
{
}
#endif
