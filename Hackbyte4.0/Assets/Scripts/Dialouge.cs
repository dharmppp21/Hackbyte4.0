using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI; // Required so we can use "Image"

public class Dialouge : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI textComponent;
    public Image portraitUI; // Drag your Canvas Image here

    [Header("Dialogue Lists")]
    public string[] lines;       // List 1: Your Text
    public Sprite[] portraits;   // List 2: Your PNG Images

    public float textSpeed;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip typingSound;

    [Range(1, 5)]
    public int soundFrequency = 2;

    private int index;

    void Start()
    {
        textComponent.text = string.Empty;
        StartDialouge();
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialouge()
    {
        index = 0;
        UpdatePortrait(); // Show the first image
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        int charCount = 0;

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;

            if (c != ' ')
            {
                charCount++;
                if (charCount % soundFrequency == 0)
                {
                    if (audioSource != null && typingSound != null)
                    {
                        audioSource.PlayOneShot(typingSound);
                    }
                }
            }

            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            UpdatePortrait(); // Change the image for the next line!
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void UpdatePortrait()
    {
        // Safety check: Make sure we actually put an image in the slot!
        if (index < portraits.Length && portraits[index] != null)
        {
            portraitUI.sprite = portraits[index];
            portraitUI.color = Color.white; // Make the image visible
        }
        else
        {
            portraitUI.color = Color.clear; // Make the box invisible if there is no image
        }
    }
}