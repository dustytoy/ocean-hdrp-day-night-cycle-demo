#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DayNightCycle_DirectionalLight)), CanEditMultipleObjects]
public class DayNightCycle_DirectionalLightEditor : DayNightCycle_BaseListenerEditor
{
    public override void EditMode_Impl()
    {
    }
}
#endif
