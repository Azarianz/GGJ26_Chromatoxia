using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour, IDamageable
{
    public bool HasArmor => currentArmorHP > 0f;
    public bool HasOxygen => currentOxygen > 0f;
    public bool HasHalfOxygen => currentOxygen > (maxOxygen * 0.5f);

    public bool ArmorIsDamaged =>
        currentArmorHP >= 0f && currentArmorHP <= (armorMaxHP * 0.5f);

    public float Toxin01 => maxToxin <= 0f ? 0f : currentToxin / maxToxin;

    public float Armor01 => armorMaxHP <= 0f ? 0f : currentArmorHP / armorMaxHP;
    public bool IsHurt => currentOxygen <= maxOxygen * 0.4f;

    // =====================
    // Player Stats
    // =====================
    public float moveSpeed = 5.0f;

    [Header("Armor")]
    public float armorMaxHP = 50;
    public float currentArmorHP { get; private set; }

    [Header("Oxygen")]
    public float maxOxygen = 100f;
    public float oxygenDrainRate = 0.85f; // per second
    public float currentOxygen { get; private set; }

    [Header("Toxins")]
    public float maxToxin = 100f;
    public float toxinFillRate = 10f; // per second
    public float currentToxin { get; private set; }

    public Animator animator;
    public GameObject asd;  //yo wad this for
    // Movement
    private Rigidbody rb;
    private Vector2 movementInput;

    void DrainOxygen(float val)
    {
        if (currentOxygen > 0)
        {
            if (HasArmor)
            {
                currentOxygen = Mathf.Max(0, currentOxygen - (val * GameModifiers.Instance.oxygenDrainMultiplier));
                GameUIManager.Instance?.UpdateOxygen(currentOxygen);
            }
            else{
                currentOxygen = Mathf.Max(0, currentOxygen - (val * (GameModifiers.Instance.oxygenDrainMultiplier * 2)));
                GameUIManager.Instance?.UpdateOxygen(currentOxygen);
            }
        }
        else
        {
            currentToxin = Mathf.Min(maxToxin, currentToxin + (val * (GameModifiers.Instance.toxinGainMultiplier)));
            GameUIManager.Instance?.UpdateToxin(currentToxin);
            if(currentToxin >= maxToxin)
            {
                // Player dies
                GameManager.I.Lose("");
            }
        }
    }

    void InputHandler()
    {
        float x = Input.GetAxisRaw("Horizontal"); // A/D
        float z = Input.GetAxisRaw("Vertical");   // W/S 

        Vector3 dir = new Vector3(x, 0f, z);

        if (dir.sqrMagnitude > 1f) dir.Normalize();

        // Move player
        transform.position += dir * moveSpeed * Time.deltaTime;

        // ===== ANIMATION =====
        bool isRunning = dir.sqrMagnitude > 0f;
        animator.SetBool("Run", isRunning);

        // ===== FLIP SPRITE LEFT/RIGHT =====
        if (x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(x) * Mathf.Abs(scale.x); // flips depending on direction
            transform.localScale = scale;
        }
    }

    public void RegisterDamage(float amount)
    {
        GameUIManager.Instance?.portraitFX?.Play();

        if (currentArmorHP > 0)
        {
            currentArmorHP = Mathf.Max(0, currentArmorHP - amount);
            GameUIManager.Instance?.UpdateArmor(currentArmorHP);
            return;
        }

        if (currentOxygen > 0)
        {
            currentOxygen = Mathf.Max(0, currentOxygen - amount);
            GameUIManager.Instance?.UpdateOxygen(currentOxygen);
            return;
        }

        currentToxin = Mathf.Min(maxToxin, currentToxin + amount);
        GameUIManager.Instance?.UpdateToxin(currentToxin);

        if(currentToxin >= maxToxin)
        {
            // Player dies
            GameManager.I.Lose("");
        }
    }

    void Start()
    {
        //Initilize
        currentArmorHP = armorMaxHP;
        currentOxygen = maxOxygen;
        currentToxin = 0f;

        GameUIManager.Instance?.InitArmor(armorMaxHP, currentArmorHP);
        GameUIManager.Instance?.InitOxygen(maxOxygen, currentOxygen);
        GameUIManager.Instance?.InitToxin(maxToxin, currentToxin);
    }

    void Update()
    {
        DrainOxygen(oxygenDrainRate * Time.deltaTime);
        InputHandler();
    }

    public void TakeDamage(int dmg)
    {
        RegisterDamage(dmg);
    }
}

