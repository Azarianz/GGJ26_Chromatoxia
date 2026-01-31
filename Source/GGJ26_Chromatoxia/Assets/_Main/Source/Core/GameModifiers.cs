using UnityEngine;

public class GameModifiers : MonoBehaviour
{
    [Header("Enemy")]
    public float enemyHpMult = 1f;
    public float enemySpeedMult = 1f;
    public float spawnMult = 1f;
    public float eliteChance = 0f;

    [Header("Loot")]
    public float lootDrawMult = 1f;
    public float dropBonus = 0f;

    [Header("Oxygen")]
    public float oxygenDrainMult = 1f;
}
