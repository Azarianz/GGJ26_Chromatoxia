using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemy : EnemyMaster
{
    public float stopDistance;
    [SerializeField]EnemyShoot2D eS2;
    public override void Awake()
    {
        base.Awake();

    }
    private void Update()
    {

        float distanceToPlayer = (player.position - transform.position).magnitude;

        print(distanceToPlayer);
        if (distanceToPlayer < stopDistance)
        {
            if (player == null) return;

            eS2.fireTimer += Time.deltaTime;

            if (eS2.fireTimer >= eS2.fireRate)
            {
                eS2.Shoot();
                eS2.fireTimer = 0f;
            }
           
        }
        else
        {
            Move();
        }
    }
}
