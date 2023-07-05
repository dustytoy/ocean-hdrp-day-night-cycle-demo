using UnityEngine;

[DefaultExecutionOrder(101)]
[RequireComponent(typeof(BoxCollider))]
public class FloatPlatform : MonoBehaviour
{
    [HideInInspector]
    public float height;
    private BoxCollider _box;

    private void Awake()
    {
        _box= GetComponent<BoxCollider>();
    }

    private void Update()
    {
        height = transform.position.y + _box.center.y + transform.lossyScale.y * _box.size.y / 2f;
    }
}
