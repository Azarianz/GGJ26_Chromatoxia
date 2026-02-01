using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyDamage : MonoBehaviour
{
    [SerializeField]float currentHP;
    [SerializeField] float maxHP;
    // Start is called before the first frame update
   
    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
            Destroy(gameObject);
    }
}
