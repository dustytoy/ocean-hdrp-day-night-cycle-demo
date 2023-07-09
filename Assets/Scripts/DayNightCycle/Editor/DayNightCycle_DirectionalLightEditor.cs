#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DayNightCycle_DirectionalLight)), CanEditMultipleObjects]
public class DayNightCycle_DirectionalLightEditor : DayNightCycle_BaseListenerEditor
{
    private Editor _cachedSettingEditor;

    public override void EditMode_Impl()
    {
        var _target = target as DayNightCycle_DirectionalLight;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"SettingsSO:");
        _target.settings = EditorGUILayout.ObjectField(_target.settings, typeof(DayNightCycle_DirectionalLightSettingsSO), false) as DayNightCycle_DirectionalLightSettingsSO;
        EditorGUILayout.EndHorizontal();

        if (_target.settings != null)
        {
            CreateCachedEditor(_target.settings, null, ref _cachedSettingEditor);
            _cachedSettingEditor.OnInspectorGUI();
        }
    }
}
#endif
