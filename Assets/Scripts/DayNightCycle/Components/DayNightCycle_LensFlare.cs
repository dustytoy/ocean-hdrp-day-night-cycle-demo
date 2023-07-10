using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(LensFlareComponentSRP))]
public class DayNightCycle_LensFlare : DayNightCycle_BaseComponent<LensFlareComponentSRP, DayNightCycle_LensFlareSettingsSO>
{
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "LensFlare/";

    [HideInInspector]
    public bool flareDuringDaytime;

    [Header("Handles")]
    public DayNightFloat intensity;
    public DayNightFloat scale;

    public override void OnSunset(long currentTick)
    {
        component.enabled = flareDuringDaytime ? false : true;
    }
    public override void OnSunrise(long currentTick)
    {
        component.enabled = flareDuringDaytime ? true : false;
    }
    public override void OnTimeChanged(long currentTick)
    {
        float t = MyTimePerDay.GetT(currentTick);
        component.intensity = intensity.Evaluate(t);
        component.scale = scale.Evaluate(t);
    }
    public override void OnStartPostProcess()
    {
        component = GetComponent<LensFlareComponentSRP>();
    }

    public override void InitializeComponent(DayNightCycle_LensFlareSettingsSO so)
    {
        intensity = new DayNightFloat(so.intensity);
        scale = new DayNightFloat(so.scale);
        flareDuringDaytime = so.flareDuringDayTime;
    }

    public override string GetRelativeSubfolderPath()
    {
        return EDITOR_SETTINGS_SUBFOLDER;
    }

    public override string GetDefaultAssetName()
    {
        return "DayNightCycle_LensFlareSettings_Default";
    }
}
