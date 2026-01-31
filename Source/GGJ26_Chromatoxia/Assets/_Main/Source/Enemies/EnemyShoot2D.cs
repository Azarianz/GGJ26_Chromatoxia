using UnityEngine;

public class EnemyShoot2D : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 8f;
    public float fireRate = 1f; // seconds between shots

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
            Debug.LogError("EnemyShoot2D: No Player found with tag " + playerTag);
    }

 
    public void Shoot()
    {
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        Vector2 direction = (player.position - transform.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }

        // Optional: rotate bullet to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
