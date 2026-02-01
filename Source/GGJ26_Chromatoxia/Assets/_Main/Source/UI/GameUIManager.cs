using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public Image[] inventorySlotIcons = new Image[6];
    public TMP_Text[] inventorySlotCounts = new TMP_Text[6];

    [Header("Game End UI")]
    public GameObject winCanvas;
    public TMP_Text winText;

    public GameObject loseCanvas;
    public TMP_Text loseText;

    [Range(0f, 1f)]
    public float emptySlotAlpha = 0f;

    public float winReturnDelay = 3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Optional: ensure both are hidden on scene start
        HideGameEnd();
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

    public void InitWave(float maxWaveHealth, float currentWaveHealth)
    {
        if (!waveSlider) return;

        waveSlider.minValue = 0;
        waveSlider.maxValue = maxWaveHealth;
        waveSlider.value = currentWaveHealth;
    }

    public void UpdateWave(float currentWave)
    {
        if (!waveSlider) return;

        waveSlider.value = currentWave;
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

            if (icon == null) continue;

            if (slot == null || slot.IsEmpty)
            {
                icon.sprite = null;
                icon.color = new Color(1, 1, 1, 0f);

                if (text) text.text = "";
            }
            else
            {
                icon.sprite = slot.item.icon;
                icon.color = Color.white;

                if (text)
                    text.text = slot.current.ToString();
            }
        }
    }
    IEnumerator ReturnToStageGraphAfterDelay()
    {
        // Pause gameplay but allow UI to update
        Time.timeScale = 0f;

        float t = 0f;
        while (t < winReturnDelay)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1f;
        HideGameEnd();

        if (RunManager.I != null) RunManager.I.ClearCurrentNode();

        // Go back to stage select
        LevelManager.I.EndToStageSelect();
    }


    // =====================
    // Called by GameManager
    // =====================
    public void ShowGameEnd(GameEndType type, string reason)
    {
        // Always start clean
        HideGameEnd();

        string baseMsg = (type == GameEndType.Win) ? "ZONE CLEARED" : "GAME OVER";
        string msg = string.IsNullOrEmpty(reason) ? baseMsg : $"{baseMsg}\n{reason}";

        if (type == GameEndType.Win)
        {
            if (winCanvas) winCanvas.SetActive(true);
            if (winText) winText.text = msg;

            // 🔹 Delay then return to stage graph
            StartCoroutine(ReturnToStageGraphAfterDelay());
        }
        else
        {
            if (loseCanvas) loseCanvas.SetActive(true);
            if (loseText) loseText.text = msg;
        }
    }

    public void HideGameEnd()
    {
        if (winCanvas) winCanvas.SetActive(false);
        if (loseCanvas) loseCanvas.SetActive(false);
    }

    // =====================
    // Button Hooks
    // =====================
    public void OnMenuClicked()
    {
        Time.timeScale = 1f;
        HideGameEnd();
        LevelManager.I.EndToMenu();
    }
}