using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("Head Bob Settings")]
    [SerializeField] private bool enableHeadBob = true;
    [SerializeField] private float bobFrequency = 2f;
    [SerializeField] private float bobHorizontalAmplitude = 0.05f;
    [SerializeField] private float bobVerticalAmplitude = 0.08f;
    [SerializeField] private float bobSmoothing = 10f;

    [Header("Sandwich Settings")]
    [SerializeField] private GameObject sandwichPrefab;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private int sandwichDamage = 1;
    [SerializeField] private float sandwichCreationTime = 0.5f;

    [Header("VFX Settings")]
    [SerializeField] private GameObject throwVFXPrefab;
    [SerializeField] private GameObject explosionVFXPrefab;
    [SerializeField] private float throwShakeDuration = 0.1f;
    [SerializeField] private float throwShakeMagnitude = 0.05f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioClip sandwichCreateSound;
    [SerializeField] private AudioClip throwSound;
    [SerializeField] private float footstepVolume = 0.5f;
    [SerializeField] private float sandwichCreateVolume = 0.7f;
    [SerializeField] private float throwVolume = 0.8f;
    [SerializeField] private float footstepInterval = 0.5f;

    [Header("Camera References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CameraController cameraController;

    private Rigidbody rb;
    private AudioSource audioSource;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private GameObject heldSandwich;
    private float verticalRotation = 0f;
    private bool isCreatingSandwich = false;

    private float bobTimer = 0f;
    private Vector3 cameraStartPosition;
    private float footstepTimer = 0f;
    private int lastFootstepIndex = -1;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0f;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (cameraController == null && cameraTransform != null)
        {
            cameraController = cameraTransform.GetComponent<CameraController>();
        }

        if (cameraTransform != null)
        {
            cameraStartPosition = cameraTransform.localPosition;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        inputActions.Gameplay.Enable();

        inputActions.Gameplay.Move.performed += OnMovePerformed;
        inputActions.Gameplay.Move.canceled += OnMoveCanceled;
        inputActions.Gameplay.CreateSandwich.performed += OnCreateSandwich;
        inputActions.Gameplay.Throw.performed += OnThrowSandwich;
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Move.performed -= OnMovePerformed;
        inputActions.Gameplay.Move.canceled -= OnMoveCanceled;
        inputActions.Gameplay.CreateSandwich.performed -= OnCreateSandwich;
        inputActions.Gameplay.Throw.performed -= OnThrowSandwich;

        inputActions.Gameplay.Disable();
    }

    private void Update()
    {
        HandleMouseLook();
        HandleHeadBob();
        HandleFootstepSounds();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void HandleMouseLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        moveDirection.y = 0f;
        moveDirection.Normalize();

        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }

    private void HandleHeadBob()
    {
        if (!enableHeadBob || cameraTransform == null)
            return;

        bool isMoving = moveInput.magnitude > 0.1f;

        if (isMoving)
        {
            bobTimer += Time.deltaTime * bobFrequency;

            float horizontalOffset = Mathf.Sin(bobTimer) * bobHorizontalAmplitude;
            float verticalOffset = Mathf.Abs(Mathf.Cos(bobTimer * 2f)) * bobVerticalAmplitude;

            Vector3 targetPosition = cameraStartPosition + new Vector3(horizontalOffset, verticalOffset, 0f);
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, bobSmoothing * Time.deltaTime);
        }
        else
        {
            bobTimer = 0f;
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, cameraStartPosition, bobSmoothing * Time.deltaTime);
        }
    }

    private void HandleFootstepSounds()
    {
        if (footstepSounds == null || footstepSounds.Length == 0)
            return;

        bool isMoving = moveInput.magnitude > 0.1f;

        if (isMoving)
        {
            footstepTimer += Time.deltaTime;

            if (footstepTimer >= footstepInterval)
            {
                PlayRandomFootstep();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    private void PlayRandomFootstep()
    {
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, footstepSounds.Length);
        }
        while (randomIndex == lastFootstepIndex && footstepSounds.Length > 1);

        lastFootstepIndex = randomIndex;

        if (footstepSounds[randomIndex] != null)
        {
            audioSource.PlayOneShot(footstepSounds[randomIndex], footstepVolume);
        }
    }

    private void OnCreateSandwich(InputAction.CallbackContext context)
    {
        if (heldSandwich == null && sandwichPrefab != null && !isCreatingSandwich)
        {
            StartCoroutine(CreateSandwichCoroutine());
        }
    }

    private System.Collections.IEnumerator CreateSandwichCoroutine()
    {
        isCreatingSandwich = true;

        if (sandwichCreateSound != null)
        {
            audioSource.PlayOneShot(sandwichCreateSound, sandwichCreateVolume);
        }

        yield return new WaitForSeconds(sandwichCreationTime);

        CreateSandwich();
        isCreatingSandwich = false;
    }

    private void CreateSandwich()
    {
        Vector3 spawnPosition = holdPoint != null ? holdPoint.position : cameraTransform.position + cameraTransform.forward;
        heldSandwich = Instantiate(sandwichPrefab, spawnPosition, Quaternion.identity);

        if (holdPoint != null)
        {
            heldSandwich.transform.SetParent(holdPoint);
            heldSandwich.transform.localPosition = Vector3.zero;
            heldSandwich.transform.localRotation = Quaternion.identity;
        }

        Rigidbody sandwichRb = heldSandwich.GetComponent<Rigidbody>();
        if (sandwichRb != null)
        {
            sandwichRb.isKinematic = true;
        }

        Collider sandwichCollider = heldSandwich.GetComponent<Collider>();
        if (sandwichCollider != null)
        {
            sandwichCollider.enabled = false;
        }
    }

    private void OnThrowSandwich(InputAction.CallbackContext context)
    {
        if (heldSandwich != null)
        {
            ThrowSandwich();
        }
    }

    private void ThrowSandwich()
    {
        Vector3 throwPosition = heldSandwich.transform.position;

        heldSandwich.transform.SetParent(null);

        Collider sandwichCollider = heldSandwich.GetComponent<Collider>();
        if (sandwichCollider != null)
        {
            sandwichCollider.enabled = true;
        }

        Rigidbody sandwichRb = heldSandwich.GetComponent<Rigidbody>();
        if (sandwichRb != null)
        {
            sandwichRb.isKinematic = false;
            sandwichRb.linearVelocity = cameraTransform.forward * throwForce;
        }

        SandwichProjectile projectile = heldSandwich.GetComponent<SandwichProjectile>();
        if (projectile == null)
        {
            projectile = heldSandwich.AddComponent<SandwichProjectile>();
        }
        projectile.damage = sandwichDamage;
        projectile.explosionVFXPrefab = explosionVFXPrefab;

        if (throwVFXPrefab != null)
        {
            GameObject throwVFX = Instantiate(throwVFXPrefab, throwPosition, Quaternion.LookRotation(cameraTransform.forward));
            Destroy(throwVFX, 2f);
        }

        if (throwSound != null)
        {
            audioSource.PlayOneShot(throwSound, throwVolume);
        }

        if (cameraController != null)
        {
            cameraController.ApplyRecoil(0.3f, 0.15f);
            cameraController.TriggerShake(throwShakeDuration, throwShakeMagnitude);
        }

        heldSandwich = null;
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
