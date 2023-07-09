#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DayNightCycle_LensFlare)), CanEditMultipleObjects]
public class DayNightCycle_LensFlareEditor : DayNightCycle_BaseListenerEditor
{
    public override void EditMode_Impl()
    {
    }
}
#endif
