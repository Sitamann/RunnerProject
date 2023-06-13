using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerScript : MonoBehaviour
{
    public static SceneManagerScript instance;
    public Transform player1;
    public Transform player2;
    public float scoreIncreaseRate = 1f;
    public TextMeshProUGUI tmp;
    public Slider rotationSlider;
    public float score = 0f;
    public float x = 0f;
    public float y = 0f;
    public Image fadeImage;
    public float fadeDuration = 1f;
    public int counter = 1000;
    public int hey;
    public Vector2 range;
    private void Start()
    {
        instance = this;
        range = new Vector2(1, 3);
    }

    private void Update()
    {
        x = player2.position.y - player1.position.y;
        y = player1.position.y - player2.position.y;
        if ((y > 4) || (x > 4))
        {
            LoseGame();
        }
        else
        {
            IncreaseScore();
            UpdateScoreText();
            UpdateRotationSlider();

        }
    }

    private void UpdateRotationSlider()
    {
        rotationSlider.value = (x - y) * 1.5f;
    }

    private void IncreaseScore()
    {
        score += scoreIncreaseRate * Time.deltaTime * 8;
    }
    // random.range(1,3)
    // random.range(3,5)
    // random.range(5,7)
    private void UpdateScoreText()
    {
        tmp.text = "" + Mathf.RoundToInt(score);
        if (score >= counter)
        {
            if (range.y < 7)
            {
                range.x = range.x + 2;
                range.y = range.y + 2;
                counter += 1000;
            }
        }



    }

    private void LoseGame()
    {
        StartCoroutine(ReloadSceneWithFade());
    }

    private System.Collections.IEnumerator ReloadSceneWithFade()
    {
        // Fade out
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float normalizedTime = t / fadeDuration;
            fadeImage.color = new Color(1f, 1f, 1f, normalizedTime);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        /* Fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float normalizedTime = t / fadeDuration;
            fadeImage.color = new Color(0f, 0f, 0f, 1f - normalizedTime);
            yield return null;
        }*/
    }
}
