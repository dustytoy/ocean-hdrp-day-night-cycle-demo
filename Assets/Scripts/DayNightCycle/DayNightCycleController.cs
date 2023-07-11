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
            dayNightCycle.timeMultiplier = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(10f, 50f, 600f, 50f), dayNightCycle.timeMultiplier, 1, 1000));
            dayNightCycle.currentTick = (long)(GUI.HorizontalSlider(new Rect(10f, 100f, 600f, 50f), MyTimePerDay.GetT(dayNightCycle.currentTick), 0f, 1f) * MyTimePerDay.TicksPerDay);

            MyTimePerDay time = (MyTimePerDay)dayNightCycle.currentTick;
            GUI.Label(new Rect(10f, 150f, 600f, 100f), $"SunRise: {dayNightCycle.settings.sunriseTime} | Sunset:{dayNightCycle.settings.sunsetTime}\n" +
                $"Time (t): {time} ({time.GetT()})\n" +
                $"TimeMultiplier: {dayNightCycle.timeMultiplier}\n");
        }
    }
}
