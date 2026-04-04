using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

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
        var pim = FindObjectOfType<PlayerInputManager>();
        if (pim != null) pim.enabled = false;
    }

    void Start()
    {
        GameObject mainCam = GameObject.FindWithTag("MainCamera");
        if (mainCam != null)
        {
            Camera cam = mainCam.GetComponent<Camera>();
            if (cam != null) cam.enabled = false;
            AudioListener listener = mainCam.GetComponent<AudioListener>();
            if (listener != null) listener.enabled = false;
        }

        if (playerPrefab == null) Debug.LogError("[LocalMultiplayerManager] Player Prefab is MISSING in inspector!");
        if (player1Spawn == null) Debug.LogError("[LocalMultiplayerManager] Player 1 Spawn is MISSING in inspector!");
    }

    void Update()
    {
        // Debug periodic check
        if (Time.frameCount % 300 == 0) // Log every ~5 seconds
        {
            if (!player1Joined) Debug.Log($"[Manager] Waiting for Player 1 (Space). Keyboard detected: {Keyboard.current != null}");
            if (!player2Joined) Debug.Log($"[Manager] Waiting for Player 2 (Gamepad A). Gamepad detected: {Gamepad.current != null}");
        }

        // Join Player 1: Keyboard (Space)
        if (!player1Joined && Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.anyKey.wasPressedThisFrame)
            {
                Debug.Log("[Manager] Input detected for Player 1. Spawning...");
                JoinPlayer(0, "KeyboardLeft", Keyboard.current, player1Spawn != null ? player1Spawn.position : Vector3.zero);
                player1Joined = true;
            }
        }

        // Join Player 2: Gamepad (A)
        if (!player2Joined && Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            Debug.Log("[Manager] Input detected for Player 2. Spawning...");
            JoinPlayer(1, "Gamepad", Gamepad.current, player2Spawn != null ? player2Spawn.position : Vector3.right * 2f);
            player2Joined = true;
        }
    }

    private void JoinPlayer(int playerIndex, string schemeName, InputDevice device, Vector3 spawnPosition)
    {
        if (playerPrefab == null) {
            Debug.LogError("[Manager] Cannot spawn: Prefab is null");
            return;
        }

        PlayerInput pi = PlayerInput.Instantiate(
            playerPrefab, 
            playerIndex: playerIndex, 
            controlScheme: schemeName, 
            pairWithDevice: device
        );

        if (pi != null)
        {
            if (playerIndex == 0 && Mouse.current != null)
            {
                InputUser.PerformPairingWithDevice(Mouse.current, pi.user);
            }

            pi.transform.position = spawnPosition;
            pi.transform.rotation = Quaternion.identity;

            PlayerCameraSetup camSetup = pi.GetComponent<PlayerCameraSetup>();
            if (camSetup == null) camSetup = pi.gameObject.AddComponent<PlayerCameraSetup>();
            
            camSetup.Initialize(playerIndex);
            
            Debug.Log($"[Manager] SUCCESS: Player {playerIndex} spawned at {spawnPosition}");
        }
        else
        {
            Debug.LogError($"[Manager] FAILED to instantiate Player {playerIndex} with scheme {schemeName}");
        }
    }
}
