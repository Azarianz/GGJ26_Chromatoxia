using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // =====================
    // Player Stats
    // =====================
    int currentHP;
    int maxHP = 100;

    int currentArmorHP;
    int currentArmorMaxHP = 50;

    public float moveSpeed = 5.0f;

    // =====================
    // Oxygen / Toxins
    // =====================
    [Header("Oxygen")]
    public Slider oxygenSlider;

    public float maxOxygen = 100f;
    public float oxygenDrainRate = 5f; // per second
    float currentOxygen;

    [Header("Toxins")]
    public float maxToxin = 100f;
    public float toxinFillRate = 10f; // per second
    float currentToxin;

    // Movement
    private Rigidbody2D rb;
    private Vector2 movementInput;

    void Start()
    {
        void Start()
        {
            currentOxygen = maxOxygen;
            currentToxin = 0f;

            if (oxygenSlider != null)
            {
                oxygenSlider.maxValue = maxOxygen;
                oxygenSlider.value = currentOxygen;
            }
        }

        void Update()
        {
            UpdateOxygen();
            InputHandler();
        }

        void UpdateOxygen()
        {
            if (currentOxygen > 0f)
            {
                currentOxygen -= oxygenDrainRate * Time.deltaTime;
                currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);
            }
            else
            {
                // Oxygen empty ? toxins fill
                currentToxin += toxinFillRate * Time.deltaTime;
                currentToxin = Mathf.Clamp(currentToxin, 0f, maxToxin);
            }

            if (oxygenSlider != null)
                oxygenSlider.value = currentOxygen;
        }
    }

    void Update()
    {
        UpdateOxygen();
        InputHandler();
    }

    void UpdateOxygen()
    {
        if (currentOxygen > 0f)
        {
            currentOxygen -= oxygenDrainRate * Time.deltaTime;
            currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);
        }
        else
        {   
            // Oxygen empty ? toxins fill
            currentToxin += toxinFillRate * Time.deltaTime;
            currentToxin = Mathf.Clamp(currentToxin, 0f, maxToxin);
        }

        if (oxygenSlider != null)
            oxygenSlider.value = currentOxygen;
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
}

