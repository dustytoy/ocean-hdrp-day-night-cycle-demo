#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DayNightCycle_Cloud)), CanEditMultipleObjects]
public class DayNightCycle_CloudEditor : DayNightCycle_BaseListenerEditor
{
    private Editor _cachedSettingEditor;

    public override void EditMode_Impl()
    {
        var _target = target as DayNightCycle_Cloud;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"SettingsSO:");
        _target.settings = EditorGUILayout.ObjectField(_target.settings, typeof(DayNightCycle_CloudSettingsSO), false) as DayNightCycle_CloudSettingsSO;
        EditorGUILayout.EndHorizontal();

        if (_target.settings != null)
        {
            CreateCachedEditor(_target.settings, null, ref _cachedSettingEditor);
            _cachedSettingEditor.OnInspectorGUI();
        }
    }
}
#endif
