using StarterAssets;
using UnityEngine;

[DefaultExecutionOrder(102)]
public class OverrideRotation_ThirdPersonController : MonoBehaviour
{
    public ThirdPersonController controller;
    private FloatPlatform _platform;
    private float yAngle;
    private void LateUpdate()
    {
        //Debug.Log($"Local: {controller.transform.localEulerAngles}");
        //Debug.Log($"{controller.transform.eulerAngles}");
        if (!controller.Grounded || _platform == null)
        {
            controller.transform.rotation = Quaternion.AngleAxis(controller.transform.eulerAngles.y, Vector3.up);
            return;
        }
        GroundedCheck();
        //controller.transform.rotation = Quaternion.AngleAxis(controller.transform.eulerAngles.y, controller.contactNormal);
        controller.transform.rotation = Quaternion.AngleAxis(controller.transform.eulerAngles.y, controller.contactNormal) * 
            Quaternion.FromToRotation(Vector3.up, controller.contactNormal);
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
