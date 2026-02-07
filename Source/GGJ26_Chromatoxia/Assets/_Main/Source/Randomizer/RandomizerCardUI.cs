using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomizerCardUI : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;

    [SerializeField] private Image frame;
    [Header("Frame Tint")]
    [SerializeField] private Color positiveColor = new Color(0.35f, 1f, 0.35f, 1f);
    [SerializeField] private Color negativeColor = new Color(1f, 0.35f, 0.35f, 1f);

    public void Setup(RandomEffect effect, int stacks)
    {
        if (!effect) return;

        if (icon) icon.sprite = effect.effectIcon;
        if (nameText) nameText.text = effect.effectName;

        // Optionally show stacks in name
        // nameText.text = stacks > 1 ? $"{effect.effectName} x{stacks}" : effect.effectName;

        if (descText) descText.text = effect.effectDescription;
        if (frame)
            frame.color = effect.isPositive ? positiveColor : negativeColor;
    }

    void Reset()
    {
        if (!frame) frame = transform.Find("Frame")?.GetComponent<Image>();
        if (!icon) icon = transform.Find("Icon")?.GetComponent<Image>();
        if (!nameText) nameText = transform.Find("Name")?.GetComponent<TMP_Text>();
        if (!descText) descText = transform.Find("Desc")?.GetComponent<TMP_Text>();
    }
}
