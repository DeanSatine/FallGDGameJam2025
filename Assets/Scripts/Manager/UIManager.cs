using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI sandwichCountText;

    [Header("Interaction")]
    [SerializeField] private GameObject interactionPromptPanel;
    [SerializeField] private TextMeshProUGUI interactionPromptText;

    [Header("Day Panels")]
    [SerializeField] private GameObject dayStartPanel;
    [SerializeField] private Image dayStartFadeImage;
    [SerializeField] private GameObject dayEndPanel;
    [SerializeField] private TextMeshProUGUI dayEndText;
    [SerializeField] private TextMeshProUGUI rentCostText;
    [SerializeField] private Image dayEndFadeImage;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;

    [Header("Death Fade")]
    [SerializeField] private Image deathImage;
    [SerializeField] private float deathSequenceDuration = 5f;
    [SerializeField] private float deathFadeDuration = 1.5f;

    private GameManager gameManager;
    private PlayerHealth playerHealth;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(UpdateHealthDisplay);
            UpdateHealthDisplay(playerHealth.GetCurrentHealth());
        }

        if (dayStartPanel != null) dayStartPanel.SetActive(false);
        if (dayEndPanel != null) dayEndPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (interactionPromptPanel != null) interactionPromptPanel.SetActive(false);

        if (dayStartFadeImage != null)
        {
            Color c = dayStartFadeImage.color;
            dayStartFadeImage.color = new Color(c.r, c.g, c.b, 0);
        }

        if (dayEndFadeImage != null)
        {
            Color c = dayEndFadeImage.color;
            dayEndFadeImage.color = new Color(c.r, c.g, c.b, 0);
        }

        if (deathImage != null)
        {
            Color c = deathImage.color;
            deathImage.color = new Color(c.r, c.g, c.b, 0);
            deathImage.gameObject.SetActive(false);
        }
    }

    private void UpdateHealthDisplay(int health)
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {health}";
        }
    }

    public void UpdateGameStats(int points, int round, int enemiesKilled, int enemiesToKill)
    {
        if (pointsText != null)
        {
            pointsText.text = $"Points: {points}";
        }

        if (roundText != null)
        {
            roundText.text = $"Day: {round}";
        }

        if (enemiesText != null)
        {
            enemiesText.text = $"Enemies: {enemiesKilled}/{enemiesToKill}";
        }

        if (healthText != null && playerHealth != null)
        {
            healthText.text = $"Health: {playerHealth.GetCurrentHealth()}";
        }
    }

    public void ShowDayStart(int day)
    {
        StartCoroutine(ShowDayStartWithFade(day));

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDayStartJingle();
        }
    }

    private IEnumerator ShowDayStartWithFade(int day)
    {
        if (dayStartFadeImage != null)
        {
            yield return StartCoroutine(FadeImage(dayStartFadeImage, 0f, 1f, fadeDuration));
        }

        yield return new WaitForSeconds(1.5f);

        if (dayStartFadeImage != null)
        {
            yield return StartCoroutine(FadeImage(dayStartFadeImage, 1f, 0f, fadeDuration));
        }
    }

    public void ShowDayEnd(int day, int points, int rentCost)
    {
        if (dayEndPanel != null && dayEndText != null && rentCostText != null)
        {
            dayEndText.text = $"Day {day} Complete!";
            rentCostText.text = $"Rent Cost: {rentCost}\nYour Points: {points}";
            dayEndPanel.SetActive(true);
        }
    }

    public void OnPayRentButtonClicked()
    {
        if (dayEndPanel != null)
        {
            dayEndPanel.SetActive(false);
        }

        if (gameManager != null)
        {
            gameManager.PayRent();
        }
    }

    public void ShowGameOver(int finalRound, int finalPoints, string reason)
    {
        if (gameOverPanel != null && gameOverText != null)
        {
            gameOverText.text = $"Game Over!\n{reason}\n\nFinal Day: {finalRound}\nFinal Points: {finalPoints}";
            gameOverPanel.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOverJingle();
        }
    }

    public void ShowDeathFade(int finalRound, int finalPoints, string menuSceneName)
    {
        StartCoroutine(DeathFadeSequence(menuSceneName));
    }

    private IEnumerator DeathFadeSequence(string menuSceneName)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOverJingle();
        }

        if (deathImage != null)
        {
            deathImage.gameObject.SetActive(true);
            yield return StartCoroutine(FadeImageUnscaled(deathImage, 0f, 1f, deathFadeDuration));
        }

        float remainingTime = deathSequenceDuration - deathFadeDuration;
        yield return new WaitForSecondsRealtime(remainingTime);

        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    public void OnRestartButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }

    public void ShowInteractionPrompt(string prompt)
    {
        if (interactionPromptPanel != null && interactionPromptText != null)
        {
            interactionPromptText.text = prompt;
            interactionPromptPanel.SetActive(true);
        }
    }

    public void HideInteractionPrompt()
    {
        if (interactionPromptPanel != null)
        {
            interactionPromptPanel.SetActive(false);
        }
    }

    public void UpdateSandwichCount(int count)
    {
        if (sandwichCountText != null)
        {
            sandwichCountText.text = $"Sandwiches: {count}";
        }
    }

    private IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = image.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            image.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        image.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    private IEnumerator FadeImageUnscaled(Image image, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = image.color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            image.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        image.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}
