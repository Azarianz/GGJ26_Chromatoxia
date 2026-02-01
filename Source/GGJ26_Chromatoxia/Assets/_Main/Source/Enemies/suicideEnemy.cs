using System.Collections;
using UnityEngine;

public class suicideEnemy : EnemyMaster
{
    public float stopDistance;
    [SerializeField] float damageAMt;

    bool isExploding = false;

    public override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (isExploding) return;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer < stopDistance)
        {
            StartCoroutine(ExplodeAfterDelay());
        }
        else
        {
            Move();
        }
    }

    IEnumerator ExplodeAfterDelay()
    {
        isExploding = true;

        // OPTIONAL: stop movement completely
        moveSpeed = 0;

        yield return new WaitForSeconds(1f);

        if (player != null)
        {
            player.GetComponent<PlayerController>()
                  .TakeOxygenDamage(damageAMt);
        }

        Destroy(gameObject);
    }

    
}