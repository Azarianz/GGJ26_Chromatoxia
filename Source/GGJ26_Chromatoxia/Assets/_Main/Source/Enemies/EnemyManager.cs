using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Enemy Prefabs")]
    [SerializeField] private List<GameObject> enemyPrefabs = new();

    [Header("Spawn Timing")]
    [SerializeField] private float spawnRate = 1.25f;
    private float spawnTimer;

    [Header("Spawn Ring (circle around player)")]
    [SerializeField] private float minSpawnRadius = 8f;
    [SerializeField] private float maxSpawnRadius = 12f;

    [Header("Burst")]
    [SerializeField] private int minPerBurst = 2;
    [SerializeField] private int maxPerBurst = 5;

    [Header("Wave Timer (seconds)")]
    [SerializeField] private float timeToLive = 30f;
    private float timeLeft;

    [Header("Population Cap")]
    [SerializeField] private int maxPopulation = 40;
    private int currentPopulation = 0;


    private void Start()
    {
        timeLeft = timeToLive;

        if (GameModifiers.Instance != null)
            spawnRate = Mathf.Max(0.15f, spawnRate - GameModifiers.Instance.spawnMult);

        if (GameUIManager.Instance != null)
            GameUIManager.Instance.InitWave(timeToLive, timeToLive);
    }

    private void Update()
    {
        if (!player || enemyPrefabs == null || enemyPrefabs.Count == 0) return;

        // countdown per second
        timeLeft -= Time.deltaTime;
        timeLeft = Mathf.Max(0f, timeLeft);

        if (GameUIManager.Instance != null)
            GameUIManager.Instance.UpdateWave(timeLeft);

        if (timeLeft <= 0f)
        {
            GameManager.I.Win("");
            return;
        }

        // spawn burst timer
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnRate)
        {
            spawnTimer = 0f;
            SpawnBurst();
        }
    }

    private void SpawnBurst()
    {
        // Stop entirely if cap reached
        if (currentPopulation >= maxPopulation)
            return;

        int count = Random.Range(minPerBurst, maxPerBurst + 1);

        // Clamp burst to remaining capacity
        int allowed = Mathf.Min(count, maxPopulation - currentPopulation);

        for (int i = 0; i < allowed; i++)
        {
            Vector3 spawnPos = GetRandomPointOnRing(player.position, minSpawnRadius, maxSpawnRadius);
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

            currentPopulation++;

            // Hook death callback
            EnemyMaster em = enemy.GetComponent<EnemyMaster>();
            if (em != null)
            {
                em.OnDeath += OnEnemyDeath;
            }
        }
    }

    private void OnEnemyDeath(EnemyMaster enemy)
    {
        currentPopulation = Mathf.Max(0, currentPopulation - 1);
    }


    private Vector3 GetRandomPointOnRing(Vector3 center, float minR, float maxR)
    {
        // angle-based direction = no bias, no zero vector
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)); // unit circle on XZ

        float radius = Random.Range(minR, maxR);
        Vector3 p = center + dir * radius;

        // lock Y (spawn on your floor height)
        p.y = center.y;
        return p;
    }

    private void OnDrawGizmosSelected()
    {
        if (!player) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, minSpawnRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, maxSpawnRadius);
    }
}