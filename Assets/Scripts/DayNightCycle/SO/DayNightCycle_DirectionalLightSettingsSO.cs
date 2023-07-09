using UnityEngine;

[CreateAssetMenu(fileName = "DayNightCycle_DirectionalLightSettings_Default", menuName = "DayNightCycle/Components/DirectionalLight", order = 0)]
public class DayNightCycle_DirectionalLightSettingsSO : ScriptableObject
{
    public float rotateAngleOffset;
    public Vector3 rotateAxis;

    public AnimationCurve angularDiameter;
    public AnimationCurve flareSize;
    public AnimationCurve flareFalloff;
    public AnimationCurve intensity;
    public AnimationCurve rotation;
    public Gradient flareTint;
    public Gradient surfaceTint;
    public Gradient emissionColor;
}
