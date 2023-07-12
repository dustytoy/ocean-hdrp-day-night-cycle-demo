using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Reference from https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@14.0/manual/WaterSystem-scripting.html
[DefaultExecutionOrder(100)]
public class FitToWaterSurface_Burst : MonoBehaviour
{
    // Public parameters
    public WaterSurface waterSurface = null;

    // List of transform to move
    public Transform[] moveTransform;

    // Sway behaviour 
    public bool sway;
    public float swayStrength;
    public float swaySpeed;

    // Input job parameters
    NativeArray<float3> _targetPositionBuffer;

    // Output job parameters
    NativeArray<float> _heightBuffer;
    NativeArray<float> _errorBuffer;
    NativeArray<float3> _candidatePositionBuffer;
    NativeArray<int> _stepCountBuffer;

    void Start()
    {
        // Allocate the buffers
        int count = moveTransform.Length;
        _targetPositionBuffer = new NativeArray<float3>(count, Allocator.Persistent);
        _heightBuffer = new NativeArray<float>(count, Allocator.Persistent);
        _errorBuffer = new NativeArray<float>(count, Allocator.Persistent);
        _candidatePositionBuffer = new NativeArray<float3>(count, Allocator.Persistent);
        _stepCountBuffer = new NativeArray<int>(count, Allocator.Persistent);
    }

    void Update()
    {
        if (waterSurface == null || moveTransform == null || moveTransform.Length == 0)
            return;
        // Try to get the simulation data if available
        WaterSimSearchData simData = new WaterSimSearchData();
        if (!waterSurface.FillWaterSearchData(ref simData))
            return;

        // Fill the input positions
        int numElements = moveTransform.Length;
        for (int i = 0; i < numElements; ++i)
            _targetPositionBuffer[i] = moveTransform[i].transform.position;

        // Prepare the first band
        WaterSimulationSearchJob searchJob = new WaterSimulationSearchJob();

        // Assign the simulation data
        searchJob.simSearchData = simData;

        // Fill the input data
        searchJob.targetPositionBuffer = _targetPositionBuffer;
        searchJob.startPositionBuffer = _targetPositionBuffer;
        searchJob.maxIterations = 8;
        searchJob.error = 0.01f;

        searchJob.heightBuffer = _heightBuffer;
        searchJob.errorBuffer = _errorBuffer;
        searchJob.candidateLocationBuffer = _candidatePositionBuffer;
        searchJob.stepCountBuffer = _stepCountBuffer;

        // Schedule the job with one Execute per index in the results array and only 1 item per processing batch
        JobHandle handle = searchJob.Schedule(numElements, 1);
        handle.Complete();

        // Fill the input positions
        for (int i = 0; i < numElements; ++i)
        {
            var floatOverride = moveTransform[i].GetComponent<FloatConfigOverride>();
            var heightOffset = floatOverride == null ? 0 : floatOverride.heightOffset;

            Vector3 newPos = new Vector3(moveTransform[i].transform.position.x, _heightBuffer[i] + heightOffset, moveTransform[i].transform.position.z);
            Quaternion newRot = Quaternion.AngleAxis(moveTransform[i].eulerAngles.y, Vector3.up) *
                Quaternion.AngleAxis(Mathf.Cos(Time.time * swaySpeed) * swayStrength, Vector3.right) *
                Quaternion.AngleAxis(Mathf.Sin(Time.time * swaySpeed) * swayStrength, Vector3.forward);
            moveTransform[i].transform.position = newPos;
            if (sway)
            {
                moveTransform[i].rotation = newRot;
            }
            else
            {
                moveTransform[i].rotation = Quaternion.AngleAxis(moveTransform[i].eulerAngles.y, Vector3.up);
            }
        }
    }

    private void OnDestroy()
    {
        _targetPositionBuffer.Dispose();
        _heightBuffer.Dispose();
        _errorBuffer.Dispose();
        _candidatePositionBuffer.Dispose();
        _stepCountBuffer.Dispose();
    }
}

