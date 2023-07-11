using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(WaterSurface))]
public class DayNightCycle_Water : DayNightCycle_BaseComponent<WaterSurface, DayNightCycle_WaterSettingsSO>
{
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "Water/";

    [Header("Handles")]
    public DayNightFloat distantWindSpeed;
    public DayNightFloat localWindSpeed;
    public DayNightColor refractingColor;
    public DayNightColor scatteringColor;

    public override void OnTimeChanged(long currentTick)
    {
        float t = MyTimePerDay.GetT(currentTick);

        component.largeWindSpeed   = distantWindSpeed.Evaluate(t);
        component.ripplesWindSpeed = localWindSpeed.Evaluate(t);
        component.refractionColor  = refractingColor.Evaluate(t);
        component.scatteringColor  = scatteringColor.Evaluate(t);
    }
    public override void OnStartPostProcess()
    {
        component = GetComponent<WaterSurface>();

        dayNightCycle.onTimeMultiplierChanged += (x) =>
        {
            component.timeMultiplier = x;
        };
    }

    public override void InitializeComponent(DayNightCycle_WaterSettingsSO so)
    {
        distantWindSpeed = new DayNightFloat(so.distantWindSpeed);
        localWindSpeed = new DayNightFloat(so.localWindSpeed);
        refractingColor = new DayNightColor(so.refractingColor);
        scatteringColor = new DayNightColor(so.scatteringColor);
    }

    public override string GetRelativeSubfolderPath()
    {
        return EDITOR_SETTINGS_SUBFOLDER;
    }

    public override string GetDefaultAssetName()
    {
        return "DayNightCycle_WaterSettings_Default";
    }
}
