using UnityEngine;

public class DayNightFloat : DayNightHandle<float>
{
    public AnimationCurve curve;
    public DayNightFloat(AnimationCurve curve)
    {
        this.curve = curve;
        cachedValue = 0f;
    }

    public override float Evaluate(float t)
    {
        cachedValue = curve.Evaluate(t);
        return cachedValue;
    }
}
