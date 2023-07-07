using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[DefaultExecutionOrder(int.MinValue)]
public partial class DayNightCycle : MonoBehaviour
{
    public static readonly string EDITOR_SETTINGS_FOLDER = "Assets/ScriptableObjects/DayNightCycle/";
    public static readonly string PLAYER_SETTINGS_FOLDER = "DayNightCycle/";
    // TODO: bundle all components data into asset bundle for distribution. And maybe share the same bundle
    public static readonly string DEFAULT_ASSET_BUNDLE_NAME = "DefaultDayNight";

    public DayNightCycleSettingsSO settings;
    [HideInInspector]
    public AssetBundle loadedBundle;
    public int timeMultiplier
    {
        get { return _timeMultiplier; }
        set
        {
            bool changed = value != _currentTick;
            if (changed)
            {
                _timeMultiplier = value;
                onTimeMultiplierChanged?.Invoke(value);
            }
        }
    }
    public long currentTick
    {
        get { return _currentTick; }
        set
        {
            bool changed = value != _currentTick;
            if(changed)
            {
                _currentTick = value;
                onTimeChanged?.Invoke(value);
            }
        }
    }
    public int dayCount
    {
        get { return _dayCount; }
        set
        {
            bool changed = value != _dayCount;
            if (changed)
            {
                _dayCount = value;
                onDayChanged?.Invoke(value);
            }
        }
    }
    [HideInInspector]
    public bool isDayTime;
    [HideInInspector]
    public bool isPaused;

    public static DayNightCycle Instance
    {
        get { return _instance; }
    }
    private static DayNightCycle _instance;

    public delegate void OnTimeChanged(long currentTick);
    public delegate void OnTimeMultiplierChanged(int currentMultiplier);
    public delegate void OnDayChanged(int newDayCount);
    public delegate void SunSet(long currentTick);
    public delegate void SunRise(long currentTick);
    public event OnTimeChanged onTimeChanged;
    public event OnTimeMultiplierChanged onTimeMultiplierChanged;
    public event OnDayChanged onDayChanged;
    public event SunSet onSunset;
    public event SunSet onSunrise;

    private int _timeMultiplier;
    private long _currentTick;
    private long _sunriseTick;
    private long _sunsetTick;
    private float _tSunrise;
    private float _tSunset;
    private int _dayCount;


    //public Light sunLight;
    //public Light moonLight;
    //public Volume globalVolumn;
    //public PhysicallyBasedSky sky;
    //public VolumetricClouds clouds;
    //public Fog fog;
    //public WaterSurface water;

    //public DayNightConfig config;
    //public ConfigControl configSample;

    //private HDAdditionalLightData _sunData;
    //private LensFlareComponentSRP _sunFlare;
    //private HDAdditionalLightData _moonData;
    //private Transform _sunTransform;
    //private Transform _moonTransform;
    //private WindSpeedParameter.WindParamaterValue _overrideWindSpeed;

    private void Awake()
    {
        //globalVolumn.profile.TryGet<PhysicallyBasedSky>(out sky);
        //globalVolumn.profile.TryGet<VolumetricClouds>(out clouds);
        //globalVolumn.profile.TryGet<Fog>(out fog);
        //_sunData = sunLight.GetComponent<HDAdditionalLightData>();
        //_moonData = moonLight.GetComponent<HDAdditionalLightData>();
        //_sunFlare = _sunData.GetComponent<LensFlareComponentSRP>();
        //_sunTransform = sunLight.transform;
        //_moonTransform = moonLight.transform;
        //_overrideWindSpeed = new WindSpeedParameter.WindParamaterValue();

        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        // Load setting either from AssetDatabase or from AssetBundle
        if (settings == null)
        {
#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<DayNightCycleSettingsSO>(EDITOR_SETTINGS_FOLDER + 
                "DayNightCycleSettings_Default.asset");
#else
            loadedBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, 
            DayNightCycle.PLAYER_SETTINGS_FOLDER, 
            DayNightCycle.DEFAULT_ASSET_BUNDLE_NAME));
            if (loadedBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }
            settings = loadedBundle.LoadAsset<DayNightCycleSettingsSO>("DayNightCycleSettings_Default");
#endif
        }

        Initialize(settings);
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
#else
        loadedBundle.Unload(true);
#endif
    }

    private void FixedUpdate()
    {
        //float t = (float)currentTick / MyTimePerDay.TicksPerDay;
        //Evaluate(t);

        //water.timeMultiplier = timeMultiplier;
        //_overrideWindSpeed.customValue = timeMultiplier * 1;
        //clouds.globalWindSpeed.Override(_overrideWindSpeed);

        currentTick += (long)(Time.fixedDeltaTime * MyTimePerDay.TicksPerSecond * timeMultiplier);
        Debug.Log(currentTick);
        if(currentTick >= MyTimePerDay.TicksPerDay)
        {
            currentTick %= MyTimePerDay.TicksPerDay;
            _dayCount++;
        }
    }

//#if DEBUG
//    private GUIStyle _style = new GUIStyle();
//    private bool _displayGui;
//    private void OnGUI()
//    {
//        GUI.color = Color.white;
//        GUI.backgroundColor = Color.white;
//        _style.fontSize = 20;
//        _style.normal.textColor = Color.black;
//        _style.normal.background = Texture2D.grayTexture;
//        timeMultiplier = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(0f, 0f, 600f, 30f), (float)timeMultiplier, 1f, 50000f));
//        if (GUI.Button(new Rect(0f, 30f, 60f, 30f), "debug"))
//        {
//            _displayGui = !_displayGui;
//        }
//        if(!_displayGui ) { return; }

//        MyTimePerDay time = (MyTimePerDay)currentTick;
//        GUI.Label(new Rect(0f, 60f, 600f, 400f), $"SunRise: {config.sunriseTime} | Sunset:{config.sunsetTime} | Eastward: {Vector3.right}\n" +
//            $"Time (t): {time} ({((float)currentTick % MyTimePerDay.TicksPerDay) / MyTimePerDay.TicksPerDay})\n" +
//            $"TimeMultiplier: {timeMultiplier}\n" +
//            $"Sun           : {(_sunTransform.gameObject.activeInHierarchy ? "Active" : "Inactive")} (angle {(int)_sunTransform.eulerAngles.x} degrees)\n" +
//            $"Moon          : {(_moonTransform.gameObject.activeInHierarchy ? "Active" : "Inactive")} (angle {(int)_moonTransform.eulerAngles.x} degrees)\n" +
//            $"Lens Flare    : {_sunFlare.intensity}\n" +
//            $"Sky           : Zenith ({sky.zenithTint.value})\n" +
//            $"                Horizon ({sky.horizonTint.value})\n" +
//            $"Cloud         : Ambient Dimmer ({clouds.ambientLightProbeDimmer.value})\n" +
//            $"                Light Source Dimmer ({clouds.sunLightDimmer.value})\n" +
//            $"                Scattering ({clouds.scatteringTint.value})\n" +
//            $"                Custom Orientation ({clouds.orientation.value.customValue})\n" +
//            $"Fog           : Height (base {fog.baseHeight.value}, max {fog.maximumHeight.value})\n" +
//            $"Water         : Current ({water.largeCurrentSpeedValue}, angle {water.largeCurrentOrientationValue})\n" +
//            $"                Distant Wind ({water.largeWindSpeed}, angle {water.largeWindOrientationValue} degree)\n" +
//            $"                Local Wind ({water.ripplesWindSpeed}, angle {water.ripplesWindOrientationValue} degree)\n", _style);
//    }
//#endif
    //public void Evaluate(float t)
    //{
    //    EvaluateLight(t);
    //    EvaluateSky(t);
    //    EvaluateClouds(t);
    //    EvaluateWater(t);
    //    EvaluateFog(t);
    //}

    //public void EvaluateLight(float t)
    //{
    //    if((t >= _tSunrise && t <= _tSunset) && !isDayTime)
    //    {
    //        isDayTime = true;
    //        onSunrise?.Invoke(currentTick);
    //    }
    //    else if ((t < _tSunrise || t > _tSunset) && isDayTime)
    //    {
    //        isDayTime = false;
    //        onSunset?.Invoke(currentTick);
    //    }

    //    {
    //        var sunAngle = configSample.sunRotation.Evaluate(t) * 360f;
    //        var moonAngle = configSample.moonRotation.Evaluate(t) * 360f;
    //        _sunTransform.rotation = Quaternion.AngleAxis(-90f, Vector3.up) * Quaternion.AngleAxis(_dayCount * 360f + 270f + sunAngle, Vector3.right);
    //        _moonTransform.rotation = Quaternion.AngleAxis(-90f, Vector3.up) * Quaternion.AngleAxis(_dayCount * 360f + 90f + moonAngle, Vector3.right);
    //    }

    //    if(isDayTime)
    //    {
    //        _sunData.angularDiameter    = configSample.sunAngularDiameter.Evaluate(t);
    //        _sunData.flareSize          = configSample.sunFlareSize.Evaluate(t);
    //        _sunData.flareFalloff       = configSample.sunFlareFalloff.Evaluate(t);
    //        _sunData.flareTint          = configSample.sunFlareTint.Evaluate(t);
    //        _sunData.surfaceTint        = configSample.sunSurfaceTint.Evaluate(t);

    //        _sunData.color      = configSample.sunEmissionColor.Evaluate(t);
    //        _sunData.intensity  = configSample.sunIntensity.Evaluate(t);

    //        _sunFlare.intensity = configSample.sunLensFlare.Evaluate(t);
    //        _sunFlare.scale     = configSample.sunLensFlare.Evaluate(t);
    //    }
    //    else
    //    {
    //        _moonData.angularDiameter   = configSample.moonAngularDiameter.Evaluate(t);
    //        _moonData.flareSize         = configSample.moonFlareSize.Evaluate(t);
    //        _moonData.flareFalloff      = configSample.moonFlareFalloff.Evaluate(t);
    //        _moonData.flareTint         = configSample.moonFlareTint.Evaluate(t);
    //        _moonData.surfaceTint       = configSample.moonSurfaceTint.Evaluate(t);

    //        _moonData.color     = configSample.moonEmissionColor.Evaluate(t);
    //        _moonData.intensity = configSample.moonIntensity.Evaluate(t);
    //    }
    //}
    //public void EvaluateSky(float t)
    //{
    //    sky.horizonTint.value   = configSample.skyHorizonTint.Evaluate(t);
    //    sky.zenithTint.value    = configSample.skyZenithTint.Evaluate(t);
    //}
    //public void EvaluateClouds(float t)
    //{
    //    clouds.ambientLightProbeDimmer.value    = configSample.cloudAmbientDimmer.Evaluate(t);
    //    clouds.sunLightDimmer.value             = configSample.cloudLightDimmer.Evaluate(t);
    //    clouds.scatteringTint.value             = configSample.cloudScatteringTint.Evaluate(t);
    //}
    //public void EvaluateWater(float t)
    //{
    //    water.largeWindSpeed    = configSample.waterDistantWindSpeed.Evaluate(t);
    //    water.ripplesWindSpeed  = configSample.waterLocalWindSpeed.Evaluate(t);
    //    water.refractionColor   = configSample.waterRefractingColor.Evaluate(t);
    //    water.scatteringColor   = configSample.waterScatteringColor.Evaluate(t);
    //}
    //public void EvaluateFog(float t)
    //{
    //    fog.maximumHeight.value = configSample.fogMaxHeight.Evaluate(t);
    //}
    public void Initialize(DayNightCycleSettingsSO so)
    {
        // Setup timings
        _tSunrise = so.sunriseTime.GetT();
        _tSunset = so.sunsetTime.GetT();
        _sunriseTick = (long)so.sunriseTime;
        _sunsetTick = (long)so.sunsetTime;
        currentTick = (long)so.startTime;
        timeMultiplier = 1;
        if (currentTick >= (long)so.sunriseTime && 
            currentTick <= (long)so.sunsetTime)
        {
            isDayTime = true;
        }
        else
        {
            isDayTime = false;
        }

        // Setup Configs
        //configSample = new ConfigControl();
        //configSample.SetupSun    (config.sunLight, _tSunrise, _tSunset);
        //configSample.SetupMoon   (config.moonLight, _tSunrise, _tSunset);
        //configSample.SetupSky    (config.sky, _tSunrise, _tSunset);
        //configSample.SetupCloud  (config.cloud, _tSunrise, _tSunset);
        //configSample.SetUpRotationCurve(_tSunrise, _tSunset);
        //configSample.SetUpLensFlare(_tSunrise, _tSunset);
        //configSample.SetUpWater(config.water, _tSunrise, _tSunset);
        //configSample.SetUpFogHeight(config.fog, _tSunrise, _tSunset);
    }
}
