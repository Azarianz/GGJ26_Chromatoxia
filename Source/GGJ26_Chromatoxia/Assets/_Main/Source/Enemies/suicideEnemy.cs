using System.Collections;
using UnityEngine;

public class suicideEnemy : EnemyMaster
{
    public float stopDistance;
    [SerializeField] float damageAMt;
    [SerializeField] GameObject explodepart;

    bool isExploding = false;

    public override void Awake()
    {
        base.Awake();
        StartCoroutine(AutoExplodeAfterTime(3f));
    }

    private void Update()
    {
        if (isExploding) return;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer < stopDistance)
        {
            StartCoroutine(ExplodeAfterDelay(1f));
        }
        else
        {
            Move();
        }
    }

    IEnumerator AutoExplodeAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        if (!isExploding)
        {
            StartCoroutine(ExplodeAfterDelay(0f));
        }
    }

    IEnumerator ExplodeAfterDelay(float delay)
    {
        if (isExploding) yield break;

        isExploding = true;

        // Stop movement
        moveSpeed = 0;

        yield return new WaitForSeconds(delay);

        if (player != null &&
            Vector3.Distance(player.position, transform.position) <= stopDistance)
        {
            player.GetComponent<PlayerController>()
                  .RegisterDamage(damageAMt);
        }

        GameObject obj = Instantiate(
            explodepart,
            transform.position,
            Quaternion.identity
        );

        obj.transform.SetParent(null);
        Destroy(gameObject);
    }
}
