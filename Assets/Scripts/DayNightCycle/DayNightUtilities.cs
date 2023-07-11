using UnityEngine;

public static class DayNightUtilities
{
    public static void AjustKeyframes(AnimationCurve curve, ref float[] timeReadOnly, bool preserveCurve)
    {
        if(preserveCurve)
        {
            var keys = new Keyframe[timeReadOnly.Length];
            for (int i = 0; i < timeReadOnly.Length; i++)
            {
                float t = timeReadOnly[i];
                keys[i] = new Keyframe(t, curve.Evaluate(t));
            }
            curve.ClearKeys();
            curve.keys = keys;
        }
        else
        {
            for (int i = 0; i < timeReadOnly.Length; i++)
            {
                curve.keys[i].time = timeReadOnly[i];
            }
        }
        
    }
    public static void AdjustKeyframes(Gradient gradient, ref float[] timeReadOnlyEvenIndex, bool preserveGradient, bool alphaAlso = false)
    {
        if (preserveGradient)
        {
            var alphas = alphaAlso ? 
                new GradientAlphaKey[timeReadOnlyEvenIndex.Length] : 
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) };
            var colors = new GradientColorKey[timeReadOnlyEvenIndex.Length];

            for (int i = 0; i < timeReadOnlyEvenIndex.Length; i+=2)
            {
                float t = timeReadOnlyEvenIndex[i];
                if (alphaAlso)
                {
                    var alphaKey = new GradientAlphaKey(gradient.Evaluate(t).a, t);
                    alphas[i] = alphaKey;
                }
                var colorKey = new GradientColorKey(gradient.Evaluate(t), t);
                colors[i] = colorKey;
            }
            gradient.SetKeys(colors, alphas);
        }
        else
        {
            for (int i = 0; i < timeReadOnlyEvenIndex.Length; i += 2)
            {
                float t = timeReadOnlyEvenIndex[i];
                gradient.colorKeys[i].time = t;
                if(alphaAlso)
                {
                    gradient.alphaKeys[i].time = t;
                }
            }
        }
    }
}
