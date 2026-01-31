using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damageDistance = 1f;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float damageCooldown = 1f;

    private float lastDamageTime;

    // Call this from Enemy script
    public void TryDamagePlayer(Transform enemy)
    {
        if (enemy == null) return;

        // XZ-only distance
        Vector3 playerPos = transform.position;
        Vector3 enemyPos = enemy.position;

        playerPos.y = 0f;
        enemyPos.y = 0f;

        float distance = Vector3.Distance(playerPos, enemyPos);

        if (distance <= damageDistance && Time.time >= lastDamageTime + damageCooldown)
        {
            TakeDamage(damageAmount);
            lastDamageTime = Time.time;
        }
    }

    void TakeDamage(int amount)
    {
        Debug.Log($"Player took {amount} damage");
        // TODO: reduce HP, trigger UI, etc.
    }
}

