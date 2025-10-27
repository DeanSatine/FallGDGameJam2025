using UnityEngine;

public class SandwichProjectile : MonoBehaviour
{
    public int damage = 1;
    public GameObject explosionVFXPrefab;
    public AudioClip explosionSound;

    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float explosionVFXLifetime = 3f;
    [SerializeField] private float explosionVolume = 1f;

    private bool hasBeenThrown = false;

    public void OnThrown()
    {
        hasBeenThrown = true;
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasBeenThrown) return;

        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            SpawnExplosionVFX(collision.contacts[0].point, collision.contacts[0].normal);
            PlayExplosionSound(collision.contacts[0].point);
            Destroy(gameObject);
        }
        else if (!collision.gameObject.CompareTag("Player"))
        {
            SpawnExplosionVFX(collision.contacts[0].point, collision.contacts[0].normal);
            PlayExplosionSound(collision.contacts[0].point);
            Destroy(gameObject);
        }
    }

    private void SpawnExplosionVFX(Vector3 position, Vector3 normal)
    {
        if (explosionVFXPrefab != null)
        {
            Quaternion rotation = Quaternion.LookRotation(normal);
            GameObject vfx = Instantiate(explosionVFXPrefab, position, rotation);
            Destroy(vfx, explosionVFXLifetime);
        }
    }

    private void PlayExplosionSound(Vector3 position)
    {
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, position, explosionVolume);
        }
    }
}
