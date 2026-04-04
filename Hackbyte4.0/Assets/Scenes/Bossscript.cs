using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
public class Bossscript : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;
    public float gravity = -20f;

    [Header("Dodge")]
    public float dodgeSpeed = 8f;
    public float dodgeDuration = 0.25f;
    public float dodgeCooldown = 0.5f;

    private Animator animator;
    private CharacterController controller;
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction runAction;
    private InputAction dodgeLeftAction;
    private InputAction dodgeRightAction;

    private Vector2 moveInput;
    private float verticalVelocity;

    private bool isDodging;
    private float dodgeTimer;
    private float lastDodgeTime;
    private Vector3 dodgeDirection;

    void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        runAction = playerInput.actions["Run"];
        dodgeLeftAction = playerInput.actions["DodgeLeft"];
        dodgeRightAction = playerInput.actions["DodgeRight"];
    }

    void Update()
    {
        if (!isDodging)
        {
            HandleMovement();
            HandleDodgeInput();
        }
        else
        {
            HandleDodgeMovement();
        }
    }

    void HandleMovement()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        bool isRunning = runAction.IsPressed();

        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        if (move.magnitude > 0.1f)
        {
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
        animator.SetBool("IsDodging", false);
    }

    void HandleDodgeInput()
    {
        if (Time.time < lastDodgeTime + dodgeCooldown)
            return;

        if (dodgeLeftAction.triggered)
        {
            StartDodge(-transform.right, true);
        }
        else if (dodgeRightAction.triggered)
        {
            StartDodge(transform.right, false);
        }
    }

    void StartDodge(Vector3 direction, bool left)
    {
        isDodging = true;
        dodgeTimer = dodgeDuration;
        lastDodgeTime = Time.time;
        dodgeDirection = direction.normalized;

        animator.SetBool("IsDodging", true);

        if (left)
            animator.SetTrigger("DodgeLeft");
        else
            animator.SetTrigger("DodgeRight");
    }

    void HandleDodgeMovement()
    {
        dodgeTimer -= Time.deltaTime;

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = dodgeDirection * dodgeSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        if (dodgeTimer <= 0f)
        {
            isDodging = false;
            animator.SetBool("IsDodging", false);
        }
    }
}