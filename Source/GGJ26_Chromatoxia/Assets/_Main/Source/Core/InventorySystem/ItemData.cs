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
    public string id;
    public string displayName;
    public ItemType type;
    public Sprite icon;

    public int maxCapacity = 30;
    public int pickupRestoreAmount = 999999; // effectively "refill"
}