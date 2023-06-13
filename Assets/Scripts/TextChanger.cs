using UnityEngine;
using TMPro;
using System.IO;

public class TextChanger : MonoBehaviour
{
    public TextAsset filePath; // Path to the text file containing equations for size increase
    public TextAsset filePath2; // Path to the text file containing equations for size decrease
                             //mediumLevel
    public TextAsset filePath3; // Path to the text file containing equations for size increase
    public TextAsset filePath4; // Path to the text file containing equations for size decrease
                             //HardLevel
    public TextAsset filePath5; // Path to the text file containing equations for size increase
    public TextAsset filePath6; // Path to the text file containing equations for size decrease


    public TextMeshProUGUI textMeshPro; // TextMeshProUGUI component to set the text
    public int signe;
    public GameObject player1; // Reference to Player 1
    public GameObject player2; // Reference to Player 2

    public float scoreIncreaseRate = 1f;
    private void Start()
    {


        Generator();
    }
    public void Generator()
    {
        signe = (int)Random.Range(SceneManagerScript.instance.range.x, SceneManagerScript.instance.range.y);
        if (textMeshPro != null)
        {
            // Randomly determine the signe value (1 or 2)
            Debug.Log(signe);
            if (signe == 1)
            {
                // Load the text file with equations for size increase
                string[] lines = filePath.text.Split('\n');
                
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
                string[] lines = filePath2.text.Split('\n');

                // Choose a random line from the file
                int randomIndex = Random.Range(0, lines.Length);
                string chosenLine = lines[randomIndex];

                // Set the chosen line to the TextMeshProUGUI component
                textMeshPro.text = chosenLine;

                // Decrease player 2's size
                gameObject.tag = "minus";

            }

            else if (signe == 3)
            {
                // Load the text file with equations for size increase
                string[] lines = filePath3.text.Split('\n');

                // Choose a random line from the file
                int randomIndex = Random.Range(0, lines.Length);
                string chosenLine = lines[randomIndex];

                // Set the chosen line to the TextMeshProUGUI component
                textMeshPro.text = chosenLine;

                gameObject.tag = "add";

            }
            else if (signe == 4)
            {
                // Load the text file with equations for size decrease
                string[] lines = filePath4.text.Split('\n');

                // Choose a random line from the file
                int randomIndex = Random.Range(0, lines.Length);
                string chosenLine = lines[randomIndex];

                // Set the chosen line to the TextMeshProUGUI component
                textMeshPro.text = chosenLine;

                // Decrease player 2's size
                gameObject.tag = "minus";

            }
            else if (signe == 5)
            {
                // Load the text file with equations for size increase
                string[] lines = filePath5.text.Split('\n');

                // Choose a random line from the file
                int randomIndex = Random.Range(0, lines.Length);
                string chosenLine = lines[randomIndex];

                // Set the chosen line to the TextMeshProUGUI component
                textMeshPro.text = chosenLine;

                gameObject.tag = "add";

            }
            else if (signe == 6)
            {
                // Load the text file with equations for size decrease
                string[] lines = filePath6.text.Split('\n');

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