using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Reference from https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@14.0/manual/WaterSystem-scripting.html

[DefaultExecutionOrder(100)]
public class FitToWaterSurface : MonoBehaviour
{
    public WaterSurface water;
    public float error = 0.01f;
    public int maxIterations = 8;

    // Sway behaviour 
    public bool sway;
    public float swayStrength;
    public float swaySpeed;

    private WaterSearchParameters _searchParams  = new WaterSearchParameters();
    private WaterSearchResult _searchResult = new WaterSearchResult();
    private FloatConfigOverride _override;
    private void Awake()
    {
        _override = GetComponent<FloatConfigOverride>();
    }

    private void Update()
    {
        if(water == null) return;
        // Build the search parameters
        _searchParams.startPosition = _searchResult.candidateLocation;
        _searchParams.targetPosition = transform.position;
        _searchParams.error = error;
        _searchParams.maxIterations = maxIterations;

        // Do the search
        if (water.FindWaterSurfaceHeight(_searchParams, out _searchResult))
        {
            float heightOffet = _override == null ? 0f : _override.heightOffset;
            transform.position = new Vector3(transform.position.x, _searchResult.height + heightOffet, transform.position.z);
            if(sway)
            {
                transform.rotation = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) *
                Quaternion.AngleAxis(Mathf.Cos(Time.time * swaySpeed) * swayStrength, Vector3.right) *
                Quaternion.AngleAxis(Mathf.Sin(Time.time * swaySpeed) * swayStrength, Vector3.forward);
            }
        }
        else Debug.LogError("Can't Find Height");

    }
}
