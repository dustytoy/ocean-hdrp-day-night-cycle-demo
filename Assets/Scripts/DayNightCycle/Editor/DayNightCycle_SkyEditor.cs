#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DayNightCycle_Sky)), CanEditMultipleObjects]
public class DayNightCycle_SkyEditor : DayNightCycle_BaseListenerEditor
{
    private Editor _cachedSettingEditor;

    public override void EditMode_Impl()
    {
        var _target = target as DayNightCycle_Sky;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"SettingsSO:");
        _target.settings = EditorGUILayout.ObjectField(_target.settings, typeof(DayNightCycle_SkySettingsSO), false) as DayNightCycle_SkySettingsSO;
        EditorGUILayout.EndHorizontal();

        if (_target.settings != null)
        {
            CreateCachedEditor(_target.settings, null, ref _cachedSettingEditor);
            _cachedSettingEditor.OnInspectorGUI();
        }
    }
}
#endif
