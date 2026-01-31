using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var inv = InventoryManager.I;
        if (inv == null) return;

        if (inv.Acquire(itemData))
            Destroy(gameObject);
    }
}