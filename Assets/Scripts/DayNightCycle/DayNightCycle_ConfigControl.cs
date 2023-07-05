using System;
using UnityEngine;

public partial class DayNightCycle : MonoBehaviour
{
    [Serializable]
    public class ConfigControl
    {
        public AnimationCurve sunAngularDiameter;
        public AnimationCurve sunFlareSize;
        public AnimationCurve sunFlareFalloff;
        public AnimationCurve sunIntensity;
        public Gradient sunFlareTint;
        public Gradient sunSurfaceTint;
        public Gradient sunEmissionColor;

        public AnimationCurve moonAngularDiameter;
        public AnimationCurve moonFlareSize;
        public AnimationCurve moonFlareFalloff;
        public AnimationCurve moonIntensity;
        public Gradient moonFlareTint;
        public Gradient moonSurfaceTint;
        public Gradient moonEmissionColor;

        public Gradient skyHorizonTint;
        public Gradient skyZenithTint;

        public AnimationCurve cloudAmbientDimmer;
        public AnimationCurve cloudLightDimmer;
        public Gradient cloudScatteringTint;

        public AnimationCurve sunRotation;
        public AnimationCurve moonRotation;
        public AnimationCurve sunLensFlare;

        public AnimationCurve waterDistantWindSpeed;
        public AnimationCurve waterLocalWindSpeed;
        public Gradient waterRefractingColor;
        public Gradient waterScatteringColor;

        public AnimationCurve fogMaxHeight;

        public ConfigControl() { }

        public void SetUpFogHeight(FogConfig config, float tSunRise, float tSunSet)
        {
            float[] ts = new float[]
            {
                0f,
                Mathf.Clamp(tSunRise - 0.05f, 0.0f, 1.0f),
                tSunRise,
                Mathf.Clamp(tSunRise + 0.05f, 0.0f, 1.0f),
                0.5f,
                Mathf.Clamp(tSunSet - 0.05f, 0.0f, 1.0f),
                tSunSet,
                Mathf.Clamp(tSunSet + 0.05f, 0.0f, 1.0f),
                1f
            };

            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[0],config.nightMaxHeight, 0.0f, 0.0f),
                    new Keyframe(ts[4],config.dayMaxHeight, 0.0f, 0.0f),
                    new Keyframe(ts[8],config.nightMaxHeight, 0.0f, 0.0f),
                };
                fogMaxHeight = new AnimationCurve(keys);
            }
        }

        public void SetUpWater(WaterConfig config, float tSunRise, float tSunSet)
        {
            float[] ts = new float[]
            {
                0f,
                Mathf.Clamp(tSunRise - 0.05f, 0.0f, 1.0f),
                tSunRise,
                Mathf.Clamp(tSunRise + 0.05f, 0.0f, 1.0f),
                0.5f,
                Mathf.Clamp(tSunSet - 0.05f, 0.0f, 1.0f),
                tSunSet,
                Mathf.Clamp(tSunSet + 0.05f, 0.0f, 1.0f),
                1f
            };

            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[0],config.simulation.nightDistantWindSpeed, 0.0f, 0.0f),
                    new Keyframe(ts[4],config.simulation.dayDistantWindSpeed, 0.0f, 0.0f),
                    new Keyframe(ts[8],config.simulation.nightDistantWindSpeed, 0.0f, 0.0f),
                };
                waterDistantWindSpeed = new AnimationCurve(keys);
            }
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[0],config.simulation.nightLocalWindSpeed, 0.0f, 0.0f),
                    new Keyframe(ts[4],config.simulation.dayLocalWindSpeed, 0.0f, 0.0f),
                    new Keyframe(ts[8],config.simulation.nightLocalWindSpeed, 0.0f, 0.0f),
                };
                waterLocalWindSpeed = new AnimationCurve(keys);
            }
            {
                waterRefractingColor = new Gradient();
                waterRefractingColor.mode = GradientMode.Blend;
                waterRefractingColor.colorSpace = ColorSpace.Linear;
                waterRefractingColor.alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1)
                };
                waterRefractingColor.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(config.appearance.nightRefractionColor,ts[0]),
                    new GradientColorKey(config.appearance.sunRiseRefractionColor,ts[2]),
                    new GradientColorKey(config.appearance.dayRefractionColor,ts[4]),
                    new GradientColorKey(config.appearance.sunSetRefractionColor,ts[6]),
                    new GradientColorKey(config.appearance.nightRefractionColor,ts[8]),
                };
            }
            {
                waterScatteringColor = new Gradient();
                waterScatteringColor.mode = GradientMode.Blend;
                waterScatteringColor.colorSpace = ColorSpace.Linear;
                waterScatteringColor.alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1)
                };
                waterScatteringColor.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(config.appearance.nightScatteringColor,ts[0]),
                    new GradientColorKey(config.appearance.sunRiseScatteringColor,ts[2]),
                    new GradientColorKey(config.appearance.dayScatteringColor,ts[4]),
                    new GradientColorKey(config.appearance.sunSetScatteringColor,ts[6]),
                    new GradientColorKey(config.appearance.nightScatteringColor,ts[8]),
                };
            }
        }

        public void SetUpLensFlare(float tSunRise, float tSunSet)
        {
            float[] sunTs = new float[]
            {
                0f,
                Mathf.Clamp(tSunRise - 0.05f, 0.0f, 1.0f),
                tSunRise,
                Mathf.Clamp(tSunRise + 0.05f, 0.0f, 1.0f),
                0.5f,
                Mathf.Clamp(tSunSet - 0.05f, 0.0f, 1.0f),
                tSunSet,
                Mathf.Clamp(tSunSet + 0.05f, 0.0f, 1.0f),
                1f
            };
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(sunTs[0],0.0f, 0.0f, 0.0f),
                    //new Keyframe(sunTs[1],0.0f, 0.0f, 0.0f),
                    new Keyframe(sunTs[2],0.0f, 0.0f, 0.0f),
                    //new Keyframe(sunTs[3],0.4f, 0.0f, 0.0f),
                    new Keyframe(sunTs[4],1.0f, 0.0f, 0.0f),
                    //new Keyframe(sunTs[5],0.6f, 0.0f, 0.0f),
                    new Keyframe(sunTs[6],0.0f, 0.0f, 0.0f),
                    //new Keyframe(sunTs[7],0.0f, 0.0f, 0.0f),
                    new Keyframe(sunTs[8],0.0f, 0.0f, 0.0f),
                };
                sunLensFlare = new AnimationCurve(keys);
            }
        }
        public void SetUpRotationCurve(float tSunRise, float tSunSet)
        {
            float[] sunTs = new float[]
            {
                0f,
                Mathf.Clamp(tSunRise - 0.05f, 0.0f, 1.0f),
                tSunRise,
                Mathf.Clamp(tSunRise + 0.05f, 0.0f, 1.0f),
                0.5f,
                Mathf.Clamp(tSunSet - 0.05f, 0.0f, 1.0f),
                tSunSet,
                Mathf.Clamp(tSunSet + 0.05f, 0.0f, 1.0f),
                1f
            };
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(sunTs[0],0.0f, 0.0f, 0.0f),
                    //new Keyframe(sunTs[1],0.2f, 0.0f, 1.0f),
                    new Keyframe(sunTs[2],0.25f, 1.0f, 1.0f),
                    //new Keyframe(sunTs[3],0.4f, 1.0f, 1.0f),
                    new Keyframe(sunTs[4],0.5f, 0.0f, 0.0f),
                    //new Keyframe(sunTs[5],0.6f, 1.0f, 1.0f),
                    new Keyframe(sunTs[6],0.75f, 1.0f, 1.0f),
                    //new Keyframe(sunTs[7],0.8f, 1.0f, 0.0f),
                    new Keyframe(sunTs[8],1.0f, 0.0f, 0.0f),
                };
                sunRotation = new AnimationCurve(keys);
            }
            float[] moonTs = new float[]
            {
                0f,
                Mathf.Clamp(tSunRise - 0.06f, 0.0f, 1.0f),
                tSunRise - 0.01f,
                Mathf.Clamp(tSunRise + 0.04f, 0.0f, 1.0f),
                0.5f,
                Mathf.Clamp(tSunSet - 0.06f, 0.0f, 1.0f),
                tSunSet,
                Mathf.Clamp(tSunSet + 0.01f, 0.0f, 1.0f),
                1f
            };
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(moonTs[0],0.0f, 0.0f, 0.0f),
                    //new Keyframe(moonTs[1],0.1f, 1.0f, 1.0f),
                    new Keyframe(moonTs[2],0.25f, 1.0f, 1.0f),
                    //new Keyframe(moonTs[3],0.3f, 1.0f, 0.0f),
                    new Keyframe(moonTs[4],0.5f, 0.0f, 0.0f),
                    //new Keyframe(moonTs[5],0.7f, 0.0f, 1.0f),
                    new Keyframe(moonTs[6],0.75f, 1.0f, 1.0f),
                    //new Keyframe(moonTs[7],0.9f, 1.0f, 1.0f),
                    new Keyframe(moonTs[8],1.0f, 0.0f, 0.0f),
                };
                moonRotation = new AnimationCurve(keys);
            }
        }
        public void SetupSun(LightConfig config, float tSunRise, float tSunSet)
        {
            float[] ts = new float[]
            {
                0f,
                Mathf.Clamp(tSunRise - 0.05f, 0.0f, 1.0f),
                tSunRise,
                Mathf.Clamp(tSunRise + 0.05f, 0.0f, 1.0f),
                0.5f,
                Mathf.Clamp(tSunSet - 0.05f, 0.0f, 1.0f),
                tSunSet,
                Mathf.Clamp(tSunSet + 0.05f, 0.0f, 1.0f),
                1f
            };
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[0],Mathf.Clamp(0.8f * config.celestialBody.maxAngularDiameter, config.celestialBody.minAngularDiameter, config.celestialBody.maxAngularDiameter), 0.0f, 0.0f),
                    new Keyframe(ts[4],Mathf.Clamp(1.0f * config.celestialBody.maxAngularDiameter, config.celestialBody.minAngularDiameter, config.celestialBody.maxAngularDiameter), 0.0f, 0.0f),
                    new Keyframe(ts[8],Mathf.Clamp(0.8f * config.celestialBody.maxAngularDiameter, config.celestialBody.minAngularDiameter, config.celestialBody.maxAngularDiameter), 0.0f, 0.0f)
                };
                sunAngularDiameter = new AnimationCurve(keys);
            }
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[2],Mathf.Clamp(0.0f * config.celestialBody.maxFlareSize, config.celestialBody.minFlareSize, config.celestialBody.maxFlareSize), 0.0f, 0.0f),
                    new Keyframe(ts[4],Mathf.Clamp(1.0f * config.celestialBody.maxFlareSize, config.celestialBody.minFlareSize, config.celestialBody.maxFlareSize), 0.0f, 0.0f),
                    new Keyframe(ts[6],Mathf.Clamp(0.0f * config.celestialBody.maxFlareSize, config.celestialBody.minFlareSize, config.celestialBody.maxFlareSize), 0.0f, 0.0f)
                };
                sunFlareSize = new AnimationCurve(keys);
            }
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[2],Mathf.Clamp(0.0f * config.celestialBody.maxFlareFalloff, config.celestialBody.minFlareFalloff, config.celestialBody.maxFlareFalloff), 0.0f, 0.0f),
                    new Keyframe(ts[4],Mathf.Clamp(0.0f * config.celestialBody.maxFlareFalloff, config.celestialBody.minFlareFalloff, config.celestialBody.maxFlareFalloff), 0.0f, 0.0f),
                    new Keyframe(ts[6],Mathf.Clamp(0.0f * config.celestialBody.maxFlareFalloff, config.celestialBody.minFlareFalloff, config.celestialBody.maxFlareFalloff), 0.0f, 0.0f)
                };
                sunFlareFalloff = new AnimationCurve(keys);
            }
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[1],Mathf.Clamp(0.0f * config.lightEmission.maxIntensity, config.lightEmission.minIntensity, config.lightEmission.maxIntensity), 0.0f, 0.0f),
                    new Keyframe(ts[2],Mathf.Clamp(0.4f * config.lightEmission.maxIntensity, config.lightEmission.minIntensity, config.lightEmission.maxIntensity), 3.0f, 2.0f),
                    new Keyframe(ts[3],Mathf.Clamp(0.9f * config.lightEmission.maxIntensity, config.lightEmission.minIntensity, config.lightEmission.maxIntensity), 2.0f, 1.0f),
                    new Keyframe(ts[4],Mathf.Clamp(1.0f * config.lightEmission.maxIntensity, config.lightEmission.minIntensity, config.lightEmission.maxIntensity), 1.0f, -1.0f),
                    new Keyframe(ts[5],Mathf.Clamp(0.9f * config.lightEmission.maxIntensity, config.lightEmission.minIntensity, config.lightEmission.maxIntensity), -1.0f, -2.0f),
                    new Keyframe(ts[6],Mathf.Clamp(0.4f * config.lightEmission.maxIntensity, config.lightEmission.minIntensity, config.lightEmission.maxIntensity), -2.0f, -3.0f),
                    new Keyframe(ts[7],Mathf.Clamp(0.0f * config.lightEmission.maxIntensity, config.lightEmission.minIntensity, config.lightEmission.maxIntensity), 0.0f, 0.0f)
                };
                sunIntensity = new AnimationCurve(keys);
            }
            {
                sunFlareTint = new Gradient();
                sunFlareTint.mode = GradientMode.Blend;
                sunFlareTint.colorSpace = ColorSpace.Linear;
                sunFlareTint.alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1)
                };
                sunFlareTint.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(config.celestialBody.sunRiseFlareTint,ts[2]),
                    new GradientColorKey(config.celestialBody.peakFlareTint,ts[4]),
                    new GradientColorKey(config.celestialBody.sunSetFlareTint,ts[6])
                };
            }
            {
                sunSurfaceTint = new Gradient();
                sunSurfaceTint.mode = GradientMode.Blend;
                sunSurfaceTint.colorSpace = ColorSpace.Linear;
                sunSurfaceTint.alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1)
                };
                sunSurfaceTint.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(config.celestialBody.sunRiseSurfaceTint,ts[2]),
                    new GradientColorKey(config.celestialBody.peakSurfaceTint,ts[4]),
                    new GradientColorKey(config.celestialBody.sunSetSurfaceTint,ts[6])
                };
            }
            {
                sunEmissionColor = new Gradient();
                sunEmissionColor.mode = GradientMode.Blend;
                sunEmissionColor.colorSpace = ColorSpace.Linear;
                sunEmissionColor.alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1)
                };
                sunEmissionColor.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(config.lightEmission.sunRiseEmissionColor,ts[2]),
                    new GradientColorKey(config.lightEmission.peakEmissionColor,ts[4]),
                    new GradientColorKey(config.lightEmission.sunSetEmissionColor,ts[6])
                };
            }
        }
        public void SetupMoon(LightConfig config, float tSunRise, float tSunSet)
        {
            float[] ts = new float[]
            {
                0f,
                Mathf.Clamp(tSunRise - 0.06f, 0.0f, 1.0f),
                tSunRise - 0.01f,
                Mathf.Clamp(tSunRise + 0.04f, 0.0f, 1.0f),
                0.5f,
                Mathf.Clamp(tSunSet - 0.06f, 0.0f, 1.0f),
                tSunSet,
                Mathf.Clamp(tSunSet + 0.01f, 0.0f, 1.0f),
                1f
            };
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[0],Mathf.Clamp(1.0f * config.celestialBody.maxAngularDiameter, config.celestialBody.minAngularDiameter, config.celestialBody.maxAngularDiameter), 0.0f, 0.0f),
                    new Keyframe(ts[4],Mathf.Clamp(0.8f * config.celestialBody.maxAngularDiameter, config.celestialBody.minAngularDiameter, config.celestialBody.maxAngularDiameter), 0.0f, 0.0f),
                    new Keyframe(ts[8],Mathf.Clamp(1.0f * config.celestialBody.maxAngularDiameter, config.celestialBody.minAngularDiameter, config.celestialBody.maxAngularDiameter), 0.0f, 0.0f)
                };
                moonAngularDiameter = new AnimationCurve(keys);
            }
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[2],Mathf.Clamp(1.0f * config.celestialBody.maxFlareSize, config.celestialBody.minFlareSize, config.celestialBody.maxFlareSize), 0.0f, 0.0f),
                    new Keyframe(ts[4],Mathf.Clamp(0.0f * config.celestialBody.maxFlareSize, config.celestialBody.minFlareSize, config.celestialBody.maxFlareSize), 0.0f, 0.0f),
                    new Keyframe(ts[6],Mathf.Clamp(1.0f * config.celestialBody.maxFlareSize, config.celestialBody.minFlareSize, config.celestialBody.maxFlareSize), 0.0f, 0.0f)
                };
                moonFlareSize = new AnimationCurve(keys);
            }
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[2],Mathf.Clamp(1.0f * config.celestialBody.maxFlareFalloff, config.celestialBody.minFlareFalloff, config.celestialBody.maxFlareFalloff), 0.0f, 0.0f),
                    new Keyframe(ts[4],Mathf.Clamp(0.0f * config.celestialBody.maxFlareFalloff, config.celestialBody.minFlareFalloff, config.celestialBody.maxFlareFalloff), 0.0f, 0.0f),
                    new Keyframe(ts[6],Mathf.Clamp(1.0f * config.celestialBody.maxFlareFalloff, config.celestialBody.minFlareFalloff, config.celestialBody.maxFlareFalloff), 0.0f, 0.0f)
                };
                moonFlareFalloff = new AnimationCurve(keys);
            }
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[0],Mathf.Clamp(1.0f * config.lightEmission.maxIntensity, config.lightEmission.minIntensity, config.lightEmission.maxIntensity), 0.0f, -2.0f),
                    new Keyframe(ts[1],Mathf.Clamp(0.3f * config.lightEmission.maxIntensity, config.lightEmission.minIntensity, config.lightEmission.maxIntensity), -2.0f, -2.0f),
                    new Keyframe(ts[2],Mathf.Clamp(0.0f * config.lightEmission.maxIntensity, config.lightEmission.minIntensity, config.lightEmission.maxIntensity), 0.0f, 0.0f),
                    new Keyframe(ts[4],Mathf.Clamp(0.0f * config.lightEmission.maxIntensity, config.lightEmission.minIntensity, config.lightEmission.maxIntensity), 0.0f, 0.0f),
                    new Keyframe(ts[6],Mathf.Clamp(0.0f * config.lightEmission.maxIntensity, config.lightEmission.minIntensity, config.lightEmission.maxIntensity), 0.0f, 0.0f),
                    new Keyframe(ts[7],Mathf.Clamp(0.3f * config.lightEmission.maxIntensity, config.lightEmission.minIntensity, config.lightEmission.maxIntensity), 2.0f, 2.0f),
                    new Keyframe(ts[8],Mathf.Clamp(1.0f * config.lightEmission.maxIntensity, config.lightEmission.minIntensity, config.lightEmission.maxIntensity), 2.0f, 0.0f)
                };
                moonIntensity = new AnimationCurve(keys);
            }
            {
                moonFlareTint = new Gradient();
                moonFlareTint.mode = GradientMode.Blend;
                moonFlareTint.colorSpace = ColorSpace.Linear;
                moonFlareTint.alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1)
                };
                moonFlareTint.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(config.celestialBody.peakFlareTint,ts[0]),
                    new GradientColorKey(config.celestialBody.sunRiseFlareTint,ts[2]),
                    new GradientColorKey(config.celestialBody.sunSetFlareTint,ts[6]),
                    new GradientColorKey(config.celestialBody.peakFlareTint,ts[8])
                };
            }
            {
                moonSurfaceTint = new Gradient();
                moonSurfaceTint.mode = GradientMode.Blend;
                moonSurfaceTint.colorSpace = ColorSpace.Linear;
                moonSurfaceTint.alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1)
                };
                moonSurfaceTint.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(config.celestialBody.peakSurfaceTint,ts[0]),
                    new GradientColorKey(config.celestialBody.sunRiseSurfaceTint,ts[2]),
                    new GradientColorKey(config.celestialBody.sunSetSurfaceTint,ts[6]),
                    new GradientColorKey(config.celestialBody.peakSurfaceTint,ts[8])
                };
            }
            {
                moonEmissionColor = new Gradient();
                moonEmissionColor.mode = GradientMode.Blend;
                moonEmissionColor.colorSpace = ColorSpace.Linear;
                moonEmissionColor.alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1)
                };
                moonEmissionColor.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(config.lightEmission.peakEmissionColor,ts[0]),
                    new GradientColorKey(config.lightEmission.sunRiseEmissionColor,ts[2]),
                    new GradientColorKey(config.lightEmission.sunSetEmissionColor,ts[6]),
                    new GradientColorKey(config.lightEmission.peakEmissionColor,ts[8])
                };
            }
        }
        public void SetupSky(SkyConfig config, float tSunRise, float tSunSet)
        {
            float[] ts = new float[]
            {
                0f,
                Mathf.Clamp(tSunRise - 0.05f, 0.0f, 1.0f),
                tSunRise,
                Mathf.Clamp(tSunRise + 0.05f, 0.0f, 1.0f),
                0.5f,
                Mathf.Clamp(tSunSet - 0.05f, 0.0f, 1.0f),
                tSunSet,
                Mathf.Clamp(tSunSet + 0.05f, 0.0f, 1.0f),
                1f
            };
            {
                skyHorizonTint = new Gradient();
                skyHorizonTint.mode = GradientMode.Blend;
                skyHorizonTint.colorSpace = ColorSpace.Linear;
                skyHorizonTint.alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1)
                };
                skyHorizonTint.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(config.artisticOverrides.nightHorizonTint,ts[0]),
                    new GradientColorKey(config.artisticOverrides.sunRiseHorizonTint,ts[2]),
                    new GradientColorKey(config.artisticOverrides.dayHorizonTint,ts[4]),
                    new GradientColorKey(config.artisticOverrides.sunSetHorizonTint,ts[6]),
                    new GradientColorKey(config.artisticOverrides.nightHorizonTint,ts[8]),
                };
            }
            {
                skyZenithTint = new Gradient();
                skyZenithTint.mode = GradientMode.Blend;
                skyZenithTint.colorSpace = ColorSpace.Linear;
                skyZenithTint.alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1)
                };
                skyHorizonTint.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(config.artisticOverrides.nightZenithTint,ts[0]),
                    new GradientColorKey(config.artisticOverrides.nightZenithTint,ts[1]),
                    new GradientColorKey(config.artisticOverrides.dayZenithTint,ts[3]),
                    new GradientColorKey(config.artisticOverrides.dayZenithTint,ts[4]),
                    new GradientColorKey(config.artisticOverrides.dayZenithTint,ts[5]),
                    new GradientColorKey(config.artisticOverrides.nightZenithTint,ts[7]),
                    new GradientColorKey(config.artisticOverrides.nightZenithTint,ts[8])
                };
            }
        }
        public void SetupCloud(CloudConfig config, float tSunRise, float tSunSet)
        {
            float[] ts = new float[]
            {
                0f,
                Mathf.Clamp(tSunRise - 0.05f, 0.0f, 1.0f),
                tSunRise,
                Mathf.Clamp(tSunRise + 0.05f, 0.0f, 1.0f),
                0.5f,
                Mathf.Clamp(tSunSet - 0.05f, 0.0f, 1.0f),
                tSunSet,
                Mathf.Clamp(tSunSet + 0.05f, 0.0f, 1.0f),
                1f
            };
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[0],config.lighting.minAmbientDimmer, 0.0f, 0.0f),
                    new Keyframe(ts[1],config.lighting.maxAmbientDimmer * 0.8f, 0.0f, 3.0f),
                    new Keyframe(ts[4],config.lighting.maxAmbientDimmer * 1.0f, 0.0f, 0.0f),
                    new Keyframe(ts[7],config.lighting.maxAmbientDimmer * 0.8f, -3.0f, 0.0f),
                    new Keyframe(ts[8],config.lighting.minAmbientDimmer, 0.0f, 0.0f)
                };
                cloudAmbientDimmer = new AnimationCurve(keys);
            }
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[0],config.lighting.minLightDimmer, 0.0f, 0.0f),
                    new Keyframe(ts[1],config.lighting.maxLightDimmer * 0.8f, 0.0f, 3.0f),
                    new Keyframe(ts[4],config.lighting.maxLightDimmer * 1.0f, 0.0f, 0.0f),
                    new Keyframe(ts[7],config.lighting.maxLightDimmer * 0.8f, -3.0f, 0.0f),
                    new Keyframe(ts[8],config.lighting.minLightDimmer, 0.0f, 0.0f)
                };
                cloudLightDimmer = new AnimationCurve(keys);
            }
            {
                cloudScatteringTint = new Gradient();
                cloudScatteringTint.mode = GradientMode.Blend;
                cloudScatteringTint.colorSpace = ColorSpace.Linear;
                cloudScatteringTint.alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1)
                };
                cloudScatteringTint.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(config.lighting.nightScatteringTint,ts[0]),
                    new GradientColorKey(config.lighting.sunRiseScatteringTint,ts[1]),
                    new GradientColorKey(config.lighting.dayScatteringTint,ts[4]),
                    new GradientColorKey(config.lighting.sunSetScatteringTint,ts[7]),
                    new GradientColorKey(config.lighting.nightScatteringTint,ts[8]),
                };
            }
        }
    }
}
