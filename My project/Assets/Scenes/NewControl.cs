using UnityEngine;
using UnityEngine.InputSystem;

public class NewControl : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;

    [Header("Ground check")]
    [Tooltip("Sphere radius at feet; should roughly match capsule radius.")]
    public float groundProbeRadius = 0.4f;
    [Tooltip("Ignore ground hits for this many frames after jumping so overlap does not instantly re-ground you.")]
    public int minAirborneFramesAfterJump = 3;

    private Animator animator;
    private Rigidbody rb;
    private PlayerInput playerInput;
    private CapsuleCollider capsule;
    private InputAction moveAction;
    private InputAction runAction;
    private InputAction jumpAction;

    private Vector2 moveInput;
    private bool isGrounded = true;
    private bool wasGrounded = true;
    private int airborneIgnoreFrames;
    private bool fallingTriggerSent;

    private static readonly Collider[] OverlapBuffer = new Collider[16];

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("NewControl requires a PlayerInput component.", this);
            return;
        }

        CacheInputActions();
    }

    void OnEnable()
    {
        playerInput?.ActivateInput();
        moveAction?.Enable();
        runAction?.Enable();
        jumpAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
        runAction?.Disable();
        jumpAction?.Disable();
    }

    void CacheInputActions()
    {
        InputActionAsset asset = playerInput.actions;
        if (asset == null)
        {
            Debug.LogError("PlayerInput has no Actions asset assigned.", this);
            return;
        }

        moveAction = FindAction(asset, "Move");
        runAction = FindAction(asset, "Run", "Sprint");
        jumpAction = FindAction(asset, "Jump");

        if (moveAction == null)
            Debug.LogError("Could not find a Move action. Expected names like Player/Move or Move.", this);
        if (runAction == null)
            Debug.LogWarning("No Run or Sprint action found; run input will be ignored.", this);
        if (jumpAction == null)
            Debug.LogError("Could not find Jump action.", this);
    }

    static InputAction FindAction(InputActionAsset asset, params string[] actionNames)
    {
        string[] mapPrefixes = { "Player/", "Gameplay/", "" };

        foreach (string name in actionNames)
        {
            if (string.IsNullOrEmpty(name))
                continue;

            foreach (string prefix in mapPrefixes)
            {
                string path = prefix + name;
                InputAction found = asset.FindAction(path, throwIfNotFound: false);
                if (found != null)
                    return found;
            }
        }

        return null;
    }

    void Update()
    {
        if (moveAction != null)
            moveInput = moveAction.ReadValue<Vector2>();
        else
            moveInput = Vector2.zero;

        bool isRunning = runAction != null && runAction.IsPressed();

        float animSpeed = moveInput.magnitude > 0.1f
            ? (isRunning ? 1f : 0.5f)
            : 0f;

        if (airborneIgnoreFrames > 0)
            airborneIgnoreFrames--;

        bool groundedNow = IsGroundedFromProbe();

        if (groundedNow && !wasGrounded && AnimatorHasTrigger(animator, "Landing"))
            animator.SetTrigger("Landing");

        wasGrounded = groundedNow;
        isGrounded = groundedNow;

        if (animator != null)
        {
            animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);
            animator.SetBool("IsGrounded", isGrounded);
        }

        if (jumpAction != null && jumpAction.WasPressedThisFrame() && isGrounded && rb != null)
            Jump();

        if (rb == null)
            return;

        if (isGrounded)
            fallingTriggerSent = false;
        else if (!fallingTriggerSent && rb.linearVelocity.y < -0.1f)
        {
            if (AnimatorHasTrigger(animator, "Falling"))
                animator.SetTrigger("Falling");
            fallingTriggerSent = true;
        }
    }

    bool IsGroundedFromProbe()
    {
        if (airborneIgnoreFrames > 0)
            return false;

        if (capsule == null)
            return isGrounded;

        Bounds b = capsule.bounds;
        float r = Mathf.Max(0.05f, groundProbeRadius);
        Vector3 probeCenter = new Vector3(b.center.x, b.min.y + r, b.center.z);

        int count = Physics.OverlapSphereNonAlloc(probeCenter, r, OverlapBuffer, ~0, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < count; i++)
        {
            Collider c = OverlapBuffer[i];
            if (c == null)
                continue;
            if (c == capsule || c.transform.IsChildOf(transform))
                continue;
            if (c.CompareTag("Ground"))
                return true;
        }

        return false;
    }

    void FixedUpdate()
    {
        if (rb == null || moveAction == null)
            return;

        Vector2 m = moveAction.ReadValue<Vector2>();
        bool isRunning = runAction != null && runAction.IsPressed();
        Vector3 move = new Vector3(m.x, 0f, m.y);

        if (move.sqrMagnitude <= 0.01f)
            return;

        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 delta = move.normalized * (currentSpeed * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + delta);

        Quaternion targetRotation = Quaternion.LookRotation(move);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    }

    void Jump()
    {
        airborneIgnoreFrames = Mathf.Max(airborneIgnoreFrames, minAirborneFramesAfterJump);
        isGrounded = false;
        wasGrounded = false;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        if (animator != null)
            animator.SetTrigger("Jump");
    }

    static bool AnimatorHasTrigger(Animator anim, string name)
    {
        if (anim == null)
            return false;

        foreach (AnimatorControllerParameter p in anim.parameters)
        {
            if (p.type == AnimatorControllerParameterType.Trigger && p.name == name)
                return true;
        }

        return false;
    }
}
