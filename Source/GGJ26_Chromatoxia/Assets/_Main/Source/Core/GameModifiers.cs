using System.Collections.Generic;
using UnityEngine;

public class GameModifiers : MonoBehaviour
{
    public static GameModifiers Instance { get; private set; }

    [Header("Enemy")]
    public List<GameObject> defaultEnemyPool = new List<GameObject>();
    public List<GameObject> enemyPool = new List<GameObject>();
    public int maxPopulation = 40;
    public float enemyHpMult = 1f;
    public float enemySpeedMult = 1f;
    public float spawnMult = 1f;
    public float eliteChance = 0f;

    [Header("Loot")]
    public float lootDrawMult = 1f;
    public float dropBonus = 0f;

    [Header("Oxygen")]
    public float oxygenDrainMultiplier = 1f;
    public float toxinGainMultiplier = 2f;

    [Header("Survival Time (Gradually Longer)")]
    public float survivalTime = 30f;    //Gradually increases

    void Awake()
    {
        // In bootstrap architecture, this prevents duplicates if you ever reload bootstrap by mistake.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // DO NOT DontDestroyOnLoad in bootstrap architecture.
        // Bootstrap scene persistence handles it.
    }

    public void ResetAll()
    {
        enemyHpMult = 1f;
        enemySpeedMult = 1f;
        spawnMult = 1f;
        eliteChance = 0f;

        lootDrawMult = 1f;
        dropBonus = 0f;

        oxygenDrainMultiplier = 1f;
        survivalTime = 30f;    //Gradually increases

        enemyPool.Clear();
        enemyPool.AddRange(defaultEnemyPool);   //copy default pool
    }
}