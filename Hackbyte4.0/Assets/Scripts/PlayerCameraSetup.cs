using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCameraSetup : MonoBehaviour
{
    [Header("Camera Settings")]
    public CinemachineFreeLook freeLookCamera;
    public Camera playerCamera;

    private PlayerInput playerInput;
    private InputAction lookAction;
    private bool initialized = false;

    public void Initialize(int playerIndex)
    {
        if (initialized) return;
        
        playerInput = GetComponent<PlayerInput>();
        lookAction = playerInput.actions["Look"];

        // 1. Find or Setup the Rendering Camera
        if (playerCamera == null)
        {
            // Look for a camera already in children (common in Cinemachine setups)
            playerCamera = GetComponentInChildren<Camera>();
        }

        if (playerCamera == null)
        {
            // If still no camera, create a new one
            GameObject camObj = new GameObject("PlayerCamera_" + playerIndex);
            camObj.transform.SetParent(transform);
            camObj.transform.localPosition = new Vector3(0, 2, -5);
            playerCamera = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
        }

        // 2. Setup Cinemachine Brain
        var brain = playerCamera.GetComponent<CinemachineBrain>();
        if (brain == null) brain = playerCamera.gameObject.AddComponent<CinemachineBrain>();

        // 3. Find or Setup the FreeLook Virtual Camera
        if (freeLookCamera == null)
        {
            freeLookCamera = GetComponentInChildren<CinemachineFreeLook>();
        }

        if (freeLookCamera != null)
        {
            freeLookCamera.Follow = transform;
            freeLookCamera.LookAt = transform;

            // Important: Each player's Brain should only react to their own Virtual Camera
            // Usually Cinemachine handles this via layers or priorities, but for split screen,
            // having the Virtual Camera as a child of the player and the Brain on the player's camera is best.
            
            // Disable default input
            freeLookCamera.m_XAxis.m_InputAxisName = "";
            freeLookCamera.m_YAxis.m_InputAxisName = "";
        }

        // 4. CONFIGURE SPLIT SCREEN VIEWPORT (Vertical: Left/Right)
        if (playerIndex == 0)
        {
            playerCamera.rect = new Rect(0, 0, 0.5f, 1); // LEFT side
        }
        else
        {
            playerCamera.rect = new Rect(0.5f, 0, 0.5f, 1); // RIGHT side
            
            // Disable redundant AudioListeners
            var audioListener = playerCamera.GetComponent<AudioListener>();
            if (audioListener != null) audioListener.enabled = false;
        }

        playerCamera.depth = playerIndex + 1; // Ensure cameras are rendered above existing ones
        initialized = true;

        Debug.Log($"PlayerCameraSetup: Player {playerIndex} camera initialized on {playerCamera.name}");
    }

    void Start()
    {
        // Fallback auto-init if not called by Manager
        if (!initialized && GetComponent<PlayerInput>() != null)
        {
            Initialize(GetComponent<PlayerInput>().playerIndex);
        }
    }

    void Update()
    {
        if (freeLookCamera != null && lookAction != null)
        {
            Vector2 input = lookAction.ReadValue<Vector2>();
            
            float sensitivityX = (playerInput.currentControlScheme == "Gamepad") ? 300f : 1f;
            float sensitivityY = (playerInput.currentControlScheme == "Gamepad") ? 5f : 0.02f;

            freeLookCamera.m_XAxis.m_InputAxisValue = input.x * sensitivityX * Time.deltaTime;
            freeLookCamera.m_YAxis.m_InputAxisValue = input.y * sensitivityY * Time.deltaTime;
        }
    }
}
