#if UNITY_EDITOR
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DayNightCycleController))]
public class DayNightCycleControllerEditor : DayNightCycle_BaseEditor
{
    public int componentIndex;
    public Editor cachedComponentEditor;
    private Vector2 scrollPosition;
    public override void EditMode_Impl()
    {
        int count = sharedController.components.Count;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < count; i++)
        {
            var c = sharedController.components[i];
            if (GUILayout.Button($"{c.name} ({c.GetType().Name})"))
            {
                componentIndex = i;
                Editor.CreateCachedEditor(c, System.Type.GetType($"{c.GetType().Name}Editor"), ref cachedComponentEditor);
            }
        }
        EditorGUILayout.EndScrollView();
        if(cachedComponentEditor != null)
        {
            System.Type.GetType($"{cachedComponentEditor.GetType().Name}").GetMethod("EditMode_Impl").Invoke(cachedComponentEditor, null);
        }
    }
    public void OnSceneGUI()
    {
        Handles.BeginGUI();
        var dayNightCycle = sharedDayNightCycle;
        GUI.color = Color.white;
        GUI.backgroundColor = Color.white;

        MyTimePerDay time = (MyTimePerDay)dayNightCycle.currentTick;
        GUI.Label(new Rect(50f, 10f, 200f, 30f), $"TimeMultiplier: {dayNightCycle.timeMultiplier}");
        dayNightCycle.timeMultiplier = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(250f, 10f, 300f, 30f), dayNightCycle.timeMultiplier, 1, 1000));
        GUI.Label(new Rect(50f, 40f, 200f, 30f), $"Time (t): {time} ({time.GetT()})");
        dayNightCycle.currentTick = (long)(GUI.HorizontalSlider(new Rect(250f, 40f, 300f, 30f), MyTimePerDay.GetT(dayNightCycle.currentTick), 0f, 1f) * MyTimePerDay.TicksPerDay);

        GUI.Label(new Rect(50f, 70f, 300f, 30f), $"SunRise: {dayNightCycle.settings.sunriseTime} | Sunset:{dayNightCycle.settings.sunsetTime}");
        Handles.EndGUI();

        // No OnSceneGUI yet
        //if (cachedComponentEditor != null)
        //{
        //    System.Type.GetType($"{cachedComponentEditor.GetType().Name}").GetMethod("OnSceneGUI").Invoke(cachedComponentEditor, null);
        //}
    }
}
#endif
