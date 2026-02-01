using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class bulletScript : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 6f;
    [SerializeField] private float damageDistance = 1f;
    [SerializeField] bool makeItSay;
    private void Update()
    {
        TryDamagePlayer(GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>());
    }
    private void Start()
    {
        Destroy(gameObject, 3);
    }
    void TryDamagePlayer(Transform player)
    {
        if (player == null) return;

        Vector3 playerPos = player.position;
        Vector3 enemyPos = this.transform.position;

        playerPos.y = 0f;
        enemyPos.y = 0f;

        float distance = Vector3.Distance(playerPos, enemyPos);

        if (distance <= damageDistance)
        {
            ApplyDamage(player);
            ApplyKnockback(player);
            if (!makeItSay)
            {
                Destroy(gameObject);
            }
           
        }
    }

    void ApplyDamage(Transform player)
    {
        player.GetComponent<PlayerController>()
              ?.TakeOxygenDamage(damageAmount);
    }


void ApplyKnockback(Transform player)
{
    Rigidbody playerRb = player.GetComponent<Rigidbody>();
    if (playerRb == null) return;

    Vector3 dir = (player.position - this.transform.position);
    dir.y = 0f;
    dir.Normalize();

    playerRb.AddForce(dir * knockbackForce, ForceMode.Impulse);
}

}
