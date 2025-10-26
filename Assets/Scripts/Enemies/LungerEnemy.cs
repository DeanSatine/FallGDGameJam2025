using UnityEngine;

public class LungerEnemy : Enemy
{
    [Header("Lunger Settings")]
    [SerializeField] private float rollSpeed = 3f;
    [SerializeField] private float jumpDistance = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float bounceForce = 8f;
    [SerializeField] private float rotationSpeed = 360f;

    private Rigidbody rb;
    private bool isGrounded = false;
    private float groundCheckDistance = 0.6f;
    private Vector3 lastPosition;
    private float stuckCheckTimer = 0f;
    private float stuckCheckInterval = 1f;
    private float minMovementThreshold = 0.1f;
    private Vector3 randomRotationAxis;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.constraints = RigidbodyConstraints.None;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        damageToPlayer = 1;
        pointValue = 1;
        lastPosition = transform.position;
        
        randomRotationAxis = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;
    }

    private void FixedUpdate()
    {
        if (isDead || playerTransform == null) return;

        CheckGrounded();
        CheckIfStuck();
        ApplyRotation();

        if (isGrounded)
        {
            BounceTowardsPlayer();
        }
    }

    private void ApplyRotation()
    {
        float velocityMagnitude = rb.linearVelocity.magnitude;
        float rotationAmount = rotationSpeed * Time.fixedDeltaTime * (velocityMagnitude / bounceForce);
        
        transform.Rotate(randomRotationAxis, rotationAmount, Space.World);
    }

    private void CheckGrounded()
    {
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance);
    }

    private void CheckIfStuck()
    {
        stuckCheckTimer += Time.fixedDeltaTime;

        if (stuckCheckTimer >= stuckCheckInterval)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);

            if (distanceMoved < minMovementThreshold)
            {
                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    0,
                    Random.Range(-1f, 1f)
                ).normalized;

                rb.linearVelocity = randomDirection * bounceForce + Vector3.up * jumpHeight;
            }

            lastPosition = transform.position;
            stuckCheckTimer = 0f;
        }
    }

    private void BounceTowardsPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0;

        Vector3 bounceVelocity = direction * bounceForce;
        bounceVelocity.y = jumpHeight;

        rb.linearVelocity = bounceVelocity;
        
        randomRotationAxis = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Default") || 
                 collision.gameObject.CompareTag("Wall"))
        {
            Vector3 wallNormal = collision.contacts[0].normal;
            wallNormal.y = 0;
            
            Vector3 bounceDirection = Vector3.Reflect(rb.linearVelocity.normalized, wallNormal);
            bounceDirection.y = 0;
            bounceDirection.Normalize();

            rb.linearVelocity = bounceDirection * bounceForce + Vector3.up * jumpHeight * 0.5f;
        }
    }
}
