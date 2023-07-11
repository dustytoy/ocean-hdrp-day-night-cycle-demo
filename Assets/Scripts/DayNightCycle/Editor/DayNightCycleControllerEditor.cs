#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DayNightCycleController))]
public class DayNightCycleControllerEditor : DayNightCycle_BaseEditor
{
    private SerializedProperty _components;
    private SerializedProperty _displayGUI;
    private bool _enableEdit;
    private DayNightCycleController _target;
    private Type _selectedComponentType;
    private Type _selectedEditorType;
    private Editor _selectedEditor;

    public override void OnEnable_Impl()
    {
        _components = serializedObject.FindProperty("components");
        _displayGUI = serializedObject.FindProperty("displayGUI");
        _target = target as DayNightCycleController;
    }
    public override void OnInspectorGUI_Impl()
    {
        // Read Only
        EditorGUI.BeginChangeCheck();
        serializedObject.UpdateIfRequiredOrScript();

        EditorGUILayout.BeginHorizontal();
        _enableEdit = EditorGUILayout.Toggle(_enableEdit);
        EditorGUI.BeginDisabledGroup(!_enableEdit);
        EditorGUILayout.PropertyField(_components);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        // Manage components editors
        EditorGUILayout.PropertyField(_displayGUI);
        int count = _components.arraySize;

        // Selection buttons
        for (int i = 0; i < count; i++)
        {
            var c = _components.GetArrayElementAtIndex(i);
            if (GUILayout.Button($"{c.objectReferenceValue.name} ({c.objectReferenceValue.GetType().Name.Substring(14)})"))
            {
                _selectedComponentType= c.objectReferenceValue.GetType();
                _selectedEditorType = Type.GetType($"{_selectedComponentType.Name}Editor");
                Editor.CreateCachedEditor(c.objectReferenceValue, _selectedEditorType, ref _selectedEditor);
            }
        }

        // Selected editor
        if (_selectedEditor != null)
        {
            ((DayNightCycle_BaseEditor)_selectedEditor).OnInspectorGUI_Impl();
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    public override void OnSceneGUI_Impl()
    {
        if (_displayGUI.boolValue == false) { return; }

        Handles.BeginGUI();
        var dayNightCycle = sharedDayNightCycle;
        GUI.color = Color.white;
        GUI.backgroundColor = Color.white;

        MyTimePerDay time = (MyTimePerDay)dayNightCycle.currentTick;
        GUI.Label(new Rect(50f, 10f, 200f, 30f), $"TimeMultiplier: {dayNightCycle.timeMultiplier}");
        dayNightCycle.timeMultiplier = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(250f, 10f, 300f, 30f), dayNightCycle.timeMultiplier, 1, 1000));
        GUI.Label(new Rect(50f, 40f, 200f, 30f), $"Time (t): {time} ({time.GetT()})");
        dayNightCycle.currentTick = (long)(GUI.HorizontalSlider(new Rect(250f, 40f, 300f, 30f), MyTimePerDay.GetT(dayNightCycle.currentTick), 0f, 1f) * MyTimePerDay.TicksPerDay);

        GUI.Label(new Rect(50f, 70f, 500f, 30f), $"SunRise: {dayNightCycle.settings.sunriseTime} ({dayNightCycle.settings.sunriseTime.GetT()}) | " +
            $"Sunset:{dayNightCycle.settings.sunsetTime} ({dayNightCycle.settings.sunsetTime.GetT()})");

        // Selected editor
        if (_selectedEditor != null)
        {
            ((DayNightCycle_BaseEditor)_selectedEditor).OnSceneGUI_Impl();
        }

        Handles.EndGUI();
    }
}
#endif
