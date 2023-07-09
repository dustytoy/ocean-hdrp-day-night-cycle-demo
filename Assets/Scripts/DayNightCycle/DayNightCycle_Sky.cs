using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]

public class DayNightCycle_Sky : DayNightCycle_BaseListener
{
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "Sky/";

    [Header("Settings")]
    public DayNightCycle_SkySettingsSO settings;

    [Header("Handles")]
    public DayNightColor horizonTint;
    public DayNightColor zenithTint;

    [HideInInspector]
    public Volume volumn;
    [HideInInspector]
    public PhysicallyBasedSky sky;

    public override void OnTimeChanged(long currentTick)
    {
        float t = MyTimePerDay.GetT(currentTick);
        sky.horizonTint.Override(horizonTint.Evaluate(t));
        sky.zenithTint.Override(zenithTint.Evaluate(t));
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
        
        volumn = GetComponent<Volume>();
        volumn.profile.TryGet(out sky);
    }

    public void Initialize(DayNightCycle_SkySettingsSO so)
    {
        horizonTint = new DayNightColor(so.horizonTint);
        zenithTint  = new DayNightColor(so.zenithTint);
    }
}
