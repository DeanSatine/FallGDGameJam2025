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

    [Header("Health Text Effects")]
    [SerializeField] private Color healthNormalColor = Color.green;
    [SerializeField] private Color healthDamageColor = Color.red;
    [SerializeField] private float healthFlashDuration = 0.3f;
    [SerializeField] private float healthShakeDuration = 0.3f;
    [SerializeField] private float healthShakeMagnitude = 5f;

    [Header("Sandwich Text Effects")]
    [SerializeField] private float sandwichBounceDuration = 0.4f;
    [SerializeField] private float sandwichBounceScale = 1.3f;

    [Header("Points Text Effects")]
    [SerializeField] private float pointsBounceDuration = 0.4f;
    [SerializeField] private float pointsBounceScale = 1.3f;

    [Header("Round Text Effects")]
    [SerializeField] private float roundBounceDuration = 0.5f;
    [SerializeField] private float roundBounceScale = 1.4f;

    [Header("Enemies Text Effects")]
    [SerializeField] private Color enemiesNormalColor = Color.white;
    [SerializeField] private Color enemiesCompleteColor = Color.yellow;
    [SerializeField] private float enemiesDanceDuration = 1.5f;
    [SerializeField] private float enemiesDanceScale = 1.2f;
    [SerializeField] private float enemiesDanceSpeed = 5f;

    private GameManager gameManager;
    private PlayerHealth playerHealth;
    private Vector3 healthTextOriginalPosition;
    private Vector3 sandwichTextOriginalScale;
    private Vector3 pointsTextOriginalScale;
    private Vector3 roundTextOriginalScale;
    private Vector3 enemiesTextOriginalScale;
    private Coroutine healthFlashCoroutine;
    private Coroutine healthShakeCoroutine;
    private Coroutine sandwichBounceCoroutine;
    private Coroutine pointsBounceCoroutine;
    private Coroutine roundBounceCoroutine;
    private Coroutine enemiesDanceCoroutine;

    private int lastPoints = 0;
    private int lastRound = 0;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (healthText != null)
        {
            healthText.color = healthNormalColor;
            healthTextOriginalPosition = healthText.rectTransform.localPosition;
        }

        if (sandwichCountText != null)
        {
            sandwichTextOriginalScale = sandwichCountText.rectTransform.localScale;
        }

        if (pointsText != null)
        {
            pointsTextOriginalScale = pointsText.rectTransform.localScale;
        }

        if (roundText != null)
        {
            roundTextOriginalScale = roundText.rectTransform.localScale;
        }

        if (enemiesText != null)
        {
            enemiesTextOriginalScale = enemiesText.rectTransform.localScale;
            enemiesText.color = enemiesNormalColor;
        }

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

            if (healthFlashCoroutine != null)
            {
                StopCoroutine(healthFlashCoroutine);
            }
            healthFlashCoroutine = StartCoroutine(FlashHealthText());

            if (healthShakeCoroutine != null)
            {
                StopCoroutine(healthShakeCoroutine);
            }
            healthShakeCoroutine = StartCoroutine(ShakeHealthText());
        }
    }

    private IEnumerator FlashHealthText()
    {
        if (healthText == null) yield break;

        healthText.color = healthDamageColor;

        float elapsed = 0f;
        while (elapsed < healthFlashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / healthFlashDuration;
            healthText.color = Color.Lerp(healthDamageColor, healthNormalColor, t);
            yield return null;
        }

        healthText.color = healthNormalColor;
    }

    private IEnumerator ShakeHealthText()
    {
        if (healthText == null) yield break;

        float elapsed = 0f;
        while (elapsed < healthShakeDuration)
        {
            elapsed += Time.deltaTime;

            float offsetX = Random.Range(-healthShakeMagnitude, healthShakeMagnitude);
            float offsetY = Random.Range(-healthShakeMagnitude, healthShakeMagnitude);

            healthText.rectTransform.localPosition = healthTextOriginalPosition + new Vector3(offsetX, offsetY, 0f);

            yield return null;
        }

        healthText.rectTransform.localPosition = healthTextOriginalPosition;
    }

    public void UpdateGameStats(int points, int round, int enemiesKilled, int enemiesToKill)
    {
        if (pointsText != null)
        {
            pointsText.text = $"Money: ${points}";

            if (points != lastPoints)
            {
                lastPoints = points;
                if (pointsBounceCoroutine != null)
                {
                    StopCoroutine(pointsBounceCoroutine);
                }
                pointsBounceCoroutine = StartCoroutine(BouncePointsText());
            }
        }

        if (roundText != null)
        {
            roundText.text = $"Day: {round}";

            if (round != lastRound)
            {
                lastRound = round;
                if (roundBounceCoroutine != null)
                {
                    StopCoroutine(roundBounceCoroutine);
                }
                roundBounceCoroutine = StartCoroutine(BounceRoundText());
            }
        }

        if (enemiesText != null)
        {
            enemiesText.text = $"Enemies: {enemiesKilled}/{enemiesToKill}";

            if (enemiesKilled >= enemiesToKill)
            {
                if (enemiesDanceCoroutine != null)
                {
                    StopCoroutine(enemiesDanceCoroutine);
                }
                enemiesDanceCoroutine = StartCoroutine(DanceEnemiesText());
            }
            else
            {
                if (enemiesDanceCoroutine != null)
                {
                    StopCoroutine(enemiesDanceCoroutine);
                    enemiesDanceCoroutine = null;
                }
                enemiesText.color = enemiesNormalColor;
                enemiesText.rectTransform.localScale = enemiesTextOriginalScale;
            }
        }

        if (healthText != null && playerHealth != null)
        {
            healthText.text = $"Health: {playerHealth.GetCurrentHealth()}";
        }
    }

    private IEnumerator BouncePointsText()
    {
        if (pointsText == null) yield break;

        float elapsed = 0f;
        float halfDuration = pointsBounceDuration * 0.5f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            float scale = Mathf.Lerp(1f, pointsBounceScale, t);
            pointsText.rectTransform.localScale = pointsTextOriginalScale * scale;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            float scale = Mathf.Lerp(pointsBounceScale, 1f, t);
            pointsText.rectTransform.localScale = pointsTextOriginalScale * scale;
            yield return null;
        }

        pointsText.rectTransform.localScale = pointsTextOriginalScale;
        pointsBounceCoroutine = null;
    }

    private IEnumerator BounceRoundText()
    {
        if (roundText == null) yield break;

        float elapsed = 0f;
        float halfDuration = roundBounceDuration * 0.5f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            float scale = Mathf.Lerp(1f, roundBounceScale, t);
            roundText.rectTransform.localScale = roundTextOriginalScale * scale;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            float scale = Mathf.Lerp(roundBounceScale, 1f, t);
            roundText.rectTransform.localScale = roundTextOriginalScale * scale;
            yield return null;
        }

        roundText.rectTransform.localScale = roundTextOriginalScale;
        roundBounceCoroutine = null;
    }

    private IEnumerator DanceEnemiesText()
    {
        if (enemiesText == null) yield break;

        float elapsed = 0f;
        while (elapsed < enemiesDanceDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / enemiesDanceDuration;

            float pulseScale = 1f + Mathf.Sin(Time.time * enemiesDanceSpeed) * (enemiesDanceScale - 1f);
            enemiesText.rectTransform.localScale = enemiesTextOriginalScale * pulseScale;

            float colorT = (Mathf.Sin(Time.time * enemiesDanceSpeed * 2f) + 1f) * 0.5f;
            enemiesText.color = Color.Lerp(enemiesNormalColor, enemiesCompleteColor, colorT);

            yield return null;
        }

        enemiesText.rectTransform.localScale = enemiesTextOriginalScale;
        enemiesText.color = enemiesCompleteColor;
        enemiesDanceCoroutine = null;
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

            if (sandwichBounceCoroutine != null)
            {
                StopCoroutine(sandwichBounceCoroutine);
            }
            sandwichBounceCoroutine = StartCoroutine(BounceSandwichText());
        }
    }

    private IEnumerator BounceSandwichText()
    {
        if (sandwichCountText == null) yield break;

        float elapsed = 0f;
        float halfDuration = sandwichBounceDuration * 0.5f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            float scale = Mathf.Lerp(1f, sandwichBounceScale, t);
            sandwichCountText.rectTransform.localScale = sandwichTextOriginalScale * scale;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            float scale = Mathf.Lerp(sandwichBounceScale, 1f, t);
            sandwichCountText.rectTransform.localScale = sandwichTextOriginalScale * scale;
            yield return null;
        }

        sandwichCountText.rectTransform.localScale = sandwichTextOriginalScale;
        sandwichBounceCoroutine = null;
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
