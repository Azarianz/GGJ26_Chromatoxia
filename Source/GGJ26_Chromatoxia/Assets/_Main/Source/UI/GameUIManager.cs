using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    // Singleton instance
    public static GameUIManager Instance { get; private set; }

    [Header("Player UI")]
    public Slider armorSlider;
    public Slider oxygenSlider;
    public Slider toxinSlider;

    [Header("Enemy UI")]
    public Slider waveSlider;

    [Header("Randomizer UI")]
    public GameObject randomizerListParent;
    public GameObject randomizerEffectCard;

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

    }
}