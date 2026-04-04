using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;

    private Animator animator;
    private Rigidbody rb;
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction runAction;

    private Vector2 moveInput;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
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
            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            transform.Translate(move.normalized * currentSpeed * Time.deltaTime, Space.World);

            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        float animSpeed = 0f;
        if (moveInput.magnitude > 0.1f)
            animSpeed = isRunning ? 1f : 0.5f;

        animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);
    }
}