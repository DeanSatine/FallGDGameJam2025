using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Sandwich Settings")]
    [SerializeField] private GameObject sandwichPrefab;
    [SerializeField] private Transform sandwichSpawnPoint;
    [SerializeField] private int maxSandwiches = 3;
    [SerializeField] private float sandwichCreateTime = 0.5f;
    [SerializeField] private int currentSandwiches = 0;

    [Header("Throwing Settings")]
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float throwUpwardForce = 2f;

    [Header("Camera Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float aimFOV = 40f;
    [SerializeField] private float fovTransitionSpeed = 10f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookDirection;
    private bool isAiming = false;
    private bool isCreatingSandwich = false;
    private float targetFOV;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (sandwichSpawnPoint == null)
        {
            GameObject spawnPoint = new GameObject("SandwichSpawnPoint");
            spawnPoint.transform.SetParent(transform);
            spawnPoint.transform.localPosition = new Vector3(0, 1f, 0.5f);
            sandwichSpawnPoint = spawnPoint.transform;
        }

        targetFOV = normalFOV;
        currentSandwiches = maxSandwiches;
    }

    private void Update()
    {
        HandleRotation();
        HandleCameraZoom();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 velocity = movement * moveSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }

    private void HandleRotation()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 pointToLook = ray.GetPoint(rayDistance);
            Vector3 direction = (pointToLook - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void HandleCameraZoom()
    {
        if (playerCamera != null)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovTransitionSpeed * Time.deltaTime);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnCreateSandwich(InputAction.CallbackContext context)
    {
        if (context.performed && !isCreatingSandwich && currentSandwiches < maxSandwiches)
        {
            StartCoroutine(CreateSandwich());
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed && currentSandwiches > 0)
        {
            ThrowSandwich();
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAiming = true;
            targetFOV = aimFOV;
        }
        else if (context.canceled)
        {
            isAiming = false;
            targetFOV = normalFOV;
        }
    }

    private System.Collections.IEnumerator CreateSandwich()
    {
        isCreatingSandwich = true;
        yield return new WaitForSeconds(sandwichCreateTime);
        currentSandwiches++;
        currentSandwiches = Mathf.Min(currentSandwiches, maxSandwiches);
        isCreatingSandwich = false;
    }

    private void ThrowSandwich()
    {
        if (sandwichPrefab == null || sandwichSpawnPoint == null) return;

        GameObject sandwich = Instantiate(sandwichPrefab, sandwichSpawnPoint.position, sandwichSpawnPoint.rotation);
        Rigidbody sandwichRb = sandwich.GetComponent<Rigidbody>();

        if (sandwichRb != null)
        {
            Vector3 throwDirection = transform.forward;
            Vector3 throwVelocity = throwDirection * throwForce + Vector3.up * throwUpwardForce;
            sandwichRb.linearVelocity = throwVelocity;
        }

        currentSandwiches--;
    }

    public int GetCurrentSandwiches()
    {
        return currentSandwiches;
    }

    public int GetMaxSandwiches()
    {
        return maxSandwiches;
    }
}
