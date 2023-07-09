#if UNITY_EDITOR

using UnityEngine.UIElements;

public abstract class DayNightCycle_BaseListenerEditor : DayNightCycle_BaseEditor
{
    public override VisualElement CreateInspectorGUI()
    {
        var listener = target as DayNightCycle_BaseListener;
        listener.target = sharedDayNightCycle;
        onEditModeOn += () =>
        {
            listener.OnStartEventSubscribe();
        };
        onEditModeOff += () =>
        {
            listener.OnDestroyEventUnsubscribe();
        };
        listener.OnStartPostProcess();
        return base.CreateInspectorGUI();
    }
}
#endif
