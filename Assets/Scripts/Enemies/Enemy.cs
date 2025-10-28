using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] protected int healthPoints = 1;
    [SerializeField] protected int damageToPlayer = 1;
    [SerializeField] protected int pointValue = 1;

    protected Transform playerTransform;
    protected bool isDead = false;

    protected virtual void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        healthPoints -= damage;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyHitSound();
        }

        if (healthPoints <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyKillSound();
        }

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnEnemyKilled(pointValue);
        }

        Destroy(gameObject);
    }


    protected virtual void OnCollisionEnter(Collision collision)
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
