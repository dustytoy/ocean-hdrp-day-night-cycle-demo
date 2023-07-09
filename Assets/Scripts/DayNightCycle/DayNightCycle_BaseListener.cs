using UnityEngine;

[DefaultExecutionOrder(0)]
public abstract class DayNightCycle_BaseListener : MonoBehaviour
{
    public DayNightCycle target;

    private void Awake()
    {
        target = DayNightCycle.Instance;
    }
    private void Start()
    {
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
    public virtual void OnSunrise(long currentTick) { }
    public virtual void OnSunset(long currentTick) { }
    public virtual void OnStartPostProcess() { }
    public virtual void OnDestroyPostProcess() { }
}
