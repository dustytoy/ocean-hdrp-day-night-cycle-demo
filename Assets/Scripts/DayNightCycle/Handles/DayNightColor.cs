using UnityEngine;

public class DayNightColor : DayNightHandle<Color>
{
    public Gradient gradient;
    public DayNightColor(Gradient gradient)
    {
        this.gradient = gradient;
        cachedValue = Color.white;
    }

    public override Color Evaluate(float t)
    {
        cachedValue = gradient.Evaluate(t);
        return cachedValue;
    }
}
