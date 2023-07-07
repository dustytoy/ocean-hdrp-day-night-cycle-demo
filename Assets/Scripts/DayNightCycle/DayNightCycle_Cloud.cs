using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class DayNightCycle_Cloud : DayNightCycleListener
{
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "Cloud/";

    [Header("Settings")]
    public DayNightCycle_CloudSettingsSO settings;

    [Header("Handles")]
    public DayNightFloat ambientDimmer;
    public DayNightFloat lightDimmer;
    public DayNightColor scatteringTint;

    private Volume _volumn;
    private VolumetricClouds _cloud;
    private WindSpeedParameter.WindParamaterValue _overrideWindSpeed;

    public override void OnTimeChanged(long currentTick)
    {
        float t = MyTimePerDay.GetT(currentTick);
        _cloud.ambientLightProbeDimmer.Override(ambientDimmer.Evaluate(t));
        _cloud.sunLightDimmer.Override(lightDimmer.Evaluate(t));
        _cloud.scatteringTint.Override(scatteringTint.Evaluate(t));
    }
    public override void OnStartPostProcess()
    {
        if (settings == null)
        {
#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<DayNightCycle_CloudSettingsSO>(DayNightCycle.EDITOR_SETTINGS_FOLDER + EDITOR_SETTINGS_SUBFOLDER +
                "DayNightCycle_CloudSettings_Default.asset");
#else
            settings = DayNightCycle.Instance.loadedBundle.LoadAsset<DayNightCycle_CloudSettingsSO>("DayNightCycle_CloudSettings_Default");
#endif
        }

        Initialize(settings);

        _volumn = GetComponent<Volume>();
        _volumn.profile.TryGet(out _cloud);
        _overrideWindSpeed = new WindSpeedParameter.WindParamaterValue();

        target.onTimeMultiplierChanged += (x) =>
        {
            _overrideWindSpeed.customValue = x;
            _cloud.globalWindSpeed.Override(_overrideWindSpeed);
        };
    }

    public void Initialize(DayNightCycle_CloudSettingsSO so)
    {
        ambientDimmer = new DayNightFloat(so.ambientDimmer);
        lightDimmer = new DayNightFloat(so.lightDimmer);
        scatteringTint = new DayNightColor(so.scatteringTint);
    }
}
