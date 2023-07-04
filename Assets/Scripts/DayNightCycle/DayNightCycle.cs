using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public partial class DayNightCycle : MonoBehaviour
{
    [Range(1f, 10000f)]
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
    public WaterSurface water;

    public DayNightConfig config;
    public ConfigControl configSample;

    private HDAdditionalLightData _sunData;
    private LensFlareComponentSRP _sunFlare;
    private HDAdditionalLightData _moonData;
    private Transform _sunTransform;
    private Transform _moonTransform;
    private WindSpeedParameter.WindParamaterValue _overrideWindSpeed;

    private float _tSunRise;
    private float _tSunSet;
    private int _dayCount;
    private GUIStyle _style = new GUIStyle();

    private void Awake()
    {
        globalVolumn.profile.TryGet<PhysicallyBasedSky>(out sky);
        globalVolumn.profile.TryGet<VolumetricClouds>(out clouds);
        _sunData = sunLight.GetComponent<HDAdditionalLightData>();
        _moonData = moonLight.GetComponent<HDAdditionalLightData>();
        _sunFlare = _sunData.GetComponent<LensFlareComponentSRP>();
        _sunTransform = sunLight.transform;
        _moonTransform = moonLight.transform;
        _overrideWindSpeed = new WindSpeedParameter.WindParamaterValue();

        _style.fontSize = 20;
        _style.normal.textColor = Color.white;
        _style.normal.background = Texture2D.grayTexture;

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
        float t = ((float)currentTick % MyTime.TotalTicks) / MyTime.TotalTicks;
        Evaluate(t);
    }

    private void FixedUpdate()
    {
        float t = (float)currentTick / MyTime.TotalTicks;
        Evaluate(t);

        water.timeMultiplier = gameSecondPerRealSecond;
        _overrideWindSpeed.customValue = gameSecondPerRealSecond * 1;
        clouds.globalWindSpeed.Override(_overrideWindSpeed);

        currentTick += (long)(Time.fixedDeltaTime * MyTime.TicksPerSecond * gameSecondPerRealSecond);
        if(currentTick >= MyTime.TotalTicks)
        {
            currentTick %= MyTime.TotalTicks;
            _dayCount++;
        }
    }

    private void OnGUI()
    {
        MyTime time = MyTime.ToOneDayTime(currentTick);
        GUI.Label(new Rect(0f, 0f, 500f, 200f), $"SunRise: {config.sunRiseTime} | Sunset:{config.sunSetTime}\n" +
            $"Time (t): {time} ({((float)currentTick % MyTime.TotalTicks) / MyTime.TotalTicks})\n" +
            $"SecondsPerRealSecond: {gameSecondPerRealSecond}\n" +
            $"Sun : {(_sunTransform.gameObject.activeInHierarchy ? "Active" : "Inactive")} ({(int)_sunTransform.eulerAngles.x} degrees, facing {_sunTransform.forward})\n" +
            $"Moon: {(_moonTransform.gameObject.activeInHierarchy ? "Active" : "Inactive")} ({(int)_moonTransform.eulerAngles.x} degrees, facing {_moonTransform.forward})\n", _style);
    }

    public void Evaluate(float t)
    {
        EvaluateLight(t);
        EvaluateSky(t);
        EvaluateClouds(t);
        EvaluateWater(t);
    }

    public void EvaluateLight(float t)
    {
        if((t >= _tSunRise && t <= _tSunSet) && !isDayTime)
        {
            //_sunTransform.gameObject.SetActive(true);
            //_moonTransform.gameObject.SetActive(false);
            isDayTime = true;
            onSunRise?.Invoke(currentTick);
        }
        else if ((t < _tSunRise || t > _tSunSet) && isDayTime)
        {
            //_sunTransform.gameObject.SetActive(false);
            //_moonTransform.gameObject.SetActive(true);
            isDayTime = false;
            onSunSet?.Invoke(currentTick);
        }

        {
            var sunAngle = Mathf.Clamp(configSample.sunRotation.Evaluate(t) * 360f, 0f, 360f);
            var moonAngle = Mathf.Clamp(configSample.moonRotation.Evaluate(t) * 360f, 0f, 360f);
            _sunTransform.rotation = Quaternion.AngleAxis(-90f, Vector3.up) * Quaternion.AngleAxis(_dayCount * 360f + 270f + sunAngle, Vector3.right);
            _moonTransform.rotation = Quaternion.AngleAxis(-90f, Vector3.up) * Quaternion.AngleAxis(_dayCount * 360f + 90f + moonAngle, Vector3.right);
        }

        if(isDayTime)
        {
            var celes = config.sunLight.celestialBody;
            _sunData.angularDiameter    = Mathf.Clamp(configSample.sunAngularDiameter.Evaluate(t) * celes.maxAngularDiameter, celes.minAngularDiameter, celes.maxAngularDiameter);
            _sunData.flareSize          = Mathf.Clamp(configSample.sunFlareSize.Evaluate(t) * celes.maxFlareSize, celes.minFlareSize, celes.maxFlareSize);
            _sunData.flareFalloff       = Mathf.Clamp(configSample.sunFlareFalloff.Evaluate(t) * celes.maxFlareFalloff, celes.minFlareFalloff, celes.maxFlareFalloff);
            _sunData.flareTint          = configSample.sunFlareTint.Evaluate(t);
            _sunData.surfaceTint        = configSample.sunSurfaceTint.Evaluate(t);

            var emis = config.sunLight.lightEmission;
            _sunData.color      = configSample.sunEmissionColor.Evaluate(t);
            _sunData.intensity  = Mathf.Clamp(configSample.sunIntensity.Evaluate(t) * emis.maxIntensity, emis.minIntensity, emis.maxIntensity);

            _sunFlare.intensity = Mathf.Clamp01(configSample.sunLensFlare.Evaluate(t));
            _sunFlare.scale     = Mathf.Clamp01(configSample.sunLensFlare.Evaluate(t));
        }
        else
        {
            var celes = config.moonLight.celestialBody;
            _moonData.angularDiameter   = Mathf.Clamp(configSample.moonAngularDiameter.Evaluate(t) * celes.maxAngularDiameter, celes.minAngularDiameter, celes.maxAngularDiameter);
            _moonData.flareSize         = Mathf.Clamp(configSample.moonFlareSize.Evaluate(t) * celes.maxFlareSize, celes.minFlareSize, celes.maxFlareSize);
            _moonData.flareFalloff      = Mathf.Clamp(configSample.moonFlareFalloff.Evaluate(t) * celes.maxFlareFalloff, celes.minFlareFalloff, celes.maxFlareFalloff);
            _moonData.flareTint         = configSample.moonFlareTint.Evaluate(t);
            _moonData.surfaceTint       = configSample.moonSurfaceTint.Evaluate(t);

            var emis = config.moonLight.lightEmission;
            _moonData.color     = configSample.moonEmissionColor.Evaluate(t);
            _moonData.intensity = Mathf.Clamp(configSample.moonIntensity.Evaluate(t) * emis.maxIntensity, emis.minIntensity, emis.maxIntensity);
        }
    }
    public void EvaluateSky(float t)
    {
        sky.horizonTint.value   = configSample.skyHorizonTint.Evaluate(t);
        sky.zenithTint.value    = configSample.skyZenithTint.Evaluate(t);
    }
    public void EvaluateClouds(float t)
    {
        var light = config.cloud.lighting;
        clouds.ambientLightProbeDimmer.value    = Mathf.Clamp(configSample.cloudAmbientDimmer.Evaluate(t) * light.maxAmbientDimmer, light.minAmbientDimmer, light.maxAmbientDimmer);
        clouds.sunLightDimmer.value             = Mathf.Clamp(configSample.cloudLightDimmer.Evaluate(t) * light.maxLightDimmer, light.minLightDimmer, light.maxLightDimmer);
        clouds.scatteringTint.value             = configSample.cloudScatteringTint.Evaluate(t);
    }
    public void EvaluateWater(float t)
    {
        water.largeWindSpeed    = configSample.waterDistantWindSpeed.Evaluate(t);
        water.ripplesWindSpeed  = configSample.waterLocalWindSpeed.Evaluate(t);
        water.refractionColor   = configSample.waterRefractingColor.Evaluate(t);
        water.scatteringColor   = configSample.waterScatteringColor.Evaluate(t);
    }
    public void Setup()
    {
        // Setup Timings
        _tSunRise = config.sunRiseTime.GetT();
        _tSunSet = config.sunSetTime.GetT();
        currentTick = config.startTime.ToTicks();

        if (currentTick >= config.sunRiseTime.ToTicks() && 
            currentTick <= config.sunSetTime.ToTicks())
        {
            isDayTime = true;
        }
        else
        {
            isDayTime = false;
        }

        // Setup Configs
        configSample = new ConfigControl();
        configSample.SetupSun    (config.sunLight, _tSunRise, _tSunSet);
        configSample.SetupMoon   (config.moonLight, _tSunRise, _tSunSet);
        configSample.SetupSky    (config.sky, _tSunRise, _tSunSet);
        configSample.SetupCloud  (config.cloud, _tSunRise, _tSunSet);
        configSample.SetUpRotationCurve(_tSunRise, _tSunSet);
        configSample.SetUpLensFlare(_tSunRise, _tSunSet);
        configSample.SetUpWater(config.water, _tSunRise, _tSunSet);
    }

    private void OnSunSet(long ticks)
    {
        _sunFlare.enabled = false;
        Debug.Log($"Sun sets at {MyTime.ToOneDayTime(ticks)}");
    }
    private void OnSunRise(long ticks)
    {
        _sunFlare.enabled = true;
        Debug.Log($"Sun rises at {MyTime.ToOneDayTime(ticks)}");
    }
}
