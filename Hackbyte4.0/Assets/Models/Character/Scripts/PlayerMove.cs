using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;
    public float gravity = -20f;
    public Transform cameraTransform;

    private Animator animator;
    private CharacterController controller;
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction runAction;

    private Vector2 moveInput;
    private float verticalVelocity;

    void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        runAction = playerInput.actions["Run"];
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        bool isRunning = runAction.IsPressed();

        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        if (move.magnitude > 0.1f)
        {
            if (cameraTransform != null)
            {
                move = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f) * move;
            }

            move.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 velocity = move.normalized * currentSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        float animSpeed = 0f;
        if (moveInput.magnitude > 0.1f)
            animSpeed = isRunning ? 1f : 0.5f;

        animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);
    }
}