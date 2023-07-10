using UnityEditor;
using UnityEngine;

[DefaultExecutionOrder(0)]
public abstract class DayNightCycle_BaseComponent<TComponent,USettings> : MonoBehaviour where TComponent : UnityEngine.Object where USettings : ScriptableObject
{
    public DayNightCycle target;
    public TComponent component;
    public USettings settings;
    public string settingsName = "Default";

    private void Awake()
    {
        target = DayNightCycle.Instance;
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
        target.onTimeChanged += OnTimeChanged;
        target.onSunrise += OnSunrise;
        target.onSunset += OnSunset;
    }
    public void OnDestroyEventUnsubscribe()
    {
        target.onTimeChanged -= OnTimeChanged;
        target.onSunrise -= OnSunrise;
        target.onSunset -= OnSunset;
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
                GetDefaultAssetName());
#else
            settings = DayNightCycle.Instance.loadedBundle.LoadAsset<USettings>(GetDefaultAssetName());
#endif
        }
        else
        {
#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<USettings>(DayNightCycle.EDITOR_SETTINGS_FOLDER + GetRelativeSubfolderPath() +
                settingsName);
#else
            settings = DayNightCycle.Instance.loadedBundle.LoadAsset<USettings>(settingsName);
#endif
        }
    }
    public abstract void InitializeComponent(USettings settings);
}
