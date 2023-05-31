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
    public Slider rotationSlider;
    private float score = 0f;
    public float x = 0f;
    public float y = 0f;
    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Start()
    {

    }

    private void Update()
    {
        x = player2.position.y - player1.position.y;
        y = player1.position.y - player2.position.y;
        if ((y > 3) || (x > 3))
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

    private void UpdateScoreText()
    {
        tmp.text = " " + Mathf.RoundToInt(score);
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
            fadeImage.color = new Color(0f, 0f, 0f, normalizedTime);
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
