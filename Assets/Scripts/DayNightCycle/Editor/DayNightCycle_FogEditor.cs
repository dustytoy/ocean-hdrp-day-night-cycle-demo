#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DayNightCycle_Fog)), CanEditMultipleObjects]
public class DayNightCycle_FogEditor : DayNightCycle_BaseListenerEditor
{
    public override void EditMode_Impl()
    {
    }
}
#endif
