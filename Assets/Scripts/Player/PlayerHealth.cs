using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int currentHealth = 5;

    [Header("Damage Effects")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private float damageShakeDuration = 0.3f;
    [SerializeField] private float damageShakeMagnitude = 0.15f;
    
    [Header("Damage Flash")]
    [SerializeField] private Image damageFlashImage;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.3f);

    [Header("Events")]
    public UnityEvent<int> OnHealthChanged;
    public UnityEvent OnPlayerDeath;

    private Coroutine flashCoroutine;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);

        if (cameraController == null)
        {
            cameraController = FindFirstObjectByType<CameraController>();
        }

        if (damageFlashImage != null)
        {
            Color transparent = flashColor;
            transparent.a = 0f;
            damageFlashImage.color = transparent;
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"PlayerHealth: Taking {damage} damage. Current health: {currentHealth}");
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"PlayerHealth: New health: {currentHealth}");
        
        OnHealthChanged?.Invoke(currentHealth);

        if (cameraController != null)
        {
            cameraController.TriggerShake(damageShakeDuration, damageShakeMagnitude);
            cameraController.TriggerDamageFOV(0.2f);
        }

        if (damageFlashImage != null)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashDamage());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashDamage()
    {
        damageFlashImage.color = flashColor;
        
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(flashColor.a, 0f, elapsedTime / flashDuration);
            Color currentColor = flashColor;
            currentColor.a = alpha;
            damageFlashImage.color = currentColor;
            yield return null;
        }

        Color transparent = flashColor;
        transparent.a = 0f;
        damageFlashImage.color = transparent;
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
