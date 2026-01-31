using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager I { get; private set; }

    public const int SLOT_COUNT = 6;

    [Header("Runtime Slots")]
    public InventorySlot[] slots = new InventorySlot[SLOT_COUNT];

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;

        for (int i = 0; i < slots.Length; i++)
            slots[i] ??= new InventorySlot();
    }

    void Start()
    {
        // initial draw
        RedrawUI();
    }

    public bool Acquire(ItemData item)
    {
        if (item == null) return false;

        // If already owned: restore ammo/uses/charges
        int existingIndex = FindItemIndex(item.id);
        if (existingIndex >= 0)
        {
            int restore = item.pickupRestoreAmount <= 0 ? item.maxCapacity : item.pickupRestoreAmount;
            slots[existingIndex].AddToCurrent(restore);
            RedrawUI();
            return true;
        }

        // else: next empty slot (left->right)
        int emptyIndex = FindEmptyIndex();
        if (emptyIndex < 0)
            return false; // inventory full

        // new item starts full
        slots[emptyIndex].Set(item, item.maxCapacity);
        RedrawUI();
        return true;
    }

    public int FindItemIndex(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return -1;

        for (int i = 0; i < slots.Length; i++)
            if (!slots[i].IsEmpty && slots[i].item.id == id)
                return i;

        return -1;
    }

    public int FindEmptyIndex()
    {
        for (int i = 0; i < slots.Length; i++)
            if (slots[i].IsEmpty)
                return i;

        return -1;
    }

    public void RedrawUI()
    {
        if (GameUIManager.Instance != null)
            GameUIManager.Instance.DrawInventory(slots);
    }
}