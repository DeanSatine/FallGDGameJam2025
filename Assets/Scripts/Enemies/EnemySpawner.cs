using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject lungerPrefab;
    [SerializeField] private GameObject throwerPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float lungerSpawnInterval = 2f;
    [SerializeField] private float throwerSpawnInterval = 4f;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private Transform playerTransform;

    private bool isSpawning = false;
    private int currentRound = 1;
    private int enemiesToSpawn = 10;

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
    }

    public void StartSpawning(int round, int enemyCount)
    {
        currentRound = round;
        enemiesToSpawn = enemyCount;
        isSpawning = true;

        StartCoroutine(SpawnLungers());
        StartCoroutine(SpawnThrowers());
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    private IEnumerator SpawnLungers()
    {
        while (isSpawning && enemiesToSpawn > 0)
        {
            yield return new WaitForSeconds(lungerSpawnInterval);

            if (lungerPrefab != null && enemiesToSpawn > 0)
            {
                SpawnEnemy(lungerPrefab);
                enemiesToSpawn--;
            }
        }
    }

    private IEnumerator SpawnThrowers()
    {
        while (isSpawning && enemiesToSpawn > 0)
        {
            yield return new WaitForSeconds(throwerSpawnInterval);

            if (throwerPrefab != null && enemiesToSpawn > 0)
            {
                SpawnEnemy(throwerPrefab);
                enemiesToSpawn--;
            }
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (playerTransform == null) return Vector3.zero;

        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnOffset = new Vector3(randomCircle.x, 0, randomCircle.y);

        return playerTransform.position + spawnOffset;
    }
}
