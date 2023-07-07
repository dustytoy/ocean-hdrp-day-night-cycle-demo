using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(WaterSurface))]
public class DayNightCycle_Water : DayNightCycleListener
{
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "Water/";

    [Header("Settings")]
    public DayNightCycle_WaterSettingsSO settings;

    [Header("Handles")]
    public DayNightFloat distantWindSpeed;
    public DayNightFloat localWindSpeed;
    public DayNightColor refractingColor;
    public DayNightColor scatteringColor;

    private WaterSurface _water;

    public override void OnTimeChanged(long currentTick)
    {
        float t = MyTimePerDay.GetT(currentTick);

        _water.largeWindSpeed   = distantWindSpeed.Evaluate(t);
        _water.ripplesWindSpeed = localWindSpeed.Evaluate(t);
        _water.refractionColor  = refractingColor.Evaluate(t);
        _water.scatteringColor  = scatteringColor.Evaluate(t);
    }
    public override void OnStartPostProcess()
    {
        if (settings == null)
        {
#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<DayNightCycle_WaterSettingsSO>(DayNightCycle.EDITOR_SETTINGS_FOLDER + EDITOR_SETTINGS_SUBFOLDER +
                "DayNightCycle_WaterSettings_Default.asset");
#else
            settings = DayNightCycle.Instance.loadedBundle.LoadAsset<DayNightCycle_WaterSettingsSO>("DayNightCycle_WaterSettings_Default");
#endif
        }

        Initialize(settings);

        _water = GetComponent<WaterSurface>();

        target.onTimeMultiplierChanged += (x) =>
        {
            _water.timeMultiplier = x;
        };
    }

    public void Initialize(DayNightCycle_WaterSettingsSO so)
    {
        distantWindSpeed = new DayNightFloat(so.distantWindSpeed);
        localWindSpeed = new DayNightFloat(so.localWindSpeed);
        refractingColor = new DayNightColor(so.refractingColor);
        scatteringColor = new DayNightColor(so.scatteringColor);
    }
}
