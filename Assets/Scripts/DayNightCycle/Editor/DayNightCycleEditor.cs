#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DayNightCycle))]
public class DayNightCycleEditor : DayNightCycle_BaseEditor
{
    private Editor _cachedSettingEditor;

    public override void EditMode_Impl()
    {
        var _target = target as DayNightCycle;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Time Multiplier: {_target.timeMultiplier}");
        _target.timeMultiplier = EditorGUILayout.IntSlider(_target.timeMultiplier, 0, 1000);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Current Time: {(MyTimePerDay)_target.currentTick}");
        _target.currentTick = (long)(EditorGUILayout.Slider(MyTimePerDay.GetT(_target.currentTick), 0f, 1f) * MyTimePerDay.TicksPerDay);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"SettingsSO:");
        _target.settings = EditorGUILayout.ObjectField(_target.settings, typeof(DayNightCycle_SettingsSO), false) as DayNightCycle_SettingsSO;
        EditorGUILayout.EndHorizontal();

        if(_target.settings != null)
        {
            CreateCachedEditor(_target.settings, null, ref _cachedSettingEditor);
            _cachedSettingEditor.OnInspectorGUI();
        }
    }

    public void OnSceneGUI()
    {
    }
}
#endif
