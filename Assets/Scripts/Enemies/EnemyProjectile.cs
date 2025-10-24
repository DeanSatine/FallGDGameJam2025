using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 2;

    [SerializeField] private float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (!collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
