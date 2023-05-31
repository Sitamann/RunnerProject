using UnityEngine;
using TMPro;
using System.IO;

public class TextChanger : MonoBehaviour
{
    public string filePath; // Path to the text file containing equations for size increase
    public string filePath2; // Path to the text file containing equations for size decrease
    public TextMeshProUGUI textMeshPro; // TextMeshProUGUI component to set the text
    public int signe;
    public GameObject player1; // Reference to Player 1
    public GameObject player2; // Reference to Player 2
    private void Start()
    {
        if (textMeshPro != null)
        {
            int signe = Random.Range(1, 3); // Randomly determine the signe value (1 or 2)

            if (signe == 1)
            {
                // Load the text file with equations for size increase
                string[] lines = File.ReadAllLines(filePath);

                // Choose a random line from the file
                int randomIndex = Random.Range(0, lines.Length);
                string chosenLine = lines[randomIndex];

                // Set the chosen line to the TextMeshProUGUI component
                textMeshPro.text = chosenLine;

                gameObject.tag = "add";

            }
            else if (signe == 2)
            {
                // Load the text file with equations for size decrease
                string[] lines = File.ReadAllLines(filePath2);

                // Choose a random line from the file
                int randomIndex = Random.Range(0, lines.Length);
                string chosenLine = lines[randomIndex];

                // Set the chosen line to the TextMeshProUGUI component
                textMeshPro.text = chosenLine;

                // Decrease player 2's size
                gameObject.tag = "minus";

            }
        }
    }
   
   
}