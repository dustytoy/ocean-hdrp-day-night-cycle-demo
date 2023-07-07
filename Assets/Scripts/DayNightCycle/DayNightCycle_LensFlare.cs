using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(LensFlareComponentSRP))]
public class DayNightCycle_LensFlare : DayNightCycleListener
{
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "LensFlare/";

    [Header("Settings")]
    public DayNightCycle_LensFlareSettingsSO settings;
    public bool flareDuringDaytime;

    [Header("Handles")]
    public DayNightFloat intensity;
    public DayNightFloat scale;

    private LensFlareComponentSRP _lensFlare;
    public override void OnSunset(long currentTick)
    {
        _lensFlare.enabled = flareDuringDaytime ? false : true;
    }
    public override void OnSunrise(long currentTick)
    {
        _lensFlare.enabled = flareDuringDaytime ? true : false;
    }
    public override void OnTimeChanged(long currentTick)
    {
        float t = MyTimePerDay.GetT(currentTick);
        _lensFlare.intensity = intensity.Evaluate(t);
        _lensFlare.scale = scale.Evaluate(t);
    }
    public override void OnStartPostProcess()
    {
        // Load setting either from AssetDatabase or from AssetBundle
        if (settings == null)
        {
#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<DayNightCycle_LensFlareSettingsSO>(DayNightCycle.EDITOR_SETTINGS_FOLDER + EDITOR_SETTINGS_SUBFOLDER +
                "DayNightCycle_LensFlareSettings_Default.asset");
#else
            settings = DayNightCycle.Instance.loadedBundle.LoadAsset<DayNightCycle_LensFlareSettingsSO>("DayNightCycle_LensFlareSettings_Default");
#endif
        }

        Initialize(settings);

        _lensFlare = GetComponent<LensFlareComponentSRP>();
    }

    public void Initialize(DayNightCycle_LensFlareSettingsSO so)
    {
        intensity = new DayNightFloat(so.intensity);
        scale = new DayNightFloat(so.scale);
    }
}
