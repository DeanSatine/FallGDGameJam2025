using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject lungerPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private Transform spawnBounds;
    [SerializeField] private float groundOffset = 0.5f;
    [SerializeField] private bool debugMode = false;

    private bool isSpawning = false;
    private Bounds spawnArea;
    private Coroutine spawnCoroutine;
    private int totalSpawned = 0;

    private void Start()
    {
        if (spawnBounds != null)
        {
            Collider boundsCollider = spawnBounds.GetComponent<Collider>();
            if (boundsCollider != null)
            {
                spawnArea = boundsCollider.bounds;
            }
            else
            {
                spawnArea = new Bounds(spawnBounds.position, spawnBounds.localScale);
            }
        }
    }

    public void StartSpawning(int round, int enemyCount)
    {
        if (isSpawning)
        {
            if (debugMode) Debug.Log("EnemySpawner: Already spawning, stopping old coroutine first.");
            StopSpawning();
        }

        isSpawning = true;
        totalSpawned = 0;

        if (debugMode) Debug.Log($"EnemySpawner: Starting spawning for round {round}");

        spawnCoroutine = StartCoroutine(SpawnEnemies());
    }

    public void StopSpawning()
    {
        if (debugMode) Debug.Log($"EnemySpawner: Stopping spawning. Total spawned: {totalSpawned}");

        isSpawning = false;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnEnemies()
    {
        if (debugMode) Debug.Log("EnemySpawner: SpawnEnemies coroutine started");

        while (isSpawning)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (!isSpawning)
            {
                if (debugMode) Debug.Log("EnemySpawner: isSpawning became false, exiting coroutine");
                yield break;
            }

            if (lungerPrefab != null)
            {
                SpawnEnemy(lungerPrefab);
            }
            else
            {
                if (debugMode) Debug.LogWarning("EnemySpawner: lungerPrefab is null!");
            }
        }

        if (debugMode) Debug.Log("EnemySpawner: SpawnEnemies coroutine ended naturally");
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        totalSpawned++;

        if (debugMode) Debug.Log($"EnemySpawner: Spawned enemy #{totalSpawned} at {spawnPosition}");
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnBounds == null)
        {
            if (debugMode) Debug.LogWarning("EnemySpawner: spawnBounds is null, using spawner position");
            return transform.position;
        }

        Vector3 randomPoint = new Vector3(
            Random.Range(spawnArea.min.x, spawnArea.max.x),
            spawnArea.center.y + groundOffset,
            Random.Range(spawnArea.min.z, spawnArea.max.z)
        );

        return randomPoint;
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnBounds != null)
        {
            Gizmos.color = Color.red;
            Collider boundsCollider = spawnBounds.GetComponent<Collider>();
            if (boundsCollider != null)
            {
                Gizmos.DrawWireCube(boundsCollider.bounds.center, boundsCollider.bounds.size);
            }
            else
            {
                Gizmos.DrawWireCube(spawnBounds.position, spawnBounds.localScale);
            }
        }
    }
}
