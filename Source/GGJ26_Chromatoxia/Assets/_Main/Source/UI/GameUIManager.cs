using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // <-- add this if you use TMP_Text

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("Player UI")]
    public Slider armorSlider;
    public Slider oxygenSlider;
    public Slider toxinSlider;

    [Header("Enemy UI")]
    public Slider waveSlider;

    [Header("Randomizer UI")]
    public GameObject randomizerUI;
    public GameObject randomizerListParent;
    public GameObject randomizerEffectCard;

    [Header("Inventory UI (6 Slots)")]
    public Image[] inventorySlotIcons = new Image[6];      // assign left->right
    public TMP_Text[] inventorySlotCounts = new TMP_Text[6]; // optional (ammo/uses)

    [Range(0f, 1f)]
    public float emptySlotAlpha = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ===== UI Update API =====
    public void InitArmor(float max, float current)
    {
        if (armorSlider == null) return;
        armorSlider.maxValue = max;
        armorSlider.value = current;
    }

    public void InitOxygen(float max, float current)
    {
        if (oxygenSlider == null) return;
        oxygenSlider.maxValue = max;
        oxygenSlider.value = current;
    }

    public void InitToxin(float max, float current)
    {
        if (toxinSlider == null) return;
        toxinSlider.maxValue = max;
        toxinSlider.value = current;
    }

    public void UpdateArmor(float value)
    {
        if (armorSlider != null)
            armorSlider.value = value;
    }

    public void UpdateOxygen(float value)
    {
        if (oxygenSlider != null)
            oxygenSlider.value = value;
    }

    public void UpdateToxin(float value)
    {
        if (toxinSlider != null)
            toxinSlider.value = value;
    }

    public void DrawRandomizer(List<RandomEffect> effectsToDraw)
    {
        // your existing randomizer drawing here
    }

    // ===== Inventory Draw API =====
    public void DrawInventory(InventorySlot[] slots)
    {
        if (slots == null) return;

        int count = Mathf.Min(6, slots.Length);

        for (int i = 0; i < count; i++)
        {
            var icon = inventorySlotIcons[i];
            var text = inventorySlotCounts[i];
            var slot = slots[i];

            if (slot == null || slot.IsEmpty)
            {
                // Hide icon
                icon.sprite = null;
                icon.color = new Color(1, 1, 1, 0);

                // Clear count
                if (text) text.text = "";
            }
            else
            {
                // Show icon
                icon.sprite = slot.item.icon;
                icon.color = Color.white;

                // Update count
                if (text)
                    text.text = slot.current.ToString();
            }
        }
    }
}