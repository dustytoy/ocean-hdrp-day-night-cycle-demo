#if UNITY_EDITOR

using System.Reflection;
using UnityEditor;
using UnityEngine;

public abstract class DayNightCycle_BaseComponentEditor<D,T,U> : DayNightCycle_BaseEditor where D : DayNightCycle_BaseComponent<T, U> where T : UnityEngine.Object where U : UnityEngine.ScriptableObject
{
    protected Editor cachedSettingEditor;
    protected float transitionSize;
    protected string cloneName = "Clone";
    private new void OnEnable()
    {
        base.OnEnable();
        var component = target as D;
        component.dayNightCycle = sharedDayNightCycle;
        component.controller = sharedController;
        onEditModeOn += () =>
        {
            component.OnStartEventSubscribe();
        };
        onEditModeOff += () =>
        {
            component.OnDestroyEventUnsubscribe();
        };
        component.InitializeSettings();
        component.InitializeComponent(component.settings);
        component.OnStartPostProcess();
    }

    public override void EditMode_Impl()
    {
        var component = target as D;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"SettingsSO:");
        component.settings = EditorGUILayout.ObjectField(component.settings, typeof(U), false) as U;
        EditorGUILayout.EndHorizontal();

        if (component.settings != null)
        {
            CreateCachedEditor(component.settings, null, ref cachedSettingEditor);
            cachedSettingEditor.OnInspectorGUI();
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"TransitionSize:");
        transitionSize = EditorGUILayout.Slider(transitionSize, 0f, 1f);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        bool resetKeyframes = GUILayout.Button("Reset Keyframes");
        bool readjustKeyframes = GUILayout.Button("Readjust Keyframes");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"CloneName:");
        cloneName = EditorGUILayout.TextField(cloneName);
        EditorGUILayout.EndHorizontal();
        bool cloneSettingsSO = GUILayout.Button("Clone Settings");
        if (cloneName == string.Empty)
        {
            cloneName = "Clone";
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"SettingsName:");
        component.settingsName = EditorGUILayout.TextField(component.settingsName);
        EditorGUILayout.EndHorizontal();
        bool reInitializeSettings = GUILayout.Button("ReinitializeSettings");
        if (component.settingsName == string.Empty)
        {
            component.settingsName = "Default";
        }

        if (resetKeyframes)
        {
            ResetKeyframesAll();
        }
        if (readjustKeyframes)
        {
            ReadjustKeyframesAll();
        }
        else if(cloneSettingsSO)
        {
            U clone = ScriptableObject.Instantiate(component.settings);
            string path = DayNightCycle.EDITOR_SETTINGS_FOLDER + component.GetRelativeSubfolderPath() + cloneName + ".asset";
            AssetDatabase.CreateAsset(clone, path);
            component.settingsName = cloneName;
            component.settings = clone;
        }
        else if(reInitializeSettings)
        {
            
            component.InitializeSettings();
            component.InitializeComponent(component.settings);
            component.OnStartPostProcess();
        }
    }
    public void ReadjustKeyframesAll()
    {
        var component = target as D;

        float tSunRise = sharedDayNightCycle.settings.sunriseTime.GetT();
        float tSunSet = sharedDayNightCycle.settings.sunsetTime.GetT();
        float[] ts = new float[]
        {
                0f,
                Mathf.Clamp(tSunRise - transitionSize, 0.0f, 1.0f),
                tSunRise,
                Mathf.Clamp(tSunRise + transitionSize, 0.0f, 1.0f),
                0.5f,
                Mathf.Clamp(tSunSet - transitionSize, 0.0f, 1.0f),
                tSunSet,
                Mathf.Clamp(tSunSet + transitionSize, 0.0f, 1.0f),
                1f
        };
        foreach (var field in component.settings.GetType().GetFields())
        {
            ReadjustKeyframes(field, ref ts);
        }
    }

    public void ResetKeyframesAll()
    {
        var component = target as D;

        float tSunRise = sharedDayNightCycle.settings.sunriseTime.GetT();
        float tSunSet = sharedDayNightCycle.settings.sunsetTime.GetT();
        float[] ts = new float[]
        {
                0f,
                Mathf.Clamp(tSunRise - transitionSize, 0.0f, 1.0f),
                tSunRise,
                Mathf.Clamp(tSunRise + transitionSize, 0.0f, 1.0f),
                0.5f,
                Mathf.Clamp(tSunSet - transitionSize, 0.0f, 1.0f),
                tSunSet,
                Mathf.Clamp(tSunSet + transitionSize, 0.0f, 1.0f),
                1f
        };
        foreach (var field in component.settings.GetType().GetFields())
        {
            ResetKeyframes(field, ref ts);
        }
    }
    public void ReadjustKeyframes(FieldInfo field, ref float[] ts)
    {
        var component = target as D;

        if (field.FieldType == typeof(AnimationCurve))
        {
            AnimationCurve curve = field.GetValue(component.settings) as AnimationCurve;
            for(int i = 0; i < ts.Length; i++)
            {
                var key = curve.keys[i];
                key.time = ts[i];
            }
        }
        else if (field.FieldType == typeof(Gradient))
        {
            Gradient gradient = field.GetValue(component.settings) as Gradient;
            for (int i = 0; i < ts.Length; i++)
            {
                if(i % 2 != 0) { continue; }
                var alphaKey = gradient.alphaKeys[i / 2];
                var colorKey = gradient.colorKeys[i / 2];
                alphaKey.time = ts[i];
                colorKey.time = ts[i];
            }
        }
    }

    public void ResetKeyframes(FieldInfo field, ref float[] ts)
    {
        var component = target as D;

        if (field.FieldType == typeof(AnimationCurve))
        {
            Keyframe[] keys = new Keyframe[]
            {
                        new Keyframe(ts[0], 1.0f),
                        new Keyframe(ts[1], 1.0f),
                        new Keyframe(ts[2], 1.0f),
                        new Keyframe(ts[3], 1.0f),
                        new Keyframe(ts[4], 1.0f),
                        new Keyframe(ts[5], 1.0f),
                        new Keyframe(ts[6], 1.0f),
                        new Keyframe(ts[7], 1.0f),
                        new Keyframe(ts[8], 1.0f)
            };
            AnimationCurve curve = new AnimationCurve(keys);
            field.SetValue(component.settings, curve);
        }
        else if (field.FieldType == typeof(Gradient))
        {
            var gradient = new Gradient();
            gradient.mode = GradientMode.Blend;
            gradient.colorSpace = ColorSpace.Linear;
            gradient.alphaKeys = new GradientAlphaKey[]
            {
                        new GradientAlphaKey(1, ts[0]),
                        new GradientAlphaKey(1, ts[2]),
                        new GradientAlphaKey(1, ts[4]),
                        new GradientAlphaKey(1, ts[6]),
                        new GradientAlphaKey(1, ts[8])

            };
            gradient.colorKeys = new GradientColorKey[]
            {
                        new GradientColorKey(Color.white,ts[0]),
                        new GradientColorKey(Color.white,ts[2]),
                        new GradientColorKey(Color.white,ts[4]),
                        new GradientColorKey(Color.white,ts[6]),
                        new GradientColorKey(Color.white,ts[8])
            };
            field.SetValue(component.settings, gradient);
        }
    }
}
#endif
