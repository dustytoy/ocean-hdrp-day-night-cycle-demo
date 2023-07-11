#if UNITY_EDITOR
using UnityEditor;
using System;

public abstract class DayNightCycle_BaseEditor : Editor
{
    public static DayNightCycle sharedDayNightCycle;
    public static DayNightCycleController sharedController;

    public delegate void OnEditModeOn();
    public delegate void OnEditModeOff();
    public static event OnEditModeOn onEditModeOn;
    public static event OnEditModeOff onEditModeOff;

    public static bool sharedEditMode
    {
        get
        {
            return _sharedEditMode;
        }
        set
        {
            _sharedEditMode = value;
            if (_sharedEditMode)
            {
                onEditModeOn?.Invoke();
            }
            else
            {
                onEditModeOff?.Invoke();
            }
        }
    }
    private static bool _sharedEditMode = true;

    public void OnEnable()
    {
        if (sharedDayNightCycle == null)
        {
            sharedDayNightCycle = FindObjectOfType<DayNightCycle>();
            if (sharedDayNightCycle == null)
            {
                throw new Exception("There is no DayNightCycle component in the scene");
            }
        }
        if (sharedController == null)
        {
            sharedController = FindObjectOfType<DayNightCycleController>();
            if (sharedController == null)
            {
                throw new Exception("There is no DayNightCycleController component in the scene");
            }
        }
        OnEnable_Impl();
    }
    public void OnDisable()
    {
        OnDisable_Impl();
    }
    public void OnSceneGUI()
    {
        OnSceneGUI_Impl();
    }
    public override void OnInspectorGUI()
    {
        OnInspectorGUI_Impl();
    }
    public virtual void OnInspectorGUI_Impl() { }
    public virtual void OnEnable_Impl() { }
    public virtual void OnDisable_Impl() { }
    public virtual void OnSceneGUI_Impl() { }

}
#endif
