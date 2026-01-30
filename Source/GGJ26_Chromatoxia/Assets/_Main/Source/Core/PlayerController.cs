using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player Stats
    int currentArmorHP, currentArmorMaxHP = 50;
    public float moveSpeed = 5.0f;

    // Player Oxygen
    public float currentOxygen, maxOxygen;
    float currentToxin, maxToxin;

    // Movement
    private Rigidbody2D rb;
    private Vector2 movementInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Init stats

        currentArmorHP = currentArmorMaxHP;
        currentOxygen = maxOxygen;
        currentToxin = 0;
    }

    void Update()
    {
        InputHandler();
    }

    void FixedUpdate()
    {
        Move();
    }

    void InputHandler()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        movementInput = movementInput.normalized;
    }

    void Move()
    {
        rb.velocity = movementInput * moveSpeed;
    }
}

