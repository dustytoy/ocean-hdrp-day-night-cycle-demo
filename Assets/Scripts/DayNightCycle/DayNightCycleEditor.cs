#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DayNightCycle))]
public class DayNightCycleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var editor = target as DayNightCycle;
        var start = editor.config.startTime;
        var rise = editor.config.sunRiseTime;
        var set = editor.config.sunSetTime;
        GUILayout.Label($"Start: {start}(t={start.GetT()})\tSunRise: {rise}(t={rise.GetT()})\tSunSet: {set}(t={set.GetT()})\tEnd: 24:00:00(t=1)");
        base.OnInspectorGUI();
    }
}
#endif
