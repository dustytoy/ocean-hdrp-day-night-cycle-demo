using StarterAssets;
using UnityEngine;

[DefaultExecutionOrder(202)]
public class OverrideRotation_FirstPersonController : MonoBehaviour
{
    public FirstPersonController controller;
    private FloatPlatform _platform;
    private void Update()
    {
        if (controller == null || !controller.Grounded)
        {
            controller.transform.rotation = Quaternion.AngleAxis(controller.transform.eulerAngles.y, Vector3.up);
            return;
        }
        GroundedCheck();
        //controller.transform.rotation = Quaternion.AngleAxis(controller.transform.eulerAngles.y, Vector3.up) * _platform.resultFromFitToWaterSurface.rotatedBy;
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = controller.transform.position - controller.transform.up * 0.1f;
        var colliders = Physics.OverlapSphere(spherePosition, controller.GroundedRadius * 0.05f, controller.GroundLayers,
            QueryTriggerInteraction.Ignore);
        if (colliders != null && colliders.Length > 0)
        {
            _platform = colliders[0].GetComponent<FloatPlatform>();
        }
        else
        {
            _platform = null;
        }
    }
}
