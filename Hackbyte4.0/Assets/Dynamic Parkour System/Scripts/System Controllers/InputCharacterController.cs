using UnityEngine;
using UnityEngine.InputSystem;

namespace Climbing
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputCharacterController : MonoBehaviour
    {
        [Header("Input Action Names")]
        [Tooltip("The names of the actions in your Input Action Asset.")]
        public string moveActionName = "Move";
        public string jumpActionName = "Jump";
        public string dropActionName = "Drop";
        public string runActionName = "Run";

        [HideInInspector] public Vector2 movement;
        [HideInInspector] public bool run;
        [HideInInspector] public bool jump;
        [HideInInspector] public bool drop;

        private PlayerInput playerInput;

        // Cached actions
        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction dropAction;
        private InputAction runAction;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();

            if (playerInput.actions == null)
            {
                Debug.LogError($"[Input Error] No Input Action Asset assigned to the PlayerInput on {gameObject.name}!");
                return;
            }

            // Look for the specific names you typed in the Inspector
            moveAction = playerInput.actions.FindAction(moveActionName);
            jumpAction = playerInput.actions.FindAction(jumpActionName);
            dropAction = playerInput.actions.FindAction(dropActionName);
            runAction = playerInput.actions.FindAction(runActionName);

            // Print helpful warnings if a name is misspelled in the Inspector
            if (moveAction == null) Debug.LogError($"[Input Error] Cannot find action named '{moveActionName}'. Make sure it exists in your Input Action Asset.");
        }

        private void OnEnable()
        {
            if (moveAction != null) moveAction.Enable();
            if (jumpAction != null)
            {
                jumpAction.Enable();
                jumpAction.performed += ctx => jump = true;
                jumpAction.canceled += ctx => jump = false;
            }
            if (dropAction != null)
            {
                dropAction.Enable();
                dropAction.performed += ctx => drop = true;
                dropAction.canceled += ctx => drop = false;
            }
            if (runAction != null)
            {
                runAction.Enable();
                runAction.performed += ctx => run = true;
                runAction.canceled += ctx => run = false;
            }
        }

        private void OnDisable()
        {
            if (moveAction != null) moveAction.Disable();
            if (jumpAction != null) jumpAction.Disable();
            if (dropAction != null) dropAction.Disable();
            if (runAction != null) runAction.Disable();
        }

        private void Update()
        {
            if (moveAction != null)
                movement = moveAction.ReadValue<Vector2>();
        }

        // The Parkour System requires this method to exist
        public void ToggleRun()
        {
            if (movement.magnitude > 0.2f && !run)
                run = true;
            else if (movement.magnitude <= 0.2f)
                run = false;
        }
    }
}