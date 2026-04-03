using UnityEngine;
using UnityEngine.InputSystem;

public class LocalMultiplayerManager : MonoBehaviour
{
    [Header("Player Setup")]
    [Tooltip("The prefab must have a PlayerInput component attached.")]
    public GameObject playerPrefab;

    [Header("Spawn Points")]
    public Transform player1Spawn;
    public Transform player2Spawn;

    // State tracking
    private bool player1Joined = false;
    private bool player2Joined = false;

    void Update()
    {
        // Safety check: Ensure a keyboard is actually connected
        if (Keyboard.current == null)
        {
            Debug.LogWarning("LocalMultiplayerManager: No keyboard detected!");
            return;
        }

        // --- DEBUG: FORCE SPAWN (F1 Key) ---
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            Debug.Log("F1 pressed - Forcing Player 1 Spawn...");
            JoinPlayer(0, "KeyboardLeft", player1Spawn.position);
            player1Joined = true;
        }

        // --- PLAYER 1 JOIN LOGIC (Space Key) ---
        if (!player1Joined && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Space pressed - Joining Player 1...");
            JoinPlayer(0, "KeyboardLeft", player1Spawn.position);
            player1Joined = true;
            Debug.Log("Player 1 Joined! (WASD)");
        }

        // --- PLAYER 2 JOIN LOGIC (Enter Key) ---
        if (!player2Joined && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
        {
            Debug.Log("Enter pressed - Joining Player 2...");
            JoinPlayer(1, "KeyboardRight", player2Spawn.position);
            player2Joined = true;
            Debug.Log("Player 2 Joined! (Arrows)");
        }
    }

    private void JoinPlayer(int playerIndex, string schemeName, Vector3 spawnPosition)
    {
        Debug.Log($"Attempting to Join Player {playerIndex} with scheme {schemeName} at {spawnPosition}");
        
        if (playerPrefab == null)
        {
            Debug.LogError("LocalMultiplayerManager: Player Prefab is missing!");
            return;
        }

        // This is the core API call that makes device sharing work.
        // It spawns the prefab, assigns the specific scheme, and pairs it to the shared keyboard.
        PlayerInput newPlayer = PlayerInput.Instantiate(
            playerPrefab,
            playerIndex: playerIndex,
            controlScheme: schemeName,
            pairWithDevice: Keyboard.current
        );

        if (newPlayer == null)
        {
            Debug.LogError("LocalMultiplayerManager: Failed to instantiate player!");
            return;
        }

        // Move the newly spawned player to their starting position
        newPlayer.transform.position = spawnPosition;
        Debug.Log($"Player {playerIndex} spawned at {newPlayer.transform.position}");

        // --- AUTOMATIC CAMERA SETUP ---
        // Ensure each player has their own camera for split-screen
        Camera playerCamera = newPlayer.GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            GameObject camObj = new GameObject("Player_" + (playerIndex + 1) + "_Camera");
            camObj.transform.SetParent(newPlayer.transform);
            
            playerCamera = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>(); // Add listener, SplitScreenCamera will handle P2's ears
            camObj.AddComponent<SplitScreenCamera>();
            
            Debug.Log("Created new Camera for Player " + (playerIndex + 1));
        }
        else
        {
            // If prefab already has a camera, ensure it has the SplitScreenCamera script
            if (playerCamera.GetComponent<SplitScreenCamera>() == null)
            {
                playerCamera.gameObject.AddComponent<SplitScreenCamera>();
            }
        }
    }
}