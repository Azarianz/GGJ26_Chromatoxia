using UnityEngine;
using UnityEngine.UI;

public class PlayerFaceController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerController player;
    private Image faceImage;

    [SerializeField] private Sprite maskOn_WithOxygenCanister;        // mask with canister (undamaged)
    [SerializeField] private Sprite maskDamaged_WithOxygenCanister;   // mask with canister (damaged)

    [SerializeField] private Sprite maskOn_UndamagedArmor;            // mask no canister (undamaged)
    [SerializeField] private Sprite maskDamaged_NoOxygenCanister;     // mask no canister (damaged)

    [SerializeField] private Sprite unmasked_Healthy;
    [SerializeField] private Sprite unmasked_Toxin_0_49;
    [SerializeField] private Sprite unmasked_Toxin_50;


    Sprite _last;

    void Reset()
    {
        faceImage = GameUIManager.Instance.characterPortrait;
    }

    void Awake()
    {
        if (!faceImage) faceImage = GameUIManager.Instance.characterPortrait;
        if (!player) player = FindFirstObjectByType<PlayerController>();
    }

    void Start() => Refresh();
    void Update() => Refresh(); // easiest; later you can optimize

    public void Refresh()
    {
        if (!player || !faceImage) return;

        Sprite next = ChooseSprite();
        if (next != null && next != _last)
        {
            faceImage.sprite = next;
            _last = next;
        }
    }

    Sprite ChooseSprite()
    {
        // Mask ONLY when armor > 0
        if (player.HasArmor)
        {
            // Oxygen decides canister vs no-canister mask
            if (player.HasOxygen)
            {
                // With canister
                return (player.Armor01 <= 0.5f)
                    ? maskDamaged_WithOxygenCanister
                    : maskOn_WithOxygenCanister;
            }
            else
            {
                //show mask: undamaged vs damaged (without oxygen canister)
                return (player.Armor01 <= 0.5f)
                ? maskDamaged_NoOxygenCanister
                : maskOn_UndamagedArmor;
            }
        }

        // 2) Unmasked phase (armor == 0)
        float t = player.Toxin01; // 0..1
        //float o = player.Oxygen01; // 0..1

        if (!player.IsHurt) return unmasked_Healthy;
        if (player.IsHurt && t <= 0.78f) return unmasked_Toxin_0_49;
        return unmasked_Toxin_50;
    }
}
    