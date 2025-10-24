using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Day Panels")]
    [SerializeField] private GameObject dayStartPanel;
    [SerializeField] private TextMeshProUGUI dayStartText;
    [SerializeField] private GameObject dayEndPanel;
    [SerializeField] private TextMeshProUGUI dayEndText;
    [SerializeField] private TextMeshProUGUI rentCostText;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;

    private GameManager gameManager;
    private PlayerHealth playerHealth;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (dayStartPanel != null) dayStartPanel.SetActive(false);
        if (dayEndPanel != null) dayEndPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
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
        if (dayStartPanel != null && dayStartText != null)
        {
            dayStartText.text = $"Day {day}";
            dayStartPanel.SetActive(true);
            Invoke(nameof(HideDayStart), 2f);
        }
    }

    private void HideDayStart()
    {
        if (dayStartPanel != null)
        {
            dayStartPanel.SetActive(false);
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
    }

    public void OnRestartButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }
}
