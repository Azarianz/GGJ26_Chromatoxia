using System;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public ItemData item;   // null = empty
    public int current;     // current ammo/uses/charges (0..maxCapacity)

    public bool IsEmpty => item == null;

    public void Clear()
    {
        item = null;
        current = 0;
    }

    public void Set(ItemData newItem, int amount)
    {
        item = newItem;
        current = Mathf.Clamp(amount, 0, item.maxCapacity);
    }

    public void AddToCurrent(int amount)
    {
        if (item == null) return;
        current = Mathf.Clamp(current + amount, 0, item.maxCapacity);
    }

    public void RefillToMax()
    {
        if (item == null) return;
        current = item.maxCapacity;
    }
}