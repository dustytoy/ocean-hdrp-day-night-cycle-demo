using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static DayNightCycle;

public class DayNightCycle : MonoBehaviour
{
    [Range(1f, 1000f)]
    public int gameSecondPerRealSecond;
    [HideInInspector]
    public long currentTick;
    [HideInInspector]
    public bool isDayTime;

    public delegate void SunSet(long ticks);
    public delegate void SunRise(long ticks);
    public event SunSet onSunSet;
    public event SunSet onSunRise;

    public Light sunLight;
    public Light moonLight;
    public Volume globalVolumn;
    public PhysicallyBasedSky sky;
    public VolumetricClouds clouds;
    public WaterSurface ocean;

    public DayNightConfig CONFIG;

    private ConfigControl _config;
    private HDAdditionalLightData _sunData;
    private HDAdditionalLightData _moonData;
    private Transform _sunTransform;
    private Transform _moonTransform;
    private WindSpeedParameter.WindParamaterValue _overrideWindSpeed;

    private float _tSunRise;
    private float _tSunSet;

    private void Awake()
    {
        globalVolumn.profile.TryGet<PhysicallyBasedSky>(out sky);
        globalVolumn.profile.TryGet<VolumetricClouds>(out clouds);
        _sunData = sunLight.GetComponent<HDAdditionalLightData>();
        _moonData = moonLight.GetComponent<HDAdditionalLightData>();
        _sunTransform = sunLight.transform;
        _moonTransform = moonLight.transform;
        _overrideWindSpeed = new WindSpeedParameter.WindParamaterValue();

        onSunRise += OnSunRise;
        onSunSet += OnSunSet;
    }

    private void OnDestroy()
    {
        onSunRise -= OnSunRise;
        onSunSet -= OnSunSet;
    }

    private void Start()
    {
        
        Setup();

        float t = (float)currentTick / OneDayTime.TotalTicks;
        Evaluate(t);
    }

    private void FixedUpdate()
    {
        float t = (float)currentTick / OneDayTime.TotalTicks;
        Evaluate(t);

        ocean.timeMultiplier = gameSecondPerRealSecond;
        _overrideWindSpeed.customValue = gameSecondPerRealSecond * 1;
        clouds.globalWindSpeed.Override(_overrideWindSpeed);

        currentTick += (long)(Time.fixedDeltaTime * OneDayTime.TicksPerSecond * gameSecondPerRealSecond);
        currentTick %= OneDayTime.TotalTicks;
    }

    public void Evaluate(float t)
    {
        EvaluateLight(t);
        EvaluateSky(t);
        EvaluateClouds(t);
    }

    public void EvaluateLight(float t)
    {
        if(t > _tSunSet && isDayTime)
        {
            //_sunTransform.gameObject.SetActive(false);
            //_moonTransform.gameObject.SetActive(true);
            isDayTime = false;
            onSunSet?.Invoke(currentTick);
        }
        else if (t > _tSunRise && !isDayTime)
        {
            //_sunTransform.gameObject.SetActive(true);
            //_moonTransform.gameObject.SetActive(false);
            isDayTime = true;
            onSunRise?.Invoke(currentTick);
        }
        _sunTransform.rotation  = Quaternion.AngleAxis(t * 360f + 180f + 90f, Vector3.right);
        _moonTransform.rotation = Quaternion.AngleAxis(t * 360f + 90f       , Vector3.right);

        if(isDayTime)
        {
            var celes = CONFIG.sunLight.celestialBody;
            _sunData.angularDiameter    = Mathf.Clamp(_config.sunAngularDiameter.Evaluate(t) * celes.maxAngularDiameter, celes.minAngularDiameter, celes.maxAngularDiameter);
            _sunData.flareSize          = Mathf.Clamp(_config.sunFlareSize.Evaluate(t) * celes.maxFlareSize, celes.minFlareSize, celes.maxFlareSize);
            _sunData.flareFalloff       = Mathf.Clamp(_config.sunFlareFalloff.Evaluate(t) * celes.maxFlareFalloff, celes.minFlareFalloff, celes.maxFlareFalloff);
            _sunData.flareTint          = _config.sunFlareTint.Evaluate(t);
            _sunData.surfaceTint        = _config.sunSurfaceTint.Evaluate(t);

            var emis = CONFIG.sunLight.lightEmission;
            _sunData.color      = _config.sunEmissionColor.Evaluate(t);
            _sunData.intensity  = Mathf.Clamp(_config.sunIntensity.Evaluate(t) * emis.maxIntensity, emis.minIntensity, emis.maxIntensity);
        }
        else
        {
            var celes = CONFIG.moonLight.celestialBody;
            _moonData.angularDiameter   = Mathf.Clamp(_config.moonAngularDiameter.Evaluate(t) * celes.maxAngularDiameter, celes.minAngularDiameter, celes.maxAngularDiameter);
            _moonData.flareSize         = Mathf.Clamp(_config.moonFlareSize.Evaluate(t) * celes.maxFlareSize, celes.minFlareSize, celes.maxFlareSize);
            _moonData.flareFalloff      = Mathf.Clamp(_config.moonFlareFalloff.Evaluate(t) * celes.maxFlareFalloff, celes.minFlareFalloff, celes.maxFlareFalloff);
            _moonData.flareTint         = _config.moonFlareTint.Evaluate(t);
            _moonData.surfaceTint       = _config.moonSurfaceTint.Evaluate(t);

            var emis = CONFIG.moonLight.lightEmission;
            _moonData.color     = _config.moonEmissionColor.Evaluate(t);
            _moonData.intensity = Mathf.Clamp(_config.moonIntensity.Evaluate(t) * emis.maxIntensity, emis.minIntensity, emis.maxIntensity);
        }
    }
    public void EvaluateSky(float t)
    {
        sky.horizonTint.value   = _config.skyHorizonTint.Evaluate(t);
        sky.zenithTint.value    = _config.skyZenithTint.Evaluate(t);
    }
    public void EvaluateClouds(float t)
    {
        var light = CONFIG.cloud.lighting;
        clouds.ambientLightProbeDimmer.value    = Mathf.Clamp(_config.cloudAmbientDimmer.Evaluate(t) * light.minAmbientDimmer, light.minAmbientDimmer, light.maxAmbientDimmer);
        clouds.sunLightDimmer.value             = Mathf.Clamp(_config.cloudLightDimmer.Evaluate(t) * light.minLightDimmer, light.minLightDimmer, light.maxLightDimmer);
        clouds.scatteringTint.value             = _config.cloudScatteringTint.Evaluate(t);
    }
    public void Setup()
    {
        // Setup Timings
        _tSunRise   = (float)CONFIG.sunRiseTime.ToTicks() / OneDayTime.TotalTicks;
        _tSunSet    = (float)CONFIG.sunSetTime.ToTicks() / OneDayTime.TotalTicks;
        currentTick = CONFIG.startTime.ToTicks();

        if (currentTick > CONFIG.sunRiseTime.ToTicks() && 
            currentTick < CONFIG.sunSetTime.ToTicks())
        {
            isDayTime = true;
        }
        else
        {
            isDayTime = false;
        }

        _config = new ConfigControl();
        _config.SetupSun    (CONFIG.sunLight, _tSunRise, _tSunSet);
        _config.SetupSun    (CONFIG.moonLight, _tSunRise, _tSunSet);
        _config.SetupSky    (CONFIG.sky, _tSunRise, _tSunSet);
        _config.SetupCloud  (CONFIG.cloud, _tSunRise, _tSunSet);
    }

    private void OnSunSet(long ticks)
    {
    }
    private void OnSunRise(long ticks)
    {
    }

    [Serializable]
    public struct OneDayTime
    {
        public const long TotalTicks = 864000000000;
        public const int TotalSeconds = 86400;
        public const int TotalMinutes = 60;
        public const int TotalHours = 24;
        public const long TicksPerHour = 36000000000;
        public const long TicksPerMinute = 600000000;
        public const long TicksPerSecond = 10000000;


        public int hour;
        public int minute;
        public int second;

        public long ToTicks()
        {
            return hour * TicksPerHour + minute * TicksPerMinute + second * TicksPerSecond;
        }
        public static OneDayTime ToOneDayTime(long ticks)
        {
            int hour = (int)(ticks / TicksPerHour);
            int minute = (int)((ticks - hour * TicksPerHour) / TicksPerMinute);
            int second = (int)((ticks - hour * TicksPerHour - minute * TicksPerMinute) / TicksPerSecond);
            return new OneDayTime()
            {
                hour = hour,
                minute = minute,
                second = second
            };
        }
    }

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

        public ConfigControl() { }
        public void SetupSun(LightConfig config, float tSunRise, float tSunSet)
        {
            float[] ts = new float[]
            {
                0f,
                Mathf.Clamp(tSunRise - 0.05f, 0.1f, 0.25f),
                tSunRise,
                Mathf.Clamp(tSunRise + 0.05f, 0.25f, 0.4f),
                0.5f,
                Mathf.Clamp(tSunSet - 0.05f, 0.65f, 0.75f),
                tSunSet,
                Mathf.Clamp(tSunSet + 0.05f, 0.75f, 0.8f),
                1f
            };
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[0],0.8f, 0.0f, 0.0f),
                    new Keyframe(ts[4],1.0f, 0.0f, 0.0f),
                    new Keyframe(ts[8],0.8f, 0.0f, 0.0f)
                };
                sunAngularDiameter = new AnimationCurve(keys);
            }
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[2],0.0f, 0.0f, 0.0f),
                    new Keyframe(ts[4],1.0f, 0.0f, 0.0f),
                    new Keyframe(ts[6],0.0f, 0.0f, 0.0f),
                };
                sunFlareSize = new AnimationCurve(keys);
            }
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[2],0.0f, 0.0f, 0.0f),
                    new Keyframe(ts[4],1.0f, 0.0f, 0.0f),
                    new Keyframe(ts[6],0.0f, 0.0f, 0.0f),
                };
                sunFlareFalloff = new AnimationCurve(keys);
            }
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[2],0.0f, 0.0f, 0.0f),
                    new Keyframe(ts[3],0.8f, 5.0f, 5.0f),
                    new Keyframe(ts[4],1.0f, 0.0f, 0.0f),
                    new Keyframe(ts[5],0.8f, -5.0f, -5.0f),
                    new Keyframe(ts[6],0.0f, 0.0f, 0.0f),
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
                    new GradientColorKey(config.celestialBody.sunSetFlareTint,ts[6]),
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
                    new GradientColorKey(config.celestialBody.sunSetSurfaceTint,ts[6]),
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
                    new GradientColorKey(config.lightEmission.sunSetEmissionColor,ts[6]),
                };
            }
        }
        public void SetupMoon(LightConfig config, float tSunRise, float tSunSet)
        {
            float[] ts = new float[]
            {
                0f,
                Mathf.Clamp(tSunRise - 0.1f, 0.1f, 0.25f),
                tSunRise - 0.05f,
                Mathf.Clamp(tSunRise, 0.2f, 0.35f),
                0.5f,
                Mathf.Clamp(tSunSet, 0.7f, 0.8f),
                tSunSet + 0.05f,
                Mathf.Clamp(tSunSet + 0.1f, 0.8f, 0.9f),
                1f
            };
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[0],1.0f, 0.0f, 0.0f),
                    new Keyframe(ts[4],0.8f, 0.0f, 0.0f),
                    new Keyframe(ts[8],1.0f, 0.0f, 0.0f)
                };
                moonAngularDiameter = new AnimationCurve(keys);
            }
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[2],1.0f, 0.0f, 0.0f),
                    new Keyframe(ts[4],0.0f, 0.0f, 0.0f),
                    new Keyframe(ts[6],1.0f, 0.0f, 0.0f),
                };
                moonFlareSize = new AnimationCurve(keys);
            }
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[2],1.0f, 0.0f, 0.0f),
                    new Keyframe(ts[4],0.0f, 0.0f, 0.0f),
                    new Keyframe(ts[6],1.0f, 0.0f, 0.0f),
                };
                moonFlareFalloff = new AnimationCurve(keys);
            }
            {
                Keyframe[] keys = new Keyframe[]
                {
                    new Keyframe(ts[0],1.0f, 0.0f, 0.0f),
                    new Keyframe(ts[1],0.8f, -5.0f, -5.0f),
                    new Keyframe(ts[4],0.0f, 0.0f, 0.0f),
                    new Keyframe(ts[7],0.8f, 5.0f, 5.0f),
                    new Keyframe(ts[8],1.0f, 0.0f, 0.0f),
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
                    new GradientColorKey(config.celestialBody.peakFlareTint,ts[8]),
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
                    new GradientColorKey(config.celestialBody.peakSurfaceTint,ts[8]),
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
                    new GradientColorKey(config.lightEmission.peakEmissionColor,ts[8]),
                };
            }
        }
        public void SetupSky(SkyConfig config, float tSunRise, float tSunSet)
        {
            float[] ts = new float[]
            {
                0f,
                tSunRise,
                Mathf.Clamp(tSunRise + 0.05f, 0.25f, 0.4f),
                0.5f,
                Mathf.Clamp(tSunSet - 0.05f, 0.65f, 0.75f),
                tSunSet,
                Mathf.Clamp(tSunSet + 0.05f, 0.75f, 0.8f),
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
                Mathf.Clamp(tSunRise - 0.05f, 0.1f, 0.25f),
                tSunRise,
                Mathf.Clamp(tSunRise + 0.05f, 0.25f, 0.4f),
                0.5f,
                Mathf.Clamp(tSunSet - 0.05f, 0.65f, 0.75f),
                tSunSet,
                Mathf.Clamp(tSunSet + 0.05f, 0.75f, 0.8f),
                1f
            };
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
                    new GradientColorKey(config.lighting.sunRiseScatteringTint,ts[2]),
                    new GradientColorKey(config.lighting.dayScatteringTint,ts[4]),
                    new GradientColorKey(config.lighting.sunSetScatteringTint,ts[6]),
                    new GradientColorKey(config.lighting.nightScatteringTint,ts[8]),
                };
            }
        }
    }

    [Serializable]
    public struct DayNightConfig
    {
        public OneDayTime startTime;
        public OneDayTime sunRiseTime;
        public OneDayTime sunSetTime;
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
            public Color peakFlareTint;
            public Color peakSurfaceTint;
            public Color sunRiseFlareTint;
            public Color sunSetFlareTint;
            public Color sunRiseSurfaceTint;
            public Color sunSetSurfaceTint;

        }
        [Serializable]
        public struct Emission
        {
            public float maxIntensity;
            public float minIntensity;
            public Color peakEmissionColor;
            public Color sunRiseEmissionColor;
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
            
            public Color dayHorizonTint;
            public Color nightHorizonTint;
            public Color sunRiseHorizonTint;
            public Color sunSetHorizonTint;

            public Color dayScattering;
            public Color nightScattering;
            public Color sunRiseScattering;
            public Color sunSetScattering;
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
            public Color dayScatteringTint;
            public Color nightScatteringTint;
            public Color sunRiseScatteringTint;
            public Color sunSetScatteringTint;
        }
    }
}
