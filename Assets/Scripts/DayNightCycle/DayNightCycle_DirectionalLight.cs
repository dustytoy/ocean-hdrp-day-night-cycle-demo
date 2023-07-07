using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Light), typeof(HDAdditionalLightData), typeof(LensFlareComponentSRP))]
public class DayNightCycle_DirectionalLight : DayNightCycleListener
{
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "DirectionalLight/";

    [Header("Settings")]
    public DayNightCycle_DirectionalLightSettingsSO settings;
    public float rotateAngleOffset;
    public Vector3 rotateAxis;

    [Header("Handles")]
    public DayNightFloat angularDiameter;
    public DayNightFloat flareSize;
    public DayNightFloat flareFalloff;
    public DayNightFloat intensity;
    public DayNightFloat rotation;
    public DayNightColor flareTint;
    public DayNightColor surfaceTint;
    public DayNightColor emissionColor;

    private HDAdditionalLightData _hdData;

    public override void OnTimeChanged(long currentTick)
    {
        float t = MyTimePerDay.GetT(currentTick);
        Debug.Log(t);
        // HDRP Light
        _hdData.angularDiameter = angularDiameter.Evaluate(t);
        _hdData.flareSize = flareSize.Evaluate(t);
        _hdData.flareFalloff = flareFalloff.Evaluate(t);
        _hdData.intensity = intensity.Evaluate(t);
        _hdData.surfaceTint = surfaceTint.Evaluate(t);
        _hdData.color = emissionColor.Evaluate(t);

        // Rotation
        float angle = rotation.Evaluate(t) * 360f;
        transform.rotation = Quaternion.AngleAxis(-90f, Vector3.up) * Quaternion.AngleAxis(target.dayCount * 360f + rotateAngleOffset + angle, rotateAxis);
    }
    public override void OnStartPostProcess()
    {
        // Load setting either from AssetDatabase or from AssetBundle
        if (settings == null)
        {
#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<DayNightCycle_DirectionalLightSettingsSO>(DayNightCycle.EDITOR_SETTINGS_FOLDER + EDITOR_SETTINGS_SUBFOLDER +
                "DayNightCycle_DirectionalLightSettings_Default.asset");
#else
            settings = DayNightCycle.Instance.loadedBundle.LoadAsset<DayNightCycle_DirectionalLightSettingsSO>("DayNightCycle_DirectionalLightSettings_Default");
#endif
        }

        Initialize(settings);

        _hdData = GetComponent<HDAdditionalLightData>();
    }

    public void Initialize(DayNightCycle_DirectionalLightSettingsSO so)
    {
        rotateAngleOffset= so.rotateAngleOffset;
        rotateAxis = so.rotateAxis;

        angularDiameter = new DayNightFloat(so.angularDiameter);
        flareSize       = new DayNightFloat(so.flareSize);
        flareFalloff    = new DayNightFloat(so.flareFalloff);
        intensity       = new DayNightFloat(so.intensity);
        rotation        = new DayNightFloat(so.rotation);
        flareTint       = new DayNightColor(so.flareTint);
        surfaceTint     = new DayNightColor(so.surfaceTint);
        emissionColor   = new DayNightColor(so.emissionColor);
    }
}
