using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

/// <summary>
/// Aims a hovering weapon sprite toward the mouse (XZ ground) using Z-rotation only,
/// and flips the sprite on X when aiming left.
/// InventoryManager owns active slot selection and notifies via OnActiveSlotChanged.
/// </summary>
public class WeaponManager : MonoBehaviour
{
    [Header("Mouse Aim")]
    public Camera cam;
    public LayerMask groundMask;
    public float raycastDistance = 500f;

    [Header("Player Visual")]
    public GameObject playerREF;
    public SpriteRenderer playerRenderer; // drag the player sprite here

    [Header("Weapon Visual")]
    public Transform weaponPivot;          // empty pivot above player
    public SpriteRenderer weaponRenderer;  // weapon sprite
    public Transform muzzle;               // projectile spawn point (child of weaponPivot)

    [Header("Sprite Rotation / Flip")]
    [Tooltip("If your sprite art points UP, set -90. If it points RIGHT, set 0.")]
    public float spriteAngleOffset = 0f;

    [Tooltip("Flip X when aiming left (recommended).")]
    public bool flipXWhenAimingLeft = true;

    [Header("Shooting")]
    public bool holdToFire = true;

    InventoryManager inv;
    ItemData gun;
    float nextFireTime;

    Vector3 lastAimPoint;
    bool hasAimPoint;

    void Start()
    {
        inv = InventoryManager.I;
        if (cam == null) cam = Camera.main;

        if (inv != null)
        {
            inv.OnActiveSlotChanged += OnActiveSlotChanged;

            // Init equip from active slot
            if (inv.activeSlotIndex >= 0 && inv.activeSlotIndex < inv.slots.Length)
                OnActiveSlotChanged(inv.activeSlotIndex, inv.slots[inv.activeSlotIndex]);
            else
                Equip(null);
        }
        else
        {
            Equip(null);
        }
    }

    void OnDestroy()
    {
        if (inv != null)
            inv.OnActiveSlotChanged -= OnActiveSlotChanged;
    }

    void Update()
    {
        if (cam == null || weaponPivot == null || muzzle == null)
            return;

        // Aim (works even if gun is null; you can gate it if you want)
        if (TryGetMouseGroundPoint(out Vector3 aimPoint))
        {
            AimWeaponZOnly(aimPoint);
            lastAimPoint = aimPoint;
            hasAimPoint = true;
        }
        else
        {
            hasAimPoint = false;
        }

        // Fire
        if (gun == null) return;

        bool firePressed = holdToFire ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
        if (firePressed)
            TryFire();
    }

    void OnActiveSlotChanged(int slotIndex, InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty || slot.item == null)
        {
            Equip(null);
            return;
        }

        // Only equip guns here
        if (slot.item.type == ItemType.Gun)
            Equip(slot.item);
        else
            Equip(null);
    }

    void Equip(ItemData newGun)
    {
        gun = newGun;
        nextFireTime = 0f;

        if (weaponRenderer != null)
        {
            if (gun != null)
            {
                // Use weaponWorldSprite if you added it; otherwise icon
                Sprite s = gun.weaponWorldSprite != null ? gun.weaponWorldSprite : gun.icon;
                weaponRenderer.sprite = s;
                weaponRenderer.enabled = (s != null);
            }
            else
            {
                weaponRenderer.sprite = null;
                weaponRenderer.enabled = false;
            }
        }
    }

    bool TryGetMouseGroundPoint(out Vector3 point)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // Choose the plane your gameplay happens on.
        // If your floor is at y = 0, use y = 0.
        // If your player floats at y = something, use weaponPivot.position.y.
        float yPlane = weaponPivot.position.y;

        Plane plane = new Plane(Vector3.up, new Vector3(0f, yPlane, 0f));

        // Debug: draw ray
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.yellow);

        if (plane.Raycast(ray, out float enter))
        {
            point = ray.GetPoint(enter);

            // Debug: hit cross
            Debug.DrawLine(point + Vector3.left * 0.2f, point + Vector3.right * 0.2f, Color.cyan);
            Debug.DrawLine(point + Vector3.forward * 0.2f, point + Vector3.back * 0.2f, Color.cyan);

            return true;
        }

        point = default;
        return false;
    }

    void AimWeaponZOnly(Vector3 aimPoint)
    {
        // Compute direction on XZ plane
        Vector3 dir = aimPoint - weaponPivot.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        // Determine left/right BEFORE normalization for stability
        bool aimingLeft = dir.x < 0f;

        // Angle: 0 = +X (right), 90 = +Z (forward)
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

        // Apply ONLY Z rotation (no X/Y)
        weaponPivot.localEulerAngles = new Vector3(0f, 0f, angle + spriteAngleOffset);

        // Flip X if aiming opposite direction
        if (weaponRenderer != null && flipXWhenAimingLeft)
            weaponRenderer.flipY = aimingLeft;

        // Flip player
        if (playerRenderer != null)
            playerRenderer.flipX = aimingLeft;

        // DEBUG LINES
        Debug.DrawLine(weaponPivot.position, weaponPivot.position + dir.normalized * 3f, Color.green); // intended
        Debug.DrawLine(weaponPivot.position, weaponPivot.position + muzzle.right.normalized * 3f, Color.red); // actual
    }

    void TryFire()
    {
        if (gun == null)
        {
            //Debug.LogWarning("TryFire: gun is null (not equipped).");
            return;
        }

        if (Time.time < nextFireTime)
            return;

        if (inv == null)
        {
            Debug.LogError("TryFire: InventoryManager is null.");
            return;
        }

        int ammoCost = Mathf.Max(1, gun.ammoPerShot);

        bool consumed = inv.TryConsumeAmmo(gun.id, ammoCost);
        if (!consumed)
        {
            //Debug.Log($"TryFire: No ammo or gun not in inventory. id={gun.id}, cost={ammoCost}");
            return;
        }

        nextFireTime = Time.time + (1f / Mathf.Max(0.01f, gun.fireRate));

        //.Log($"TryFire: Firing gun={gun.id}");
        SpawnProjectile();
    }

    void SpawnProjectile()
    {
        if (!hasAimPoint) return;
        if (gun.projectilePrefab == null) return;

        // Compute aim direction (world)
        Vector3 aimDir = lastAimPoint - weaponPivot.position;
        aimDir.y = 0f;

        if (aimDir.sqrMagnitude < 0.0001f)
            return;

        aimDir.Normalize();

        // Rotate prefab root to face aim direction
        Quaternion aimRot = Quaternion.LookRotation(aimDir, Vector3.up);

        // Spawn prefab root
        GameObject go = Instantiate(
            gun.projectilePrefab,
            muzzle.position,
            aimRot
        );

        // THIS is the correct call
        Projectile[] bullets = go.GetComponentsInChildren<Projectile>();
        foreach (var bullet in bullets)
            InitBulletFromTransform(bullet);
    }

    void InitBulletFromTransform(Projectile bullet)
    {
        // IMPORTANT:
        // Use the bullet's WORLD-space facing direction.
        // Do NOT use localRotation.
        // Do NOT recompute spread.
        // Do NOT zero Y manually.

        Vector3 dir = bullet.transform.forward;

        // Project cleanly onto XZ plane (safe)
        dir = Vector3.ProjectOnPlane(dir, Vector3.up);

        if (dir.sqrMagnitude < 0.00001f)
        {
            Debug.LogWarning("Bullet direction collapsed to zero. Check prefab orientation.");
            return;
        }

        dir.Normalize();

        Debug.DrawRay(bullet.transform.position, dir * 3f, Color.magenta, 0.5f);

        bullet.Init(
            playerREF,
            dir,
            gun.projectileSpeed,
            gun.damage,
            gun.projectileLifetime,
            gun.hasExplosion,
            gun.penetration
        );
    }
}