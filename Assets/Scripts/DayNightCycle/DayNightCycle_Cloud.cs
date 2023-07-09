using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class DayNightCycle_Cloud : DayNightCycle_BaseListener
{
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "Cloud/";

    [Header("Settings")]
    public DayNightCycle_CloudSettingsSO settings;

    [Header("Handles")]
    public DayNightFloat ambientDimmer;
    public DayNightFloat lightDimmer;
    public DayNightColor scatteringTint;

    [HideInInspector]
    public Volume volumn;
    [HideInInspector]
    public VolumetricClouds cloud;
    [HideInInspector]
    public WindSpeedParameter.WindParamaterValue overrideWindSpeed;

    public override void OnTimeChanged(long currentTick)
    {
        float t = MyTimePerDay.GetT(currentTick);
        cloud.ambientLightProbeDimmer.Override(ambientDimmer.Evaluate(t));
        cloud.sunLightDimmer.Override(lightDimmer.Evaluate(t));
        cloud.scatteringTint.Override(scatteringTint.Evaluate(t));
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

        volumn = GetComponent<Volume>();
        volumn.profile.TryGet(out cloud);
        overrideWindSpeed = new WindSpeedParameter.WindParamaterValue();

        target.onTimeMultiplierChanged += (x) =>
        {
            overrideWindSpeed.customValue = x;
            cloud.globalWindSpeed.Override(overrideWindSpeed);
        };
    }

    public void Initialize(DayNightCycle_CloudSettingsSO so)
    {
        ambientDimmer = new DayNightFloat(so.ambientDimmer);
        lightDimmer = new DayNightFloat(so.lightDimmer);
        scatteringTint = new DayNightColor(so.scatteringTint);
    }
}
