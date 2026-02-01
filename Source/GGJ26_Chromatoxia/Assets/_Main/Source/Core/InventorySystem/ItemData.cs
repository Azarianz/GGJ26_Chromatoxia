using UnityEngine;

public enum ItemType
{
    Gun,    
    Throwable,
    Healing
}

[CreateAssetMenu(menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string id;
    public string displayName;
    public ItemType type;
    public Sprite icon;
    public Sprite weaponWorldSprite; // optional, for guns

    [Header("Capacity (Ammo / Uses / Charges)")]
    public int maxCapacity = 30;
    public int pickupRestoreAmount = 999999; // effectively "refill"

    [Header("Gun Settings (only if type = Gun)")]
    public float fireRate = 8f;     // shots per second
    public int ammoPerShot = 1;
    public GameObject projectilePrefab; // optional (or hitscan)
    public float projectileSpeed = 20f;
    public float projectileLifetime = 2f;
    public int damage = 1;
}