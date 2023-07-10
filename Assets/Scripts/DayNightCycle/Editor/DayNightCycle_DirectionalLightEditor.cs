#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Rendering.HighDefinition;

[CustomEditor(typeof(DayNightCycle_DirectionalLight)), CanEditMultipleObjects]
public class DayNightCycle_DirectionalLightEditor : DayNightCycle_BaseComponentEditor<DayNightCycle_DirectionalLight, HDAdditionalLightData, DayNightCycle_DirectionalLightSettingsSO>
{
}
#endif
