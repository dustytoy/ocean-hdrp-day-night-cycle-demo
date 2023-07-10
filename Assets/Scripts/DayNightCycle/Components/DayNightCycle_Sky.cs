using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]

public class DayNightCycle_Sky : DayNightCycle_BaseComponent<PhysicallyBasedSky, DayNightCycle_SkySettingsSO>
{
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "Sky/";

    [Header("Handles")]
    public DayNightColor horizonTint;
    public DayNightColor zenithTint;

    [HideInInspector]
    public Volume volumn;

    public override void OnTimeChanged(long currentTick)
    {
        float t = MyTimePerDay.GetT(currentTick);
        component.horizonTint.Override(horizonTint.Evaluate(t));
        component.zenithTint.Override(zenithTint.Evaluate(t));
    }
    public override void OnStartPostProcess()
    {
        volumn = GetComponent<Volume>();
        volumn.profile.TryGet(out component);
    }

    public override void InitializeComponent(DayNightCycle_SkySettingsSO so)
    {
        horizonTint = new DayNightColor(so.horizonTint);
        zenithTint  = new DayNightColor(so.zenithTint);
    }

    public override string GetRelativeSubfolderPath()
    {
        return EDITOR_SETTINGS_SUBFOLDER;
    }

    public override string GetDefaultAssetName()
    {
        return "DayNightCycle_SkySettings_Default.asset";
    }
}
