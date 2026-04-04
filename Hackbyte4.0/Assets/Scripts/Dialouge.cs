using UnityEngine;
using TMPro;

public class Dialouge : MonoBehaviour
{
    public TextMeshProUGUI textComponent; // Reference to the TextMeshProUGUI component to display the dialouge text
    public string[] lines; // Array of strings to hold the dialouge lines
    public float textSpeed; // Speed at which the text is displayed

    private int index; // Index to keep track of the current line being displayed

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textComponent text = string.Empty; // Clear the text component's text at the start of the game
        StartDialouge(); // Call the method to start the dialouge sequence
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartDialouge()
    {
        index = 0; // Initialize the index to start from the first line
        StartCoroutine(TypeLine()); // Start the coroutine to display the first line of dialouge
    }

    int IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray()) // Loop through each character in the current line of dialouge
        {
            textComponent.text += c; // Append the character to the text component's text
            yield return new WaitForSeconds(textSpeed); // Wait for a specified time before displaying the next character
        }
    }
}
