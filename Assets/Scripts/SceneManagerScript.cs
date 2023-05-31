using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneManagerScript : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    public float scoreIncreaseRate = 1f;
    public TextMeshProUGUI tmp;
 
    private float score = 0f;
    public float x =0f;
    public float y = -5;
    private void Start()
    {
        x = player1.transform.position.y;
    }

    private void Update()
    {
  
            x = player1.transform.position.y-3;
            

        if (player1.position.y < x || player2.position.y < x)
        {
            LoseGame();
        }

        IncreaseScore();
        UpdateScoreText();

    }

    private void IncreaseScore()
    {
        score += scoreIncreaseRate * Time.deltaTime;
    }

    private void UpdateScoreText()
    {
        tmp.text = " " + Mathf.RoundToInt(score);
    }

    private void LoseGame()
    {
        Invoke("ReloadScene", 3f);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
