using UnityEngine;

public class BasicEnemyOne : MonoBehaviour
{
    private EnemyMaster enemyMaster;

    void Awake()
    {
        enemyMaster = GetComponentInParent<EnemyMaster>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (enemyMaster != null)
        {
            enemyMaster.OnHitboxTriggerEnter(other);
        }
    }
}
