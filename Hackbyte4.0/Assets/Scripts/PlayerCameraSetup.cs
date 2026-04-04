using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCameraSetup : MonoBehaviour
{
    [Header("Camera Settings")]
    public CinemachineFreeLook freeLookCamera;
    public Camera playerCamera;

    [Header("Sensitivity")]
    public float mouseSensitivityX = 15f;
    public float mouseSensitivityY = 0.1f;
    public float gamepadSensitivityX = 300f;
    public float gamepadSensitivityY = 2f;

    private PlayerInput playerInput;
    private InputAction lookAction;
    private bool initialized = false;

    public void Initialize(int playerIndex)
    {
        if (initialized) return;
        
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null) return;

        // Ensure actions are ready
        if (playerInput.actions == null)
        {
            Debug.LogError($"[PlayerCameraSetup] PlayerInput on {gameObject.name} has no Input Action Asset!");
            return;
        }

        lookAction = playerInput.actions.FindAction("Look");
        if (lookAction == null)
        {
            Debug.LogWarning($"[PlayerCameraSetup] 'Look' action not found for Player {playerIndex}");
        }

        // 1. Setup Layers for Culling
        int playerLayer = (playerIndex == 0) ? 6 : 7;
        int otherPlayerLayer = (playerIndex == 0) ? 7 : 6;
        
        SetLayerRecursively(gameObject, playerLayer);

        // 2. Setup the Rendering Camera
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
        
        if (playerCamera != null)
        {
            playerCamera.enabled = true;
            
            // Culling Mask: Hide other player
            playerCamera.cullingMask &= ~(1 << otherPlayerLayer);
            playerCamera.cullingMask |= (1 << playerLayer);

            // Set Split Screen Viewport (Vertical: Left/Right)
            if (playerIndex == 0)
            {
                playerCamera.rect = new Rect(0, 0, 0.5f, 1);
                playerCamera.depth = 10;
            }
            else
            {
                playerCamera.rect = new Rect(0.5f, 0, 0.5f, 1);
                playerCamera.depth = 11;
                
                var al = playerCamera.GetComponent<AudioListener>();
                if (al != null) al.enabled = false;
            }

            var brain = playerCamera.GetComponent<CinemachineBrain>();
            if (brain == null) brain = playerCamera.gameObject.AddComponent<CinemachineBrain>();
        }

        // 3. Setup the FreeLook Virtual Camera
        if (freeLookCamera == null) freeLookCamera = GetComponentInChildren<CinemachineFreeLook>();

        if (freeLookCamera != null)
        {
            freeLookCamera.Follow = this.transform;
            freeLookCamera.LookAt = this.transform;
            freeLookCamera.m_XAxis.m_InputAxisName = "";
            freeLookCamera.m_YAxis.m_InputAxisName = "";
            freeLookCamera.gameObject.layer = playerLayer;
        }

        initialized = true;
        Debug.Log($"[PlayerCameraSetup] Player {playerIndex} initialized.");
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj.GetComponent<Camera>() != null) return;
        
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    void Start()
    {
        if (!initialized && GetComponent<PlayerInput>() != null)
            Initialize(GetComponent<PlayerInput>().playerIndex);
    }

    void Update()
    {
        if (freeLookCamera != null && lookAction != null)
        {
            Vector2 input = lookAction.ReadValue<Vector2>();
            
            bool isGamepad = playerInput.currentControlScheme == "Gamepad";
            float xSens = isGamepad ? gamepadSensitivityX : mouseSensitivityX;
            float ySens = isGamepad ? gamepadSensitivityY : mouseSensitivityY;

            freeLookCamera.m_XAxis.m_InputAxisValue = input.x * xSens;
            freeLookCamera.m_YAxis.m_InputAxisValue = input.y * ySens;
        }
    }
}
