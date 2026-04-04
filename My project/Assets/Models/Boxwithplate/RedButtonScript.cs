using UnityEngine;

public class RedButtonSpawn : MonoBehaviour
{
    public GameObject boxPrefab;
    public Transform spawnPoint;
    public GameObject interactText;

    private bool hasSpawned = false;

    private void Start()
    {
        if (interactText != null)
            interactText.SetActive(false);
    }

    public void Interact()
    {
        if (hasSpawned) return;

        Instantiate(boxPrefab, spawnPoint.position, spawnPoint.rotation);
        hasSpawned = true;

        if (interactText != null)
            interactText.SetActive(false);
    }

    public void ShowPrompt()
    {
        if (interactText != null && !hasSpawned)
            interactText.SetActive(true);
    }

    public void HidePrompt()
    {
        if (interactText != null)
            interactText.SetActive(false);
    }
}