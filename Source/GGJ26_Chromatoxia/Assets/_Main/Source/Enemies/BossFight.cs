using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossFight : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] GameObject enemyHealth;

    [Header("Timing")]
    public float attackInterval = 3f;

    [Header("Attack Prefabs")]
    public GameObject spikeAttackPrefab;      // Attack 1
    public GameObject chasingProjectilePrefab; // Attack 2
    public GameObject randomShotPrefab;        // Attack 3

    [Header("References")]
    public Transform player;
    public Transform firePoint;

    bool isPhaseTwo = false;

    void Start()
    {
      
        StartCoroutine(AttackLoop());

    }

    void Update()
    {
        if (!isPhaseTwo && enemyHealth.GetComponent<enemyDamage>().currentHP <= enemyHealth.GetComponent<enemyDamage>().maxHP * 0.3f)
        {
            isPhaseTwo = true;
            Debug.Log("PHASE 2 START");
        }
    }

    IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);

            if (!isPhaseTwo)
            {
                // Phase 1 Å® 1 random attack
                DoRandomAttack();
            }
            else
            {
                // Phase 2 Å® 2 different attacks at once
                DoTwoRandomAttacks();
            }
        }
    }

    void DoRandomAttack()
    {
        int attackIndex = Random.Range(0, 3);

        switch (attackIndex)
        {
            case 0:
                SpikeAttack();
                break;
            case 1:
                ChasingProjectileAttack();
                break;
            case 2:
                Random360ShotAttack();
                break;
        }
    }

    void DoTwoRandomAttacks()
    {
        List<int> attacks = new List<int> { 0, 1, 2 };

        int first = attacks[Random.Range(0, attacks.Count)];
        attacks.Remove(first);

        int second = attacks[Random.Range(0, attacks.Count)];

        ExecuteAttack(first);
        ExecuteAttack(second);
    }

    void ExecuteAttack(int index)
    {
        switch (index)
        {
            case 0:
                SpikeAttack();
                break;
            case 1:
                ChasingProjectileAttack();
                break;
            case 2:
                Random360ShotAttack();
                break;
        }
    }
    void SpikeAttack()
    {
        int spikeCount = 6;

        for (int i = 0; i < spikeCount; i++)
        {
            float radius = Random.Range(1f, 5f);
            Vector2 randomCircle = Random.insideUnitCircle * radius;

            Vector3 spawnPos = player.position +
                               new Vector3(randomCircle.x, 0f, randomCircle.y);

            Instantiate(spikeAttackPrefab, spawnPos, Quaternion.identity);
        }

    }
    void ChasingProjectileAttack()
    {
        Instantiate(chasingProjectilePrefab, firePoint.position, Quaternion.identity);
    }
    void Random360ShotAttack()
    {
        int shots = Random.Range(40, 50);

        for (int i = 0; i < shots; i++)
        {
            float angle = Random.Range(0f, 360f);
            Quaternion rotation = Quaternion.Euler(0, angle, 0);

            Instantiate(randomShotPrefab, firePoint.position, rotation);
        }
    }
}
