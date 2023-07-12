using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-1)]
public class DayNightCycleController : MonoBehaviour
{
    public static DayNightCycleController Instance
    {
        get { return _instance; }
    }
    private static DayNightCycleController _instance;

    [HideInInspector]
    public DayNightCycle dayNightCycle;
    public List<UnityEngine.Object> components;
    public bool displayGUI;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
        dayNightCycle = DayNightCycle.Instance;
        components = new List<UnityEngine.Object>();
    }

    private void OnGUI()
    {
        GUI.color = Color.white;
        GUI.backgroundColor = Color.white;

        if (displayGUI = GUI.Toggle(new Rect(10f, 0f, 50f, 50f), displayGUI, "GUI"))
        {
            MyTimePerDay time = (MyTimePerDay)dayNightCycle.currentTick;
            GUI.Label(new Rect(50f, 10f, 200f, 30f), $"TimeMultiplier: {dayNightCycle.timeMultiplier}");
            dayNightCycle.timeMultiplier = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(250f, 10f, 300f, 30f), dayNightCycle.timeMultiplier, 1, 1000));
            GUI.Label(new Rect(50f, 40f, 200f, 30f), $"Time (t): {time} ({time.GetT()})");
            dayNightCycle.currentTick = (long)(GUI.HorizontalSlider(new Rect(250f, 40f, 300f, 30f), MyTimePerDay.GetT(dayNightCycle.currentTick), 0f, 1f) * MyTimePerDay.TicksPerDay);

            GUI.Label(new Rect(50f, 70f, 500f, 30f), $"SunRise: {dayNightCycle.settings.sunriseTime} ({dayNightCycle.settings.sunriseTime.GetT()}) | " +
            $"Sunset:{dayNightCycle.settings.sunsetTime} ({dayNightCycle.settings.sunsetTime.GetT()})");
        }
    }
}
