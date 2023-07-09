#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DayNightCycle_Sky)), CanEditMultipleObjects]
public class DayNightCycle_SkyEditor : DayNightCycle_BaseListenerEditor
{
    public override void EditMode_Impl()
    {
    }
}
#endif
