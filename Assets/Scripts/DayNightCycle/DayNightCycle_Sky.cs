using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]

public class DayNightCycle_Sky : DayNightCycleListener
{
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "Sky/";

    [Header("Settings")]
    public DayNightCycle_SkySettingsSO settings;

    [Header("Handles")]
    public DayNightColor horizonTint;
    public DayNightColor zenithTint;

    private Volume _volumn;
    private PhysicallyBasedSky _sky;

    public override void OnTimeChanged(long currentTick)
    {
        float t = MyTimePerDay.GetT(currentTick);
        _sky.horizonTint.Override(horizonTint.Evaluate(t));
        _sky.zenithTint.Override(zenithTint.Evaluate(t));
    }
    public override void OnStartPostProcess()
    {
        if (settings == null)
        {
#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<DayNightCycle_SkySettingsSO>(DayNightCycle.EDITOR_SETTINGS_FOLDER + EDITOR_SETTINGS_SUBFOLDER +
                "DayNightCycle_SkySettings_Default.asset");
#else
            settings = DayNightCycle.Instance.loadedBundle.LoadAsset<DayNightCycle_SkySettingsSO>("DayNightCycle_SkySettings_Default");
#endif
        }

        Initialize(settings);
        
        _volumn = GetComponent<Volume>();
        _volumn.profile.TryGet(out _sky);
    }

    public void Initialize(DayNightCycle_SkySettingsSO so)
    {
        horizonTint = new DayNightColor(so.horizonTint);
        zenithTint  = new DayNightColor(so.zenithTint);
    }
}
