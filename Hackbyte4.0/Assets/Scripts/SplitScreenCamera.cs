using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class SplitScreenCamera : MonoBehaviour
{
    [Header("Camera Positioning")]
    [Tooltip("How far above and behind the player the camera should sit")]
    public Vector3 positionOffset = new Vector3(0f, 2.2f, -3.5f); // Brought closer (was 5f, -7f)

    [Tooltip("The angle the camera looks down at the player")]
    public Vector3 rotationOffset = new Vector3(15f, 0f, 0f); // Slightly shallower angle for lower height

    private Camera cam;
    private int index;
    private Transform playerTransform; // Reference to the parent player

    void Start()
    {
        cam = GetComponent<Camera>();

        // Grab the player index from the parent object
        PlayerInput playerInput = GetComponentInParent<PlayerInput>();
        if (playerInput != null)
        {
            index = playerInput.playerIndex;
            playerTransform = playerInput.transform;
        }
        else
        {
            Debug.LogError("SplitScreenCamera: No PlayerInput found in parent! Make sure the camera is a child of the Player Prefab.");
            return;
        }

        cam.depth = index;

        // --- THE AUDIO LISTENER FIX ---
        // If this camera does NOT belong to Player 1 (index 0), destroy its ears.
        if (index != 0)
        {
            AudioListener listener = GetComponent<AudioListener>();
            if (listener != null)
            {
                Destroy(listener); // Deletes the component so only P1 can hear
            }
        }

        UpdateAllCameras();
    }

    // LateUpdate runs AFTER the player movement script finishes
    void LateUpdate()
    {
        if (playerTransform != null)
        {
            // 1. Force the camera to stay exactly at the offset distance from the player
            // By using absolute position, we ignore if the player capsule is spinning
            transform.position = playerTransform.position + positionOffset;

            // 2. Lock the camera's rotation to always face the same angle
            transform.rotation = Quaternion.Euler(rotationOffset);
        }
    }

    private void UpdateAllCameras()
    {
        int totalPlayers = PlayerInput.all.Count;
        SplitScreenCamera[] allCameras = FindObjectsByType<SplitScreenCamera>(FindObjectsInactive.Exclude);

        foreach (var splitCam in allCameras)
        {
            splitCam.RecalculateRect(totalPlayers);
        }
    }

    public void RecalculateRect(int totalPlayers)
    {
        if (cam == null) cam = GetComponent<Camera>();

        if (totalPlayers <= 1)
        {
            cam.rect = new Rect(0, 0, 1, 1);
        }
        else if (totalPlayers == 2)
        {
            // Split screen vertically (left/right)
            cam.rect = new Rect(index == 0 ? 0 : 0.5f, 0, 0.5f, 1);
        }
        else
        {
            // Grid for 3+ players
            cam.rect = new Rect((index % 2) * 0.5f, (index < 2) ? 0.5f : 0f, 0.5f, 0.5f);
        }
    }
}