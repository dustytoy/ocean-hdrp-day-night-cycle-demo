using System;
using UnityEditor.Rendering;
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
    public Fog fog;
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

    private void Awake()
    {
        globalVolumn.profile.TryGet<PhysicallyBasedSky>(out sky);
        globalVolumn.profile.TryGet<VolumetricClouds>(out clouds);
        globalVolumn.profile.TryGet<Fog>(out fog);
        _sunData = sunLight.GetComponent<HDAdditionalLightData>();
        _moonData = moonLight.GetComponent<HDAdditionalLightData>();
        _sunFlare = _sunData.GetComponent<LensFlareComponentSRP>();
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

#if DEBUG
    private GUIStyle _style = new GUIStyle();
    private bool _displayGui;
    private void OnGUI()
    {
        GUI.color = Color.white;
        GUI.backgroundColor = Color.white;
        _style.fontSize = 20;
        _style.normal.textColor = Color.black;
        _style.normal.background = Texture2D.grayTexture;
        gameSecondPerRealSecond = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(0f, 0f, 600f, 30f), (float)gameSecondPerRealSecond, 1f, 10000f));
        if (GUI.Button(new Rect(0f, 30f, 60f, 30f), "debug"))
        {
            _displayGui = !_displayGui;
        }
        if(!_displayGui ) { return; }

        MyTime time = MyTime.ToOneDayTime(currentTick);
        GUI.Label(new Rect(0f, 60f, 600f, 400f), $"SunRise: {config.sunRiseTime} | Sunset:{config.sunSetTime} | Eastward: {Vector3.right}\n" +
            $"Time (t): {time} ({((float)currentTick % MyTime.TotalTicks) / MyTime.TotalTicks})\n" +
            $"TimeMultiplier: {gameSecondPerRealSecond}\n" +
            $"Sun           : {(_sunTransform.gameObject.activeInHierarchy ? "Active" : "Inactive")} (angle {(int)_sunTransform.eulerAngles.x} degrees)\n" +
            $"Moon          : {(_moonTransform.gameObject.activeInHierarchy ? "Active" : "Inactive")} (angle {(int)_moonTransform.eulerAngles.x} degrees)\n" +
            $"Lens Flare    : {_sunFlare.intensity}\n" +
            $"Sky           : Zenith ({sky.zenithTint.value})\n" +
            $"                Horizon ({sky.horizonTint.value})\n" +
            $"Cloud         : Ambient Dimmer ({clouds.ambientLightProbeDimmer.value})\n" +
            $"                Light Source Dimmer ({clouds.sunLightDimmer.value})\n" +
            $"                Scattering ({clouds.scatteringTint.value})\n" +
            $"                Custom Orientation ({clouds.orientation.value.customValue})\n" +
            $"Fog           : Height (base {fog.baseHeight.value}, max {fog.maximumHeight.value})\n" +
            $"Water         : Current ({water.largeCurrentSpeedValue}, angle {water.largeCurrentOrientationValue})\n" +
            $"                Distant Wind ({water.largeWindSpeed}, angle {water.largeWindOrientationValue} degree)\n" +
            $"                Local Wind ({water.ripplesWindSpeed}, angle {water.ripplesWindOrientationValue} degree)\n", _style);
    }
#endif
    public void Evaluate(float t)
    {
        EvaluateLight(t);
        EvaluateSky(t);
        EvaluateClouds(t);
        EvaluateWater(t);
        EvaluateFog(t);
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
            var sunAngle = configSample.sunRotation.Evaluate(t) * 360f;
            var moonAngle = configSample.moonRotation.Evaluate(t) * 360f;
            _sunTransform.rotation = Quaternion.AngleAxis(-90f, Vector3.up) * Quaternion.AngleAxis(_dayCount * 360f + 270f + sunAngle, Vector3.right);
            _moonTransform.rotation = Quaternion.AngleAxis(-90f, Vector3.up) * Quaternion.AngleAxis(_dayCount * 360f + 90f + moonAngle, Vector3.right);
        }

        if(isDayTime)
        {
            var celes = config.sunLight.celestialBody;
            _sunData.angularDiameter    = configSample.sunAngularDiameter.Evaluate(t);
            _sunData.flareSize          = configSample.sunFlareSize.Evaluate(t);
            _sunData.flareFalloff       = configSample.sunFlareFalloff.Evaluate(t);
            _sunData.flareTint          = configSample.sunFlareTint.Evaluate(t);
            _sunData.surfaceTint        = configSample.sunSurfaceTint.Evaluate(t);

            var emis = config.sunLight.lightEmission;
            _sunData.color      = configSample.sunEmissionColor.Evaluate(t);
            _sunData.intensity  = configSample.sunIntensity.Evaluate(t);

            _sunFlare.intensity = configSample.sunLensFlare.Evaluate(t);
            _sunFlare.scale     = configSample.sunLensFlare.Evaluate(t);
        }
        else
        {
            var celes = config.moonLight.celestialBody;
            _moonData.angularDiameter   = configSample.moonAngularDiameter.Evaluate(t);
            _moonData.flareSize         = configSample.moonFlareSize.Evaluate(t);
            _moonData.flareFalloff      = configSample.moonFlareFalloff.Evaluate(t);
            _moonData.flareTint         = configSample.moonFlareTint.Evaluate(t);
            _moonData.surfaceTint       = configSample.moonSurfaceTint.Evaluate(t);

            var emis = config.moonLight.lightEmission;
            _moonData.color     = configSample.moonEmissionColor.Evaluate(t);
            _moonData.intensity = configSample.moonIntensity.Evaluate(t);
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
        clouds.ambientLightProbeDimmer.value    = configSample.cloudAmbientDimmer.Evaluate(t);
        clouds.sunLightDimmer.value             = configSample.cloudLightDimmer.Evaluate(t);
        clouds.scatteringTint.value             = configSample.cloudScatteringTint.Evaluate(t);
    }
    public void EvaluateWater(float t)
    {
        water.largeWindSpeed    = configSample.waterDistantWindSpeed.Evaluate(t);
        water.ripplesWindSpeed  = configSample.waterLocalWindSpeed.Evaluate(t);
        water.refractionColor   = configSample.waterRefractingColor.Evaluate(t);
        water.scatteringColor   = configSample.waterScatteringColor.Evaluate(t);
    }
    public void EvaluateFog(float t)
    {
        fog.maximumHeight.value = configSample.fogMaxHeight.Evaluate(t);
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
        configSample.SetUpFogHeight(config.fog, _tSunRise, _tSunSet);
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
