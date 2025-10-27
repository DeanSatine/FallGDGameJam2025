using UnityEngine;

public class MenuCameraOrbit : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset = Vector3.zero;

    [Header("Orbit Settings")]
    [SerializeField] private float orbitSpeed = 10f;
    [SerializeField] private float orbitRadius = 10f;
    [SerializeField] private float height = 5f;

    [Header("Look Settings")]
    [SerializeField] private bool lookAtTarget = true;
    [SerializeField] private float lookSmoothness = 5f;

    [Header("Tilt Settings")]
    [SerializeField] private bool enableTilt = false;
    [SerializeField] private float tiltAmount = 5f;
    [SerializeField] private float tiltSpeed = 2f;

    private float currentAngle = 0f;
    private float tiltOffset = 0f;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("MenuCameraOrbit: No target assigned! Camera will orbit around world origin.");
        }

        currentAngle = 0f;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = target != null ? target.position + targetOffset : targetOffset;

        currentAngle += orbitSpeed * Time.deltaTime;
        if (currentAngle >= 360f)
        {
            currentAngle -= 360f;
        }

        if (enableTilt)
        {
            tiltOffset = Mathf.Sin(Time.time * tiltSpeed) * tiltAmount;
        }

        float radians = currentAngle * Mathf.Deg2Rad;
        float x = targetPosition.x + Mathf.Cos(radians) * orbitRadius;
        float z = targetPosition.z + Mathf.Sin(radians) * orbitRadius;
        float y = targetPosition.y + height + tiltOffset;

        transform.position = new Vector3(x, y, z);

        if (lookAtTarget)
        {
            Vector3 direction = targetPosition - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookSmoothness * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 targetPosition = target != null ? target.position + targetOffset : targetOffset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPosition, 0.5f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(targetPosition, orbitRadius);

        Gizmos.color = Color.green;
        Vector3 heightPos = targetPosition + Vector3.up * height;
        Gizmos.DrawLine(targetPosition, heightPos);
        Gizmos.DrawWireSphere(heightPos, 0.3f);
    }
}
