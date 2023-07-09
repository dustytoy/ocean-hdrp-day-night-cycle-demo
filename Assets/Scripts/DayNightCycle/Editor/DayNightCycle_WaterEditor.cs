#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DayNightCycle_Water)), CanEditMultipleObjects]
public class DayNightCycle_WaterEditor : DayNightCycle_BaseListenerEditor
{
    public override void EditMode_Impl()
    {
    }
}
#endif
