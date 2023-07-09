#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DayNightCycle_LensFlare)), CanEditMultipleObjects]
public class DayNightCycle_LensFlareEditor : DayNightCycle_BaseListenerEditor
{
    private Editor _cachedSettingEditor;

    public override void EditMode_Impl()
    {
        var _target = target as DayNightCycle_LensFlare;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"SettingsSO:");
        _target.settings = EditorGUILayout.ObjectField(_target.settings, typeof(DayNightCycle_LensFlareSettingsSO), false) as DayNightCycle_LensFlareSettingsSO;
        EditorGUILayout.EndHorizontal();

        if (_target.settings != null)
        {
            CreateCachedEditor(_target.settings, null, ref _cachedSettingEditor);
            _cachedSettingEditor.OnInspectorGUI();
        }
    }
}
#endif
