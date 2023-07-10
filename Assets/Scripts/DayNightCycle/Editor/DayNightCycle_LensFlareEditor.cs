#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Rendering;

[CustomEditor(typeof(DayNightCycle_LensFlare)), CanEditMultipleObjects]
public class DayNightCycle_LensFlareEditor : DayNightCycle_BaseComponentEditor<DayNightCycle_LensFlare, LensFlareComponentSRP, DayNightCycle_LensFlareSettingsSO>
{
}
#endif
