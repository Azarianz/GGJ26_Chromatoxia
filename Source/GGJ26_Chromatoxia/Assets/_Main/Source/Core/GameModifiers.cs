using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Normal, Fast, Huge, Ranger, Elite, Boss }

public class GameModifiers : MonoBehaviour
{
    public static GameModifiers Instance { get; private set; }

    [Header("Enemy")]
    public List<EnemyType> enemyTypes = new();
    public float enemyHpMult = 1f;
    public float enemySpeedMult = 1f;
    public float spawnMult = 1f;
    public float eliteChance = 0f;

    [Header("Loot")]
    public float lootDrawMult = 1f;
    public float dropBonus = 0f;

    [Header("Oxygen")]
    public float oxygenDrainMult = 1f;

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

        oxygenDrainMult = 1f;

        enemyTypes.Clear();
    }
}