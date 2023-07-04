using System;
using UnityEngine;

public partial class DayNightCycle : MonoBehaviour
{
    [Serializable]
    public struct DayNightConfig
    {
        public MyTime startTime;
        public MyTime sunRiseTime;
        public MyTime sunSetTime;
        public LightConfig sunLight;
        public LightConfig moonLight;
        public SkyConfig sky;
        public CloudConfig cloud;
    }

    // Per half day. 1 sun and 1 moon
    [Serializable]
    public struct LightConfig
    {
        public CelestialBody celestialBody;
        public Emission lightEmission;
        [Serializable]
        public struct CelestialBody
        {
            public float maxAngularDiameter;
            public float minAngularDiameter;
            public float maxFlareSize;
            public float minFlareSize;
            public float maxFlareFalloff;
            public float minFlareFalloff;
            public Color sunRiseFlareTint;
            public Color peakFlareTint;
            public Color sunSetFlareTint;
            public Color sunRiseSurfaceTint;
            public Color peakSurfaceTint;
            public Color sunSetSurfaceTint;

        }
        [Serializable]
        public struct Emission
        {
            public float maxIntensity;
            public float minIntensity;
            public Color sunRiseEmissionColor;
            public Color peakEmissionColor;
            public Color sunSetEmissionColor;

        }
    }

    [Serializable]
    public struct SkyConfig
    {
        public ArtOverride artisticOverrides;
        [Serializable]
        public struct ArtOverride
        {
            public Color dayZenithTint;
            public Color nightZenithTint;

            public Color sunRiseHorizonTint;
            public Color dayHorizonTint;
            public Color sunSetHorizonTint;
            public Color nightHorizonTint;
        }
    }

    [Serializable]
    public struct CloudConfig
    {
        public Lighting lighting;
        [Serializable]
        public struct Lighting
        {
            public float maxAmbientDimmer;
            public float minAmbientDimmer;
            public float maxLightDimmer;
            public float minLightDimmer;
            public Color sunRiseScatteringTint;
            public Color dayScatteringTint;
            public Color sunSetScatteringTint;
            public Color nightScatteringTint;
        }
    }
}
