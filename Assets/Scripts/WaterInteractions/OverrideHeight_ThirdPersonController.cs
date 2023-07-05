using UnityEngine;
using StarterAssets;

[DefaultExecutionOrder(201)]
public class OverrideHeight_ThirdPersonController : MonoBehaviour
{
    public ThirdPersonController controller;
    private FloatPlatform _platform;
    private void Update()
    {
        if (controller == null || !controller.Grounded) { return; }
        GroundedCheck();
        Vector3 pos = controller.transform.position;
        controller.transform.position = new Vector3(pos.x, _platform == null ? pos.y : _platform.height, pos.z);
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(controller.transform.position.x, controller.transform.position.y - 0.0001f,
            controller.transform.position.z);
        var colliders = Physics.OverlapSphere(spherePosition, controller.GroundedRadius * 0.00005f, controller.GroundLayers,
            QueryTriggerInteraction.Ignore);
        if(colliders != null && colliders.Length > 0)
        {
            _platform = colliders[0].GetComponent<FloatPlatform>();
        }
        else
        {
            _platform = null;
        }
    }
}
