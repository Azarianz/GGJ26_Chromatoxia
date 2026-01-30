using UnityEngine;

public class BasicEnemyOne : MonoBehaviour
{
    private EnemyMaster enemyMaster;

    void Awake()
    {
        enemyMaster = GetComponentInParent<EnemyMaster>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (enemyMaster != null)
        {
            enemyMaster.OnHitboxTriggerEnter(other);
        }
    }
}
