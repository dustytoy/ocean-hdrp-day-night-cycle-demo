#if !UNITY_EDITOR
using System.IO;
#endif
using UnityEditor;
using UnityEngine;

[DefaultExecutionOrder(int.MinValue)]
public partial class DayNightCycle : MonoBehaviour
{
    public static readonly string EDITOR_SETTINGS_FOLDER = "Assets/ScriptableObjects/DayNightCycle/";
    public static readonly string PLAYER_SETTINGS_FOLDER = "DayNightCycle/";
    // TODO: bundle all components data into asset bundle for distribution. And maybe share the same bundle
    public static readonly string DEFAULT_ASSET_BUNDLE_NAME = "DefaultDayNight";

    public string settingsName = "Default";
    public string assetBundleName = "Default";

    [HideInInspector]
    public DayNightCycle_SettingsSO settings;
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

    private void Awake()
    {
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
        if (settingsName == "Default")
        {
#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<DayNightCycle_SettingsSO>(EDITOR_SETTINGS_FOLDER + 
                "DayNightCycleSettings_Default.asset");
#else
            loadedBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, 
            DayNightCycle.PLAYER_SETTINGS_FOLDER, 
            assetBundleName == "Default" ? DayNightCycle.DEFAULT_ASSET_BUNDLE_NAME : assetBundleName));
            if (loadedBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }
            settings = loadedBundle.LoadAsset<DayNightCycleSettingsSO>("DayNightCycleSettings_Default");
#endif
        }
        else
        {
#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<DayNightCycle_SettingsSO>(EDITOR_SETTINGS_FOLDER +
                $"{settingsName}.asset");
#else
            loadedBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, 
            DayNightCycle.PLAYER_SETTINGS_FOLDER, 
            assetBundleName == "Default" ? DayNightCycle.DEFAULT_ASSET_BUNDLE_NAME : assetBundleName));
            if (loadedBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }
            settings = loadedBundle.LoadAsset<DayNightCycleSettingsSO>(settingsName);
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
        currentTick += (long)(Time.fixedDeltaTime * MyTimePerDay.TicksPerSecond * timeMultiplier);
        if(currentTick >= MyTimePerDay.TicksPerDay)
        {
            currentTick %= MyTimePerDay.TicksPerDay;
            _dayCount++;
        }
    }
   
    public void Initialize(DayNightCycle_SettingsSO so)
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
    }
}
