using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyDamage : MonoBehaviour
{
    public float currentHP;
    public float maxHP;
    // Start is called before th
    //
    //
    // e first frame update

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(30);
            print(currentHP);
        }
    }
    private void Start()
    {
        currentHP = maxHP;
    }
    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
            Destroy(gameObject);
    }
}
