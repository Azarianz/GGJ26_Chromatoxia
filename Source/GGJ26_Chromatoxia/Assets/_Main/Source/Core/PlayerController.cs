using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // =====================
    // Player Stats
    // =====================
    public float moveSpeed = 5.0f;

    [Header("Armor")]
    public float armorMaxHP = 50;
    public float currentArmorHP { get; private set; }

    [Header("Oxygen")]
    public float maxOxygen = 100f;
    public float oxygenDrainRate = 5f; // per second
    public float currentOxygen { get; private set; }

    [Header("Toxins")]
    public float maxToxin = 100f;
    public float toxinFillRate = 10f; // per second
    public float currentToxin { get; private set; }

    // Movement
    private Rigidbody rb;
    private Vector2 movementInput;

    void DrainOxygen()
    {
        TakeOxygenDamage(oxygenDrainRate * Time.deltaTime);
    }

    void InputHandler()
    {
        // Movement / actions later
        float x = Input.GetAxisRaw("Horizontal"); // A/D, Left/Right
        float z = Input.GetAxisRaw("Vertical");   // W/S, Up/Down

        Vector3 dir = new Vector3(x, 0f, z);

        if (dir.sqrMagnitude > 1f) dir.Normalize(); // prevent faster diagonal

        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    public void TakeOxygenDamage(float amount)
    {
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
        DrainOxygen();
        InputHandler();
    }
}

