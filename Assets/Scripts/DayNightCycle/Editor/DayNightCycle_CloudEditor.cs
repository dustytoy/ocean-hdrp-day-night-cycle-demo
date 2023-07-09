#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DayNightCycle_Cloud)), CanEditMultipleObjects]
public class DayNightCycle_CloudEditor : DayNightCycle_BaseListenerEditor
{
    public override void EditMode_Impl()
    {
    }
}
#endif
