using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class TwoPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 1.1f;

    private PlayerInput playerInput;
    private Rigidbody rb;
    
    // Cached Input Actions
    private InputAction moveAction;
    private InputAction jumpAction;
    
    private Vector2 moveInput;
    private bool isGrounded;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // --- OPTIMIZED: Cache actions in Awake to avoid string lookups in Update ---
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
    }

    void Update()
    {
        // 1. Read Inputs generically (PlayerInput handles the scheme mapping automatically)
        moveInput = moveAction.ReadValue<Vector2>();

        // 2. Ground Check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        // 3. Jump Force (Optimized: Use events or check WasPressedThisFrame)
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // 4. Horizontal Movement
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
    }
}