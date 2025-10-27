using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.6f, 0f);
    [SerializeField] private float followSpeed = 10f;

    [Header("Camera Shake Settings")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeMagnitude = 0.1f;
    [SerializeField] private float shakeRoughness = 10f;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilAmount = 2f;
    [SerializeField] private float recoilSpeed = 8f;

    [Header("FOV Effects")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float damageFOV = 70f;
    [SerializeField] private float fovChangeSpeed = 5f;

    private Vector3 originalPosition;
    private float shakeTimer = 0f;
    private float recoilRotation = 0f;
    private float targetFOV;
    private bool isDamageFOV = false;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponent<Camera>();
        }

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        targetFOV = normalFOV;
        if (playerCamera != null)
        {
            playerCamera.fieldOfView = normalFOV;
        }

        originalPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            FollowTarget();
        }

        ApplyShake();
        ApplyRecoilEffect();
        ApplyFOVEffect();
    }

    private void FollowTarget()
    {
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }

    public void TriggerShake(float duration = -1f, float magnitude = -1f)
    {
        shakeTimer = duration > 0 ? duration : shakeDuration;
        float shakeMag = magnitude > 0 ? magnitude : shakeMagnitude;

        StartCoroutine(ShakeCoroutine(shakeTimer, shakeMag));
    }

    private System.Collections.IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;
            float damper = 1f - Mathf.Clamp01(4f * percentComplete - 3f);

            float x = Random.value * 2f - 1f;
            float y = Random.value * 2f - 1f;
            x *= magnitude * damper;
            y *= magnitude * damper;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    private void ApplyShake()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
        }
    }

    public void ApplyRecoil(float upwardKick = 0.3f, float duration = 0.15f)
    {
        recoilRotation += upwardKick;
        StartCoroutine(RecoilRecovery(duration));
    }

    private System.Collections.IEnumerator RecoilRecovery(float duration)
    {
        yield return new WaitForSeconds(duration);
    }

    private void ApplyRecoilEffect()
    {
        if (Mathf.Abs(recoilRotation) > 0.01f)
        {
            recoilRotation = Mathf.Lerp(recoilRotation, 0f, recoilSpeed * Time.deltaTime);

            float currentXRotation = transform.localEulerAngles.x;
            if (currentXRotation > 180f) currentXRotation -= 360f;

            float newXRotation = currentXRotation - (recoilRotation * recoilAmount);
            transform.localRotation = Quaternion.Euler(newXRotation, 0f, 0f);
        }
    }

    public void TriggerDamageFOV(float duration = 0.3f)
    {
        isDamageFOV = true;
        targetFOV = damageFOV;
        StartCoroutine(ResetFOVAfterDelay(duration));
    }

    private System.Collections.IEnumerator ResetFOVAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isDamageFOV = false;
        targetFOV = normalFOV;
    }

    private void ApplyFOVEffect()
    {
        if (playerCamera != null)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
        }
    }

    public void UpdateOriginalTransform()
    {
        originalPosition = transform.localPosition;
    }
}
