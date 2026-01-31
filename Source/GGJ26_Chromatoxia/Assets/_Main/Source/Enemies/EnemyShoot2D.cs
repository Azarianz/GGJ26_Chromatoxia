using UnityEngine;

public class EnemyShoot2D : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 8f;
    public float fireRate = 1f;

    [Header("Targeting")]
    public string playerTag = "Player";

    private Transform player;
    public float fireTimer;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("EnemyShoot3D: No Player found with tag " + playerTag);
    }

    public void Shoot()
    {
        if (player == null) return;

        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // Direction ONLY on X and Z
        Vector3 direction = player.position - transform.position;
        direction.y = 0f; // ignore vertical difference
        direction.Normalize();

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }

        // Rotate bullet to face movement direction
        bullet.transform.rotation = Quaternion.LookRotation(direction);
    }
}
