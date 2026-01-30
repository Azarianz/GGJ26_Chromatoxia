using UnityEngine;

public class EnemyMaster : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] protected int maxHP = 50;
    [SerializeField] protected float moveSpeed = 3f;

    [Header("Combat")]
    [SerializeField] protected GameObject hitbox;

    protected int currentHP;
    protected Transform player;

    public virtual void Awake()
    {
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // =========================
    // Hitbox Callback
    // =========================

    public virtual void OnHitboxTriggerEnter(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"{name} hit the player");
        }
    }

    // =========================
    // Core
    // =========================

    public virtual void Move()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
    }

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
