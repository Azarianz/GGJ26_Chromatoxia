using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Optional VFX")]
    public ParticleSystem smoke;   // drag your smoke system here in prefab
    public GameObject hitVfxPrefab; // optional explosion prefab

    GameObject owner;
    Vector3 dir;
    float speed, lifetime, spawnTime;
    int damage;

    bool hit;
    bool hasExplosion;
    bool isExplosion;   //hardcoded ASF

    int penetrationRemaining;

    // Prevent hitting the same enemy multiple times
    readonly HashSet<Collider> hitTargets = new();

    public void Init(
        GameObject owner,
        Vector3 dir,
        float speed,
        int damage,
        float lifetime,
        bool hasExplosion,
        int penetration = 1
    )
    {
        this.owner = owner;
        this.dir = dir.normalized;
        this.speed = speed;
        this.damage = damage;
        this.lifetime = lifetime;
        this.hasExplosion = hasExplosion;

        penetrationRemaining = Mathf.Max(1, penetration);
        spawnTime = Time.time;
        hit = false;
        isExplosion = false;
        hitTargets.Clear();
    }

    void Update()
    {
        if (hit) return;

        transform.position += dir * speed * Time.deltaTime;

        if (Time.time - spawnTime >= lifetime)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hit) return;

        // Ignore self collision
        if (owner != null &&
            other.transform.IsChildOf(owner.transform) &&
            !other.CompareTag("Projectile"))
            return;

        // Prevent double-hit on same collider
        if (hitTargets.Contains(other))
            return;

        // Damageable target
        var damageable = other.GetComponentInParent<IDamageable>();
        if (other.CompareTag("Enemy") || damageable != null)
        {
            hitTargets.Add(other);

            // Prefer interface (most flexible)
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            else
            {
                // Fallback if you still want EnemyMaster
                var enemy = other.GetComponentInParent<EnemyMaster>();
                if (enemy != null)
                    enemy.TakeDamage(damage);
                else
                    Debug.LogWarning($"Hit tagged Enemy but no IDamageable/EnemyMaster found on {other.name}", other);
            }

            penetrationRemaining--;
            if (penetrationRemaining <= 0)
                Kill();
        }
    }

    void Kill()
    {

        hit = true;

        // Spawn hit VFX
        if (hitVfxPrefab != null)
        {
            GameObject hitFx = Instantiate(hitVfxPrefab, transform.position, Quaternion.identity);

            if (hasExplosion)
            {
                var explosionFx = hitFx.GetComponent<Projectile>();
                if (explosionFx != null)
                {
                    explosionFx.Init(
                        null,
                        Vector3.up,
                        0f,
                        100,
                        1.0f,
                        false,
                        999 // explosion hits everything once
                    );
                    explosionFx.isExplosion = true;
                }
            }
        }

        // Detach smoke
        if (smoke != null)
        {
            smoke.transform.SetParent(null, true);
            smoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            float t = smoke.main.duration + smoke.main.startLifetime.constantMax;
            Destroy(smoke.gameObject, t);
        }

        Destroy(gameObject);
    }
}