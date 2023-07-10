using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class DayNightCycle_Cloud : DayNightCycle_BaseComponent<VolumetricClouds, DayNightCycle_CloudSettingsSO>
{
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "Cloud/";

    [Header("Handles")]
    public DayNightFloat ambientDimmer;
    public DayNightFloat lightDimmer;
    public DayNightColor scatteringTint;

    [HideInInspector]
    public Volume volumn;
    [HideInInspector]
    public WindSpeedParameter.WindParamaterValue overrideWindSpeed;

    public override void OnTimeChanged(long currentTick)
    {
        float t = MyTimePerDay.GetT(currentTick);
        component.ambientLightProbeDimmer.Override(ambientDimmer.Evaluate(t));
        component.sunLightDimmer.Override(lightDimmer.Evaluate(t));
        component.scatteringTint.Override(scatteringTint.Evaluate(t));
    }
    public override void OnStartPostProcess()
    {
        volumn = GetComponent<Volume>();
        volumn.profile.TryGet(out component);
        overrideWindSpeed = new WindSpeedParameter.WindParamaterValue();

        target.onTimeMultiplierChanged += (x) =>
        {
            overrideWindSpeed.customValue = x;
            component.globalWindSpeed.Override(overrideWindSpeed);
        };
    }

    public override void InitializeComponent(DayNightCycle_CloudSettingsSO so)
    {
        ambientDimmer = new DayNightFloat(so.ambientDimmer);
        lightDimmer = new DayNightFloat(so.lightDimmer);
        scatteringTint = new DayNightColor(so.scatteringTint);
    }

    public override string GetRelativeSubfolderPath()
    {
        return EDITOR_SETTINGS_SUBFOLDER;
    }

    public override string GetDefaultAssetName()
    {
        return "DayNightCycle_CloudSettings_Default";
    }
}
