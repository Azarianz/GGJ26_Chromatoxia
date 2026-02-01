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
    [SerializeField] private float timeToLive;

    private void Start()
    {
        GameUIManager.Instance.InitWave(timeToLive, timeToLive);
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnRate && timeToLive > 0)
        {
            SpawnEnemy();
            spawnTimer = 0f;
            timeToLive--;
        }
        GameUIManager.Instance.UpdateWave(timeToLive);

        if (timeToLive < 0f)
            GameManager.I.Win("");
    }

   void SpawnEnemy()
{
    Vector3 spawnPos = GetRandomSpawnPositionXZ();
    GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

    Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
}

Vector3 GetRandomSpawnPositionXZ()
{
    Vector2 randomDirection2D = Random.insideUnitCircle.normalized;
    float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

    Vector3 offset = new Vector3(
        randomDirection2D.x,
        0f,                     // 🔒 Y axis locked
        randomDirection2D.y
    );

    return player.position + offset * distance;
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
