using UnityEngine;
using UnityEngine.Animations;

public class EnemyMaster : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private int maxHP = 50;
    [SerializeField] public float moveSpeed = 3f;

    [Header("Combat")]
    [SerializeField] protected GameObject hitbox;

    protected int currentHP;
    public Transform player;
    

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

    public int stopSpeed(int iamspeed)
    {
        moveSpeed = iamspeed;
        return iamspeed;
    }
}
