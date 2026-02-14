using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -5f);
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Fixed Angle")]
    [SerializeField] private Vector3 fixedRotation = new Vector3(60f, 0f, 0f);

    private void Start()
    {
        // Apply fixed rotation
        transform.rotation = Quaternion.Euler(fixedRotation);

        // Auto-find player if not assigned
        if (target == null)
        {
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
