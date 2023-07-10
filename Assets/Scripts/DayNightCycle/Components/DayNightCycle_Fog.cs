using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class DayNightCycle_Fog : DayNightCycle_BaseComponent<Fog, DayNightCycle_FogSettingsSO>
{
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "Fog/";

    [Header("Handles")]
    public DayNightFloat baseHeight;
    public DayNightFloat maxHeight;

    [HideInInspector]
    public Volume volumn;

    public override void OnTimeChanged(long currentTick)
    {
        float t = MyTimePerDay.GetT(currentTick);
        component.baseHeight.Override(baseHeight.Evaluate(t));
        component.maximumHeight.Override(maxHeight.Evaluate(t));
    }
    public override void OnStartPostProcess()
    {
        volumn = GetComponent<Volume>();
        volumn.profile.TryGet(out component);
    }

    public override void InitializeComponent(DayNightCycle_FogSettingsSO so)
    {
        baseHeight = new DayNightFloat(so.baseHeight);
        maxHeight = new DayNightFloat(so.maxHeight);
    }

    public override string GetRelativeSubfolderPath()
    {
        return EDITOR_SETTINGS_SUBFOLDER;
    }

    public override string GetDefaultAssetName()
    {
        return "DayNightCycle_FogSettings_Default";
    }
}
