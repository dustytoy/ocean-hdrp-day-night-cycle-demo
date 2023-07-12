using UnityEngine;
using StarterAssets;

[DefaultExecutionOrder(int.MaxValue)]
public class OverrideHeight_ThirdPersonController : MonoBehaviour
{
    public ThirdPersonController controller;
    private FloatPlatform _platform;
    private void Update()
    {
        GroundedCheck();
        if (!controller.Grounded || _platform == null)
        {
            controller.transform.rotation = Quaternion.AngleAxis(controller.yAngle, Vector3.up);
            return;
        }
        controller.transform.rotation = Quaternion.AngleAxis(controller.yAngle, controller.contactNormal) *
            Quaternion.FromToRotation(Vector3.up, controller.contactNormal);
        controller.transform.position = controller.contactPoint +  controller.transform.rotation * controller.motion;
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(controller.transform.position.x, controller.transform.position.y - controller.GroundedOffset,
            controller.transform.position.z);
        var colliders = Physics.OverlapSphere(spherePosition, controller.GroundedRadius, controller.GroundLayers,
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
