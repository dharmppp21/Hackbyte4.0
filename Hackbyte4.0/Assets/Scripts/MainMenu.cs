using System.Collections; // Required for Coroutines
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    public string firstLevelName = "ParkourLevel";

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip playSound;
    [SerializeField] private AudioClip quitSound;

    public void PlayGame()
    {
        // Don't load instantly. Start the sequence instead!
        StartCoroutine(PlaySoundAndLoad());
    }

    public void QuitGame()
    {
        StartCoroutine(PlaySoundAndQuit());
    }

    // --- COROUTINES ---

    private IEnumerator PlaySoundAndLoad()
    {
        // 1. Play the sound
        if (audioSource != null && playSound != null)
        {
            audioSource.PlayOneShot(playSound);

            // 2. Wait for the exact length of the sound clip
            yield return new WaitForSeconds(playSound.length);
        }

        // 3. Load the scene
        Debug.Log("Loading Game...");
        SceneManager.LoadScene(firstLevelName);
    }

    private IEnumerator PlaySoundAndQuit()
    {
        // 1. Play the sound
        if (audioSource != null && quitSound != null)
        {
            audioSource.PlayOneShot(quitSound);

            // 2. Wait for the exact length of the sound clip
            yield return new WaitForSeconds(quitSound.length);
        }

        // 3. Quit the game
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}