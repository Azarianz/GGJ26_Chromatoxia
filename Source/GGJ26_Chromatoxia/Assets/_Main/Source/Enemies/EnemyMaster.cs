using UnityEngine;

public class EnemyMaster : MonoBehaviour, IDamageable
{
    [Header("Base Stats")]
    [SerializeField] public float moveSpeed = 3f;
    public float currentHP;
    public float maxHP;

    [Header("Combat")]
    [SerializeField] protected GameObject hitbox;

    [Header("Swarm Settings")]
    [SerializeField] private float separationRadius = 1.2f;
    [SerializeField] private float separationStrength = 1.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] Animator enemyAnim;
   
    public Transform player;

    public System.Action<EnemyMaster> OnDeath;


    public virtual void Awake()
    {
        InitStat();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void InitStat()
    {
        moveSpeed = GameModifiers.Instance.enemySpeedMult * moveSpeed;
        maxHP = GameModifiers.Instance.enemyHpMult * maxHP;
        currentHP = maxHP;
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

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(player.position.x - transform.position.x) * Mathf.Abs(scale.x);
        transform.localScale = scale;

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
    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        enemyAnim.SetTrigger("enemDamg");
        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

}
