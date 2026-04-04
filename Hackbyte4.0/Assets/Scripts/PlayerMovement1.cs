using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))] // Automatically adds a Rigidbody for jump physics
public class TwoPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f; // How high the player jumps

    [Header("Ground Check")]
    public LayerMask groundLayer; // Select your "Ground" layer in the inspector
    public float groundCheckDistance = 1.1f; // Adjust based on your player's height

    private PlayerInput playerInput;
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        // Prevent the player from falling over
        rb.freezeRotation = true;
    }

    void Update()
    {
        bool jumpPressed = false;

        // 1. Read Inputs based on Control Scheme
        if (playerInput.currentControlScheme == "KeyboardLeft")
        {
            moveInput = playerInput.actions["Move_P1"].ReadValue<Vector2>();
            // Check if P1 pressed jump this frame
            jumpPressed = playerInput.actions["Jump_P1"].WasPressedThisFrame();
        }
        else if (playerInput.currentControlScheme == "KeyboardRight")
        {
            moveInput = playerInput.actions["Move_P2"].ReadValue<Vector2>();
            // Check if P2 pressed jump this frame
            jumpPressed = playerInput.actions["Jump_P2"].WasPressedThisFrame();
        }

        // 2. Check if the player is touching the ground (prevents infinite mid-air jumps)
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        // 3. Apply Jump Force
        if (jumpPressed && isGrounded)
        {
            // Reset Y velocity before jumping so downward momentum doesn't cancel the jump
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // 4. Apply Horizontal Movement using Rigidbody Physics
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;

        // We apply the new X/Z movement, but keep the current Y velocity (gravity/jumping)
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
    }
}