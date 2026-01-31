using UnityEngine;

public class EnemyMaster : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private int maxHP = 50;
    [SerializeField] public float moveSpeed = 3f;

    [Header("Combat")]
    [SerializeField] protected GameObject hitbox;

    [Header("Swarm Settings")]
    [SerializeField] private float separationRadius = 1.2f;
    [SerializeField] private float separationStrength = 1.5f;
    [SerializeField] private LayerMask enemyLayer;

    protected int currentHP;
    public Transform player;

    public virtual void Awake()
    {
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // =========================
    // Core
    // =========================

    public virtual void Move()
    {
        if (player == null) return;

        // Direction to player (XZ only)
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        Vector3 separationDir = GetSeparationDirectionXZ();

        Vector3 finalDir = (toPlayer.normalized + separationDir).normalized;

        transform.position += finalDir * moveSpeed * Time.deltaTime;
    }

    Vector3 GetSeparationDirectionXZ()
    {
        Collider[] neighbors = Physics.OverlapSphere(
            transform.position,
            separationRadius,
            enemyLayer
        );

        Vector3 separation = Vector3.zero;
        int count = 0;

        foreach (Collider col in neighbors)
        {
            if (col.transform == transform) continue;

            Vector3 diff = transform.position - col.transform.position;
            diff.y = 0f;

            float distance = diff.magnitude;

            if (distance > 0f)
            {
                separation += diff.normalized / distance;
                count++;
            }
        }

        if (count > 0)
            separation /= count;

        return separation * separationStrength;
    }

    // =========================
    // Combat
    // =========================

    public virtual void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
