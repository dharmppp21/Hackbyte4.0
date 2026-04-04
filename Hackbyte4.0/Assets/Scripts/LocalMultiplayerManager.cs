using UnityEngine;
using UnityEngine.InputSystem;

public class LocalMultiplayerManager : MonoBehaviour
{
    [Header("Player Setup")]
    public GameObject playerPrefab;

    [Header("Spawn Points")]
    public Transform player1Spawn;
    public Transform player2Spawn;

    private bool player1Joined = false;
    private bool player2Joined = false;

    void Awake()
    {
        // Disable PlayerInputManager if it's on the same object or in the scene
        // because we are manually joining players to avoid double-spawning.
        var pim = FindObjectOfType<PlayerInputManager>();
        if (pim != null)
        {
            pim.enabled = false;
            Debug.Log("LocalMultiplayerManager: Disabled PlayerInputManager to prevent double-spawning.");
        }
    }

    void Start()
    {
        // Hide/Disable the Main Camera so only split-screen cameras show
        GameObject mainCam = GameObject.FindWithTag("MainCamera");
        if (mainCam != null)
        {
            Camera cam = mainCam.GetComponent<Camera>();
            if (cam != null) cam.enabled = false;
            
            // Also disable AudioListener to avoid dual listener warnings
            AudioListener listener = mainCam.GetComponent<AudioListener>();
            if (listener != null) listener.enabled = false;
            
            Debug.Log("LocalMultiplayerManager: Main Camera disabled.");
        }
    }

    void Update()
    {
        // 1. Join Player 1 (KeyboardLeft) with Space
        if (!player1Joined && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            JoinPlayer(0, "KeyboardLeft", Keyboard.current, player1Spawn.position);
            player1Joined = true;
        }

        // 2. Join Player 2 (Gamepad) with Gamepad A/South button
        if (!player2Joined && Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            JoinPlayer(1, "Gamepad", Gamepad.current, player2Spawn.position);
            player2Joined = true;
        }
    }

    private void JoinPlayer(int playerIndex, string schemeName, InputDevice device, Vector3 spawnPosition)
    {
        // Pair device manually
        PlayerInput pi = PlayerInput.Instantiate(
            playerPrefab, 
            playerIndex: playerIndex, 
            controlScheme: schemeName, 
            pairWithDevice: device
        );

        if (pi != null)
        {
            pi.transform.position = spawnPosition;
            pi.transform.rotation = Quaternion.identity;

            // Initialize Camera Setup
            PlayerCameraSetup camSetup = pi.GetComponent<PlayerCameraSetup>();
            if (camSetup == null) camSetup = pi.gameObject.AddComponent<PlayerCameraSetup>();
            
            camSetup.Initialize(playerIndex);
            
            Debug.Log($"LocalMultiplayerManager: Player {playerIndex} joined with scheme {schemeName}");
        }
    }
}
