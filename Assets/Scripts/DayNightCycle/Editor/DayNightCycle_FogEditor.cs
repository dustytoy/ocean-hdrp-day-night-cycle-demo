#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DayNightCycle_Fog)), CanEditMultipleObjects]
public class DayNightCycle_FogEditor : DayNightCycle_BaseListenerEditor
{
    private Editor _cachedSettingEditor;

    public override void EditMode_Impl()
    {
        var _target = target as DayNightCycle_Fog;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"SettingsSO:");
        _target.settings = EditorGUILayout.ObjectField(_target.settings, typeof(DayNightCycle_FogSettingsSO), false) as DayNightCycle_FogSettingsSO;
        EditorGUILayout.EndHorizontal();

        if (_target.settings != null)
        {
            CreateCachedEditor(_target.settings, null, ref _cachedSettingEditor);
            _cachedSettingEditor.OnInspectorGUI();
        }
    }
}
#endif
