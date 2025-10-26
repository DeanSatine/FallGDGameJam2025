using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int currentHealth = 5;

    [Header("Damage Effects")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private float damageShakeDuration = 0.3f;
    [SerializeField] private float damageShakeMagnitude = 0.15f;

    [Header("Events")]
    public UnityEvent<int> OnHealthChanged;
    public UnityEvent OnPlayerDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);

        if (cameraController == null)
        {
            cameraController = FindFirstObjectByType<CameraController>();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        OnHealthChanged?.Invoke(currentHealth);

        if (cameraController != null)
        {
            cameraController.TriggerShake(damageShakeDuration, damageShakeMagnitude);
            cameraController.TriggerDamageFOV(0.2f);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke();

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnPlayerDeath();
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}
