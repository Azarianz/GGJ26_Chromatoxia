using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damageDistance = 1f;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float damageCooldown = 1f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 6f;

    private float lastDamageTime;
    private Transform enemyRoot;

    void Awake()
    {
        // Cache enemy root transform
        enemyRoot = transform.root;
    }

    // Called from Enemy script
    public void TryDamagePlayer(Transform player)
    {
        if (player == null) return;

        Vector3 playerPos = player.position;
        Vector3 enemyPos = enemyRoot.position;

        playerPos.y = 0f;
        enemyPos.y = 0f;

        float distance = Vector3.Distance(playerPos, enemyPos);

        if (distance <= damageDistance && Time.time >= lastDamageTime + damageCooldown)
        {
            ApplyDamage(player);
            ApplyKnockback(player);
            lastDamageTime = Time.time;
        }
    }

    void ApplyDamage(Transform player)
    {
        player.GetComponent<PlayerController>()
              ?.TakeOxygenDamage(damageAmount);
    }

    void ApplyKnockback(Transform player)
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb == null) return;

        Vector3 dir = (player.position - enemyRoot.position);
        dir.y = 0f;
        dir.Normalize();

        playerRb.AddForce(dir * knockbackForce, ForceMode.Impulse);
    }
}
