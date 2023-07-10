#if UNITY_EDITOR

using UnityEditor;
using UnityEngine.UIElements;

public abstract class DayNightCycle_BaseComponentEditor<D,T,U> : DayNightCycle_BaseEditor where D : DayNightCycle_BaseComponent<T, U> where T : UnityEngine.Object where U : UnityEngine.ScriptableObject
{
    protected Editor cachedSettingEditor;

    public override VisualElement CreateInspectorGUI()
    {
        var component = target as D;
        component.target = sharedDayNightCycle;
        onEditModeOn += () =>
        {
            component.OnStartEventSubscribe();
        };
        onEditModeOff += () =>
        {
            component.OnDestroyEventUnsubscribe();
        };
        component.InitializeSettings();
        component.InitializeComponent(component.settings);
        component.OnStartPostProcess();
        return base.CreateInspectorGUI();
    }

    public override void EditMode_Impl()
    {
        var _target = target as D;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"SettingsSO:");
        _target.settings = EditorGUILayout.ObjectField(_target.settings, typeof(U), false) as U;
        EditorGUILayout.EndHorizontal();

        if (_target.settings != null)
        {
            CreateCachedEditor(_target.settings, null, ref cachedSettingEditor);
            cachedSettingEditor.OnInspectorGUI();
        }
    }
}
#endif
