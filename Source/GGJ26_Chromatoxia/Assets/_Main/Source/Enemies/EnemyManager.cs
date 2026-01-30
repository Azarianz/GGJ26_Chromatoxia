using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Spawning")]
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private float minSpawnDistance = 5f;
    [SerializeField] private float maxSpawnDistance = 10f;

    private float spawnTimer;

    void Update()
    {
        if (player == null || enemyPrefabs.Count == 0) return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnRate)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnPos = GetRandomSpawnPosition();
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    Vector2 GetRandomSpawnPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

        return (Vector2)player.position + randomDirection * distance;
    }

    // =========================
    // Debug
    // =========================
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, minSpawnDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, maxSpawnDistance);
    }
}
