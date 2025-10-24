using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] private int currentRound = 1;
    [SerializeField] private int currentPoints = 0;
    [SerializeField] private int enemiesKilledThisRound = 0;

    [Header("Round Settings")]
    [SerializeField] private int baseRentCost = 10;
    [SerializeField] private int rentIncreasePerRound = 2;
    [SerializeField] private int baseEnemiesPerRound = 10;
    [SerializeField] private int enemyIncreasePerRound = 2;
    [SerializeField] private int healthRestorePerRound = 3;

    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private UIManager uiManager;

    private bool isRoundActive = false;
    private bool isGameOver = false;
    private int currentRentCost;
    private int enemiesToKillThisRound;

    private void Start()
    {
        if (enemySpawner == null)
        {
            enemySpawner = FindFirstObjectByType<EnemySpawner>();
        }

        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<UIManager>();
        }

        StartCoroutine(StartGameSequence());
    }

    private IEnumerator StartGameSequence()
    {
        yield return new WaitForSeconds(1f);
        StartRound();
    }

    private void StartRound()
    {
        if (isGameOver) return;

        currentRentCost = baseRentCost + (currentRound - 1) * rentIncreasePerRound;
        enemiesToKillThisRound = baseEnemiesPerRound + (currentRound - 1) * enemyIncreasePerRound;
        enemiesKilledThisRound = 0;
        isRoundActive = true;

        if (uiManager != null)
        {
            uiManager.ShowDayStart(currentRound);
        }

        if (enemySpawner != null)
        {
            enemySpawner.StartSpawning(currentRound, enemiesToKillThisRound);
        }

        UpdateUI();
    }

    public void OnEnemyKilled(int pointValue)
    {
        if (!isRoundActive) return;

        currentPoints += pointValue;
        enemiesKilledThisRound++;

        UpdateUI();

        if (enemiesKilledThisRound >= enemiesToKillThisRound)
        {
            EndRound();
        }
    }

    private void EndRound()
    {
        isRoundActive = false;

        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
        }

        ClearRemainingEnemies();

        if (playerHealth != null)
        {
            playerHealth.Heal(healthRestorePerRound);
        }

        if (uiManager != null)
        {
            uiManager.ShowDayEnd(currentRound, currentPoints, currentRentCost);
        }
    }

    public void PayRent()
    {
        if (currentPoints >= currentRentCost)
        {
            currentPoints -= currentRentCost;
            currentRound++;
            UpdateUI();

            StartCoroutine(StartNextRoundDelay());
        }
        else
        {
            GameOver(false);
        }
    }

    private IEnumerator StartNextRoundDelay()
    {
        yield return new WaitForSeconds(2f);
        StartRound();
    }

    public void OnPlayerDeath()
    {
        GameOver(true);
    }

    private void GameOver(bool fromDeath)
    {
        isGameOver = true;
        isRoundActive = false;

        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
        }

        ClearRemainingEnemies();

        if (uiManager != null)
        {
            string reason = fromDeath ? "Health reached 0!" : "Could not pay rent!";
            uiManager.ShowGameOver(currentRound, currentPoints, reason);
        }
    }

    private void ClearRemainingEnemies()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        EnemyProjectile[] projectiles = FindObjectsByType<EnemyProjectile>(FindObjectsSortMode.None);
        foreach (EnemyProjectile projectile in projectiles)
        {
            Destroy(projectile.gameObject);
        }
    }

    private void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateGameStats(currentPoints, currentRound, enemiesKilledThisRound, enemiesToKillThisRound);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public int GetCurrentPoints()
    {
        return currentPoints;
    }

    public int GetCurrentRound()
    {
        return currentRound;
    }

    public int GetCurrentRentCost()
    {
        return currentRentCost;
    }
}
