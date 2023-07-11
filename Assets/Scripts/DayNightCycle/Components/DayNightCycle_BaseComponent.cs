using UnityEditor;
using UnityEngine;

[DefaultExecutionOrder(0)]
public abstract class DayNightCycle_BaseComponent<TComponent,USettings> : MonoBehaviour where TComponent : UnityEngine.Object where USettings : ScriptableObject
{
    [HideInInspector]
    public DayNightCycle dayNightCycle;
    [HideInInspector]
    public DayNightCycleController controller;
    [HideInInspector]
    public TComponent component;
    [HideInInspector]
    public USettings settings;
    [HideInInspector]
    public string settingsName = "Default";

    private void Awake()
    {
        dayNightCycle = DayNightCycle.Instance;
        controller = DayNightCycleController.Instance;
        controller.components.Add(this);
    }
    private void Start()
    {
        InitializeSettings();
        InitializeComponent(settings);
        OnStartEventSubscribe();
        OnStartPostProcess();
    }
    private void OnDestroy()
    {
        OnDestroyEventUnsubscribe();
        OnDestroyPostProcess();
    }
    public void OnStartEventSubscribe()
    {
        dayNightCycle.onTimeChanged += OnTimeChanged;
        dayNightCycle.onSunrise += OnSunrise;
        dayNightCycle.onSunset += OnSunset;
    }
    public void OnDestroyEventUnsubscribe()
    {
        dayNightCycle.onTimeChanged -= OnTimeChanged;
        dayNightCycle.onSunrise -= OnSunrise;
        dayNightCycle.onSunset -= OnSunset;
    }
    public abstract void OnTimeChanged(long currentTick);
    public abstract string GetRelativeSubfolderPath();
    public abstract string GetDefaultAssetName();
    public virtual void OnSunrise(long currentTick) { }
    public virtual void OnSunset(long currentTick) { }
    public virtual void OnStartPostProcess() { }
    public virtual void OnDestroyPostProcess() { }
    public virtual void InitializeSettings()
    {
        if(settingsName == "Default")
        {
#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<USettings>(DayNightCycle.EDITOR_SETTINGS_FOLDER + GetRelativeSubfolderPath() +
                GetDefaultAssetName() + ".asset");
#else
            settings = DayNightCycle.Instance.loadedBundle.LoadAsset<USettings>(GetDefaultAssetName());
#endif
        }
        else
        {
#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<USettings>(DayNightCycle.EDITOR_SETTINGS_FOLDER + GetRelativeSubfolderPath() +
                settingsName + ".asset");
#else
            settings = DayNightCycle.Instance.loadedBundle.LoadAsset<USettings>(settingsName);
#endif
        }
    }
    public abstract void InitializeComponent(USettings settings);
}
