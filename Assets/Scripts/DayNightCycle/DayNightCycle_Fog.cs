using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class DayNightCycle_Fog : DayNightCycle_BaseListener
{
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "Fog/";

    [Header("Settings")]
    public DayNightCycle_FogSettingsSO settings;

    [Header("Handles")]
    public DayNightFloat baseHeight;
    public DayNightFloat maxHeight;

    [HideInInspector]
    public Volume volumn;
    [HideInInspector]
    public Fog fog;

    public override void OnTimeChanged(long currentTick)
    {
        float t = MyTimePerDay.GetT(currentTick);
        fog.baseHeight.Override(baseHeight.Evaluate(t));
        fog.maximumHeight.Override(maxHeight.Evaluate(t));
    }
    public override void OnStartPostProcess()
    {
        if (settings == null)
        {
#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<DayNightCycle_FogSettingsSO>(DayNightCycle.EDITOR_SETTINGS_FOLDER + EDITOR_SETTINGS_SUBFOLDER +
                "DayNightCycle_FogSettings_Default.asset");
#else
            settings = DayNightCycle.Instance.loadedBundle.LoadAsset<DayNightCycle_FogSettingsSO>("DayNightCycle_FogSettings_Default");
#endif
        }

        Initialize(settings);

        volumn = GetComponent<Volume>();
        volumn.profile.TryGet(out fog);
    }

    public void Initialize(DayNightCycle_FogSettingsSO so)
    {
        baseHeight = new DayNightFloat(so.baseHeight);
        maxHeight = new DayNightFloat(so.maxHeight);
    }
}
