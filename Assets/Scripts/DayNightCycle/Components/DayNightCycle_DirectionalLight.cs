using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Light), typeof(HDAdditionalLightData))]
public class DayNightCycle_DirectionalLight : DayNightCycle_BaseComponent<HDAdditionalLightData, DayNightCycle_DirectionalLightSettingsSO>
{
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "DirectionalLight/";

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

    public override void OnTimeChanged(long currentTick)
    {
        float t = MyTimePerDay.GetT(currentTick);
        // HDRP Light
        component.angularDiameter = angularDiameter.Evaluate(t);
        component.flareSize = flareSize.Evaluate(t);
        component.flareFalloff = flareFalloff.Evaluate(t);
        component.intensity = intensity.Evaluate(t);
        component.surfaceTint = surfaceTint.Evaluate(t);
        component.color = emissionColor.Evaluate(t);

        // Rotation
        float angle = rotation.Evaluate(t) * 360f;
        transform.rotation = Quaternion.AngleAxis(-90f, Vector3.up) * Quaternion.AngleAxis(target.dayCount * 360f + rotateAngleOffset + angle, rotateAxis);
    }
    public override void OnStartPostProcess()
    {
        component = GetComponent<HDAdditionalLightData>();
    }

    public override void InitializeComponent(DayNightCycle_DirectionalLightSettingsSO so)
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

    public override string GetRelativeSubfolderPath()
    {
        return EDITOR_SETTINGS_SUBFOLDER;
    }

    public override string GetDefaultAssetName()
    {
        return "DayNightCycle_DirectionalLightSettings_Default.asset";
    }
}
