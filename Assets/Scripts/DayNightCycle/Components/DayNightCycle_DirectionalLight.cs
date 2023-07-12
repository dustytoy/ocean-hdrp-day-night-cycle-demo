using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Light), typeof(HDAdditionalLightData))]
public class DayNightCycle_DirectionalLight : DayNightCycle_BaseComponent<HDAdditionalLightData, DayNightCycle_DirectionalLightSettingsSO>
{
    public enum ShadowMode
    {
        NoShadow,
        ShadowDayTime,
        ShadowNightTime
    }
    public static readonly string EDITOR_SETTINGS_SUBFOLDER = "DirectionalLight/";
    [HideInInspector]
    public float rotateAngleOffset;
    [HideInInspector]
    public Vector3 rotateAxis;
    [HideInInspector]
    public ShadowMode shadowMode;

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
        component.flareTint = flareTint.Evaluate(t);
        component.flareFalloff = flareFalloff.Evaluate(t);
        component.intensity = intensity.Evaluate(t);
        component.surfaceTint = surfaceTint.Evaluate(t);
        component.color = emissionColor.Evaluate(t);

        // Rotation
        float angle = rotation.Evaluate(t) * 360f;
        transform.rotation = Quaternion.AngleAxis(-90f, Vector3.up) * Quaternion.AngleAxis(dayNightCycle.dayCount * 360f + rotateAngleOffset + angle, rotateAxis);
    }
    public override void OnStartPostProcess()
    {
        component = GetComponent<HDAdditionalLightData>();

        switch (shadowMode)
        {
            case ShadowMode.ShadowDayTime:
                {
                    component.EnableShadows(dayNightCycle.isDayTime == true ? true : false);
                    break;
                }
            case ShadowMode.ShadowNightTime:
                {
                    component.EnableShadows(dayNightCycle.isDayTime == true ? false : true);
                    break;
                }
            case ShadowMode.NoShadow:
                {
                    component.EnableShadows(false);
                    break;
                }
            default:
                break;
        }
    }

    public override void OnSunset(long currentTick)
    {
        switch(shadowMode)
        {
            case ShadowMode.ShadowDayTime:
                {
                    component.EnableShadows(false);
                    break;
                }
            case ShadowMode.ShadowNightTime:
                {
                    component.EnableShadows(true);
                    break;
                }
            case ShadowMode.NoShadow:
                {
                    component.EnableShadows(false);
                    break;
                }
            default:
                break;
        }
    }
    public override void OnSunrise(long currentTick)
    {
        switch (shadowMode)
        {
            case ShadowMode.ShadowDayTime:
                {
                    component.EnableShadows(true);
                    break;
                }
            case ShadowMode.ShadowNightTime:
                {
                    component.EnableShadows(false);
                    break;
                }
            case ShadowMode.NoShadow:
                {
                    component.EnableShadows(false);
                    break;
                }
            default:
                break;
        }
    }

    public override void InitializeComponent(DayNightCycle_DirectionalLightSettingsSO so)
    {
        rotateAngleOffset   = so.rotateAngleOffset;
        rotateAxis          = so.rotateAxis;
        shadowMode          = so.shadowMode;
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
        return "DayNightCycle_DirectionalLightSettings_Default";
    }
}
