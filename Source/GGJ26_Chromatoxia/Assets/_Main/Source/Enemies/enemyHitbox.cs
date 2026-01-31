using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHitbox : MonoBehaviour
{
    [SerializeField] float damageAmt;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Get the root GameObject of this hitbox
            GameObject root = transform.root.gameObject;

            // Try to get the EnemyMaster component (or child class)
            EnemyMaster enemy = root.GetComponent<EnemyMaster>();

            if (enemy != null)
            {
                enemy.OnHitboxTriggerEnter(collision);
                collision.GetComponent<PlayerController>().currentOxygen -= damageAmt;
            }
            else
            {
                Debug.LogWarning("No EnemyMaster found on root of hitbox!");
            }
        }
    }
}
