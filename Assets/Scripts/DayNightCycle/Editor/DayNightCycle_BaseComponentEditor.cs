#if UNITY_EDITOR

using System.Reflection;
using UnityEditor;
using UnityEngine;

public abstract class DayNightCycle_BaseComponentEditor<D,T,U> : DayNightCycle_BaseEditor where D : DayNightCycle_BaseComponent<T, U> where T : UnityEngine.Object where U : UnityEngine.ScriptableObject
{
    protected string cloneName = "Clone";
    protected SerializedProperty component;
    protected SerializedProperty settings;
    protected SerializedProperty settingsName;

    private D _target;
    private bool _disabledGroup;

    public override void OnEnable_Impl()
    {
        component = serializedObject.FindProperty("component");
        settings = serializedObject.FindProperty("settings");
        settingsName = serializedObject.FindProperty("settingsName");

        _target = target as D;
        _target.dayNightCycle = sharedDayNightCycle;
        _target.controller = sharedController;
        onEditModeOn += () =>
        {
            _target.OnStartEventSubscribe();
        };
        onEditModeOff += () =>
        {
            _target.OnDestroyEventUnsubscribe();
        };
        InitializeSettingsEditorMode();
    }

    public override void OnInspectorGUI_Impl()
    {
        // Read Only
        EditorGUI.BeginChangeCheck();
        serializedObject.UpdateIfRequiredOrScript();

        _disabledGroup = true;
        EditorGUI.BeginDisabledGroup(_disabledGroup);
        EditorGUILayout.PropertyField(component);
        EditorGUILayout.PropertyField(settings);
        EditorGUI.EndDisabledGroup();

        // Assign, Initialize, Open assets
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(settingsName);
        if (GUILayout.Button("Assign & Initialize"))
        {
            string assetName = (settingsName.stringValue == "Default" || settingsName.stringValue == string.Empty) ?
                "DayNightCycleSettings_Default" : settingsName.stringValue;
            settings.objectReferenceValue = AssetDatabase.LoadAssetAtPath<DayNightCycle_SettingsSO>(DayNightCycle.EDITOR_SETTINGS_FOLDER +
                $"{assetName}.asset");
            InitializeSettingsEditorMode();
        }
        if(GUILayout.Button("Initialize"))
        {
            InitializeSettingsEditorMode();
        }
        if (GUILayout.Button("Open"))
        {
            EditorUtility.OpenPropertyEditor(_target.settings);
        }
        EditorGUILayout.EndHorizontal();

        // 
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"CloneName:");
        cloneName = EditorGUILayout.TextField(cloneName);
        if(GUILayout.Button("Clone Settings"))
        {
            U clone = Instantiate(_target.settings);
            string assetName = cloneName == string.Empty ? "Clone" : cloneName;
            string path = DayNightCycle.EDITOR_SETTINGS_FOLDER + _target.GetRelativeSubfolderPath() + assetName + ".asset";
            AssetDatabase.CreateAsset(clone, path);
            _target.settingsName = cloneName;
            _target.settings = clone;
        }
        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    public void InitializeSettingsEditorMode()
    {
        _target.InitializeSettings();
        _target.InitializeComponent(_target.settings);
        _target.OnStartPostProcess();
    }
    
}
#endif
