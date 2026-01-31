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

        Vector2 moveDir = (player.position - transform.position).normalized;
        Vector2 separationDir = GetSeparationDirection();

        Vector2 finalDir = (moveDir + separationDir).normalized;

        transform.position += (Vector3)finalDir * moveSpeed * Time.deltaTime;
    }
    public virtual void OnHitboxTriggerEnter(Collider2D other)
    { if (other.CompareTag("Player"))
        {
            Debug.Log($"{name} hit the player"); 
        }
    }

    Vector2 GetSeparationDirection()
    {
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(
            transform.position,
            separationRadius,
            enemyLayer
        );

        Vector2 separation = Vector2.zero;
        int count = 0;

        foreach (Collider2D col in neighbors)
        {
            if (col.transform == transform) continue;

            Vector2 diff = (Vector2)(transform.position - col.transform.position);
            float distance = diff.magnitude;

            if (distance > 0)
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
