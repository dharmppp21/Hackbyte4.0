using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Crucial: Allows us to talk to Canvas Images

public class ImageSequencePlayer : MonoBehaviour
{
    [Header("UI Setup")]
    public Image displayBox; // Drag your Canvas Image here

    [Header("Animation Settings")]
    public Sprite[] storyFrames; // Your list of PNGs

    [Tooltip("How many images to show per second. 3 FPS = 1/3 second per image.")]
    public float framesPerSecond = 3f;

    [Tooltip("Check this if you want the animation to repeat forever.")]
    public bool loopAnimation = false;

    private void Start()
    {
        // Safety check to make sure we didn't forget to assign things in the Inspector
        if (displayBox == null || storyFrames.Length == 0)
        {
            Debug.LogWarning("ImageSequencePlayer is missing the display box or the image frames!");
            return;
        }

        // Start the flipbook animation!
        StartCoroutine(PlayFlipbook());
    }

    private IEnumerator PlayFlipbook()
    {
        // Calculate the exact wait time (e.g., 1 / 3 = 0.333 seconds)
        float waitTime = 1f / framesPerSecond;

        // The outer loop handles repeating the animation if 'loopAnimation' is true
        do
        {
            // The inner loop goes through your list of images one by one
            for (int i = 0; i < storyFrames.Length; i++)
            {
                // Swap the image
                displayBox.sprite = storyFrames[i];

                // Wait for a fraction of a second before continuing
                yield return new WaitForSeconds(waitTime);
            }

        } while (loopAnimation); // If true, it goes back to the top and starts at image 0 again
    }
}