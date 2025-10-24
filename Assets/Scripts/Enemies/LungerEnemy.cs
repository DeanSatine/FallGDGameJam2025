using UnityEngine;

public class LungerEnemy : Enemy
{
    [Header("Lunger Settings")]
    [SerializeField] private float rollSpeed = 3f;
    [SerializeField] private float jumpDistance = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpHeight = 5f;

    private Rigidbody rb;
    private bool hasJumped = false;
    private bool isJumping = false;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        damageToPlayer = 1;
        pointValue = 1;
    }

    private void FixedUpdate()
    {
        if (isDead || playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (!hasJumped && distanceToPlayer <= jumpDistance && !isJumping)
        {
            JumpTowardsPlayer();
        }
        else if (!hasJumped && !isJumping)
        {
            RollTowardsPlayer();
        }
    }

    private void RollTowardsPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0;

        Vector3 rollVelocity = direction * rollSpeed;
        rollVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = rollVelocity;

        transform.Rotate(Vector3.right * rollSpeed * 100f * Time.fixedDeltaTime);
    }

    private void JumpTowardsPlayer()
    {
        hasJumped = true;
        isJumping = true;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0;

        Vector3 jumpVelocity = direction * jumpForce;
        jumpVelocity.y = jumpHeight;

        rb.linearVelocity = jumpVelocity;

        Invoke(nameof(ResetJump), 2f);
    }

    private void ResetJump()
    {
        isJumping = false;
    }
}
