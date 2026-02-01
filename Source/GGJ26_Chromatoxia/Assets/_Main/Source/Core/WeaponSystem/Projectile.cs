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

    public void Init(GameObject owner, Vector3 dir, float speed, int damage, float lifetime)
    {
        Debug.Log("Owner of projectile: " + (owner != null ? owner.name : "null"));
        this.owner = owner;
        this.dir = dir.normalized;
        this.speed = speed;
        this.damage = damage;
        this.lifetime = lifetime;
        spawnTime = Time.time;
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
        Debug.Log("Projectile collided with: " + other.name);

        // Ignore self & projectile collisions
        if (owner != null && other.transform.IsChildOf(owner.transform) 
            && !other.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("Projectile ignored self collision.");
            return;
        }

        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg != null)
            dmg.TakeDamage(damage);

        Kill();
    }

    void Kill()
    {
        hit = true;

        // optional hit VFX
        if (hitVfxPrefab != null)
            Instantiate(hitVfxPrefab, transform.position, Quaternion.identity);

        // detach smoke so it can finish naturally
        if (smoke != null)
        {
            smoke.transform.SetParent(null, true);
            smoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            // destroy after it finishes
            float t = smoke.main.duration + smoke.main.startLifetime.constantMax;
            Destroy(smoke.gameObject, t);
        }

        Destroy(gameObject);
    }
}