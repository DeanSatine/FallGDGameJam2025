using UnityEngine;
using System.Collections;

public class ThrowerEnemy : Enemy
{
    [Header("Thrower Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwInterval = 3f;
    [SerializeField] private float initialDelay = 1f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float throwUpwardForce = 3f;

    private bool hasStartedThrowing = false;
    private Coroutine throwCoroutine;

    protected override void Start()
    {
        base.Start();

        damageToPlayer = 2;
        pointValue = 2;

        if (throwPoint == null)
        {
            GameObject point = new GameObject("ThrowPoint");
            point.transform.SetParent(transform);
            point.transform.localPosition = new Vector3(0, 1f, 0.5f);
            throwPoint = point.transform;
        }

        StartCoroutine(StartThrowingAfterDelay());
    }

    private IEnumerator StartThrowingAfterDelay()
    {
        yield return new WaitForSeconds(initialDelay);
        hasStartedThrowing = true;
        throwCoroutine = StartCoroutine(ThrowProjectiles());
    }

    private IEnumerator ThrowProjectiles()
    {
        while (!isDead && hasStartedThrowing)
        {
            yield return new WaitForSeconds(throwInterval);

            if (playerTransform != null && projectilePrefab != null)
            {
                ThrowAtPlayer();
            }
        }
    }

    private void ThrowAtPlayer()
    {
        Vector3 direction = (playerTransform.position - throwPoint.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        if (projectileRb != null)
        {
            Vector3 throwVelocity = direction * throwForce + Vector3.up * throwUpwardForce;
            projectileRb.linearVelocity = throwVelocity;
        }

        transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
    }

    protected override void Die()
    {
        if (throwCoroutine != null)
        {
            StopCoroutine(throwCoroutine);
        }
        base.Die();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
            }
        }
    }
}
