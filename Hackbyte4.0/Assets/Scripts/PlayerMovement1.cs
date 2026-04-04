using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class TwoPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float rotationSpeed = 10f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 1.1f;

    [Header("Camera Setup")]
    public CinemachineFreeLook freeLookCamera;
    private Transform playerCameraTransform;

    private PlayerInput playerInput;
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Start()
    {
        // Try to find the specific camera for this player
        if (freeLookCamera == null)
        {
            freeLookCamera = GetComponentInChildren<CinemachineFreeLook>();
        }

        if (freeLookCamera != null)
        {
            // The camera for this player is usually either the child camera of the Freelook or a sibling
            // We want the transform of the actual rendering camera for this player
            Camera cam = freeLookCamera.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                playerCameraTransform = cam.transform;
            }
            else
            {
                // Fallback to the FreeLook transform itself if no camera is found
                playerCameraTransform = freeLookCamera.transform;
            }
        }
        else
        {
            // Absolute fallback to Main Camera if setup is missing
            if (Camera.main != null)
            {
                playerCameraTransform = Camera.main.transform;
            }
        }
    }

    void Update()
    {
        // 1. Read Inputs
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        bool jumpPressed = playerInput.actions["Jump"].WasPressedThisFrame();

        // 2. Check if the player is touching the ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        // 3. Apply Jump Force
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // 4. Handle Rotation (Rotate towards movement direction relative to camera)
        if (moveInput.magnitude > 0.1f)
        {
            Vector3 moveDirection = CalculateMoveDirection();
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    void FixedUpdate()
    {
        // 5. Apply Horizontal Movement relative to player's camera
        Vector3 moveDirection = CalculateMoveDirection();
        Vector3 movement = moveDirection * moveSpeed;
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
    }

    private Vector3 CalculateMoveDirection()
    {
        Vector3 cameraForward = Vector3.forward;
        Vector3 cameraRight = Vector3.right;

        if (playerCameraTransform != null)
        {
            cameraForward = playerCameraTransform.forward;
            cameraRight = playerCameraTransform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
        }

        return (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;
    }
}
