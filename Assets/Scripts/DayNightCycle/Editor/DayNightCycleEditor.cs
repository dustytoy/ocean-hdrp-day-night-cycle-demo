#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DayNightCycle))]
public class DayNightCycleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var editor = target as DayNightCycle;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Time Multiplier: {editor.timeMultiplier}");
        editor.timeMultiplier = EditorGUILayout.IntSlider(editor.timeMultiplier, 0, 1000);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Current Time: {(MyTimePerDay)editor.currentTick}");
        editor.currentTick = (long)(EditorGUILayout.Slider(MyTimePerDay.GetT(editor.currentTick), 0f, 1f) * MyTimePerDay.TicksPerDay);
        EditorGUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}
#endif
