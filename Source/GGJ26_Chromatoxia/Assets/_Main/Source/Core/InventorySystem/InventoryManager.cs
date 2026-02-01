using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager I { get; private set; }

    public const int SLOT_COUNT = 6;

    [Header("Runtime Slots")]
    public InventorySlot[] slots = new InventorySlot[SLOT_COUNT];

    public int activeSlotIndex { get; private set; } = -1;

    // Fires when active slot changes (for WeaponManager, UI highlight later, etc.)
    public System.Action<int, InventorySlot> OnActiveSlotChanged;

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

    void Update()
    {
        HandleActiveSlotInput();
    }

    public void SetActiveSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        activeSlotIndex = index;
        OnActiveSlotChanged?.Invoke(activeSlotIndex, slots[activeSlotIndex]);
    }

    void HandleActiveSlotInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetActiveSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetActiveSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetActiveSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetActiveSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetActiveSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SetActiveSlot(5);
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

        // If nothing active yet, make this active
        if (activeSlotIndex < 0)
            SetActiveSlot(emptyIndex);

        // If you want: auto-switch to newly picked up gun
        if (item.type == ItemType.Gun) SetActiveSlot(emptyIndex);
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

    // ===== WEAPONS & ITEMS: consume ammo/uses/charges =====
    public bool TryConsume(string itemId, int amount)
    {
        int idx = FindItemIndex(itemId);
        if (idx < 0) return false;

        var s = slots[idx];
        if (s.IsEmpty) return false;
        if (s.current < amount) return false;

        s.current -= amount;
        RedrawUI();
        return true;
    }

    public int GetCurrent(string itemId)
    {
        int idx = FindItemIndex(itemId);
        if (idx < 0) return 0;
        return slots[idx].current;
    }

    public bool TryConsumeAmmo(string itemId, int amount)
    {
        int idx = FindItemIndex(itemId);
        if (idx < 0) return false;

        var s = slots[idx];
        if (s == null || s.IsEmpty) return false;
        if (s.current < amount) return false;

        s.current -= amount;
        RedrawUI(); // ✅ THIS is what updates your UI every shot
        return true;
    }

    public int GetAmmo(string itemId)
    {
        int idx = FindItemIndex(itemId);
        if (idx < 0) return 0;
        return slots[idx].current;
    }
}