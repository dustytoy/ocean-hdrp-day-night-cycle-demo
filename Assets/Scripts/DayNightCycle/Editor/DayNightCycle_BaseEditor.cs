#if UNITY_EDITOR
using UnityEditor;
using System;

public abstract class DayNightCycle_BaseEditor : Editor
{
    public static DayNightCycle sharedDayNightCycle;

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
            if(_sharedEditMode)
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
    private bool _showBaseInspector = false;

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
    }
    public override void OnInspectorGUI()
    {
        sharedDayNightCycle = EditorGUILayout.ObjectField(sharedDayNightCycle, typeof(DayNightCycle), true) as DayNightCycle;
        EditMode();
        Addition_Impl();
        //_showBaseInspector = EditorGUILayout.BeginFoldoutHeaderGroup(_showBaseInspector, "Base Inspector");
        _showBaseInspector = EditorGUILayout.BeginToggleGroup("Base Inspector", _showBaseInspector);
        if(_showBaseInspector)
        {
            base.OnInspectorGUI();
        }
        //EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.EndToggleGroup();
    }
    public void EditMode()
    {
        sharedEditMode = EditorGUILayout.BeginToggleGroup("Shared Edit Mode", sharedEditMode);
        if (sharedEditMode)
        {
            EditMode_Impl();
        }
        EditorGUILayout.EndToggleGroup();

    }
    public virtual void EditMode_Impl() { }
    public virtual void Addition_Impl() { }
}
#endif
