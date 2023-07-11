#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DayNightCycle))]
public class DayNightCycleEditor : DayNightCycle_BaseEditor
{
    private SerializedProperty _settings;
    private SerializedProperty _settingsName;
    private SerializedProperty _assetBundleName;
    private bool _disabledGroup;
    private DayNightCycle _target;
    public override void OnEnable_Impl()
    {
        _target = target as DayNightCycle;
        _settings = serializedObject.FindProperty("settings");
        _settingsName = serializedObject.FindProperty("settingsName");
        _assetBundleName = serializedObject.FindProperty("assetBundleName");
    }
    public override void OnInspectorGUI_Impl()
    {
        // Read Only
        EditorGUI.BeginChangeCheck();
        serializedObject.UpdateIfRequiredOrScript();
        _disabledGroup = true;
        EditorGUI.BeginDisabledGroup(_disabledGroup);
        EditorGUILayout.LabelField($"Time Multiplier: {_target.timeMultiplier}");
        EditorGUILayout.LabelField($"Current Time: {(MyTimePerDay)_target.currentTick} ({MyTimePerDay.GetT(_target.currentTick)})");
        EditorGUILayout.PropertyField(_settings);
        EditorGUI.EndDisabledGroup();

        // Fetch and Open assets
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(_settingsName);
        if (GUILayout.Button("Assign"))
        {
            string assetName = (_settingsName.stringValue == "Default" || _settingsName.stringValue == string.Empty) ?
                "DayNightCycleSettings_Default" : _settingsName.stringValue;
            _settings.objectReferenceValue = AssetDatabase.LoadAssetAtPath<DayNightCycle_SettingsSO>(DayNightCycle.EDITOR_SETTINGS_FOLDER +
                $"{assetName}.asset");
        }
        if (GUILayout.Button("Open"))
        {
            EditorUtility.OpenPropertyEditor(_target.settings);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(_assetBundleName);
        if(EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
