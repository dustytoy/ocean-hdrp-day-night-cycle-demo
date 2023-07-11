#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DayNightCycleController))]
public class DayNightCycleControllerEditor : DayNightCycle_BaseEditor
{
    public int componentIndex;
    public override void EditMode_Impl()
    {
        int count = sharedController.components.Count;

        for (int i = 0; i < count; i++)
        {
            var c = sharedController.components[i];
            if (GUILayout.Button($"{c.name} ({c.GetType()})"))
            {
                componentIndex = i;
                EditorUtility.OpenPropertyEditor(c);
            }
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

        GUI.Label(new Rect(50f, 70f, 500f, 30f), $"SunRise: {dayNightCycle.settings.sunriseTime} ({dayNightCycle.settings.sunriseTime.GetT()}) | " +
            $"Sunset:{dayNightCycle.settings.sunsetTime} ({dayNightCycle.settings.sunsetTime.GetT()})");
        Handles.EndGUI();

        // No OnSceneGUI yet
        //if (cachedComponentEditor != null)
        //{
        //    System.Type.GetType($"{cachedComponentEditor.GetType().Name}").GetMethod("OnSceneGUI").Invoke(cachedComponentEditor, null);
        //}
    }
}
#endif
