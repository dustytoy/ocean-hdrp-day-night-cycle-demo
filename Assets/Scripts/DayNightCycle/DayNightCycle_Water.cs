using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(WaterSurface))]
public class DayNightCycle_Water : DayNightCycle_BaseListener
{
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "Water/";

    [Header("Settings")]
    public DayNightCycle_WaterSettingsSO settings;

    [Header("Handles")]
    public DayNightFloat distantWindSpeed;
    public DayNightFloat localWindSpeed;
    public DayNightColor refractingColor;
    public DayNightColor scatteringColor;

    [HideInInspector]
    public WaterSurface water;

    public override void OnTimeChanged(long currentTick)
    {
        float t = MyTimePerDay.GetT(currentTick);

        water.largeWindSpeed   = distantWindSpeed.Evaluate(t);
        water.ripplesWindSpeed = localWindSpeed.Evaluate(t);
        water.refractionColor  = refractingColor.Evaluate(t);
        water.scatteringColor  = scatteringColor.Evaluate(t);
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

        water = GetComponent<WaterSurface>();

        target.onTimeMultiplierChanged += (x) =>
        {
            water.timeMultiplier = x;
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
