using UnityEngine;

[DefaultExecutionOrder(0)]
public abstract class DayNightCycleListener : MonoBehaviour
{
    protected DayNightCycle target;

    private void Awake()
    {
        target = DayNightCycle.Instance;
    }
    private void Start()
    {
        target.onTimeChanged += OnTimeChanged;
        target.onSunrise += OnSunrise;
        target.onSunset += OnSunset;
        OnStartPostProcess();
    }
    private void OnDestroy()
    {
        target.onTimeChanged -= OnTimeChanged;
        target.onSunrise -= OnSunrise;
        target.onSunset -= OnSunset;
        OnDestroyPostProcess();
    }
    public abstract void OnTimeChanged(long currentTick);
    public virtual void OnSunrise(long currentTick) { }
    public virtual void OnSunset(long currentTick) { }
    public virtual void OnStartPostProcess() { }
    public virtual void OnDestroyPostProcess() { }
}
