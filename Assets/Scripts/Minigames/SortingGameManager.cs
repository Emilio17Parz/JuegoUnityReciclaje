using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SortingGameManager : MonoBehaviour
{
    public enum WasteType
    {
        Recyclable,
        Organic,
        NonRecyclable
    }

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text livesText;
    public TMP_Text comboText;
    public TMP_Text feedbackText;
    public GameObject endPanel;
    public TMP_Text resultText;
    public ModalAnimator modalAnimator;

    [Header("Gameplay")]
    public float gameTime = 45f;
    public int startLives = 3;
    public int pointsPerCorrect = 100;
    public int penaltyPerWrong = 25;
    public int comboBonus = 25;

    private int score;
    private int lives;
    private float currentTime;
    private bool gameEnded;
    private int remainingWaste;
    private int comboStreak;

    private void Awake()
    {
        score = 0;
        lives = startLives;
        currentTime = gameTime;
        gameEnded = false;
        comboStreak = 0;
        remainingWaste = 0;
    }

    private void Start()
    {
        if (endPanel != null)
            endPanel.SetActive(false);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        UpdateUI();
    }

    private void Update()
    {
        if (gameEnded) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            EndGame(false);
        }

        UpdateUI();
    }

    public void RegisterSpawnedWaste(int count)
    {
        remainingWaste = count;
        Debug.Log("Residuos registrados en manager: " + remainingWaste);
    }

    public void HandleCorrectDrop(DraggableWasteItem item)
    {
        if (gameEnded) return;

        comboStreak++;
        int gained = pointsPerCorrect + ((comboStreak - 1) * comboBonus);
        score += gained;
        remainingWaste--;

        Debug.Log("Correcto. Restantes: " + remainingWaste);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMatch();

        ShowFeedback("¡Correcto! +" + gained, new Color(0.3f, 1f, 0.3f));

        UpdateUI();

        if (remainingWaste <= 0)
            EndGame(true);
    }

    public void HandleWrongDrop(DraggableWasteItem item)
    {
        if (gameEnded) return;

        comboStreak = 0;
        score = Mathf.Max(0, score - penaltyPerWrong);
        lives--;

        Debug.Log("Incorrecto. Vidas restantes: " + lives);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayFail();

        ShowFeedback("Incorrecto", new Color(1f, 0.35f, 0.35f));

        UpdateUI();

        if (lives <= 0)
        {
            lives = 0;
            EndGame(false);
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Puntos: " + score;

        if (timeText != null)
            timeText.text = "Tiempo: " + Mathf.CeilToInt(currentTime);

        if (livesText != null)
            livesText.text = "Vidas: " + lives;

        if (comboText != null)
            comboText.text = comboStreak > 1 ? "Combo x" + comboStreak : "";
    }

    private void EndGame(bool won)
    {
        gameEnded = true;

        Debug.Log("ENDGAME -> " + (won ? "GANASTE" : "PERDISTE"));

        if (endPanel != null)
            endPanel.SetActive(true);

        if (resultText != null)
        {
            resultText.text = won
                ? "¡Zona limpia!\nPuntaje: " + score
                : "Fin del juego\nPuntaje: " + score;
        }

        if (AudioManager.Instance != null && won)
            AudioManager.Instance.PlayWin();

        if (modalAnimator != null)
            modalAnimator.PlayShowAnimation();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;

        StopAllCoroutines();
        StartCoroutine(FeedbackRoutine(message, color));
    }

    private IEnumerator FeedbackRoutine(string message, Color color)
    {
        feedbackText.gameObject.SetActive(true);
        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.transform.localScale = Vector3.one * 0.8f;

        float time = 0f;
        const float duration = 0.2f;

        while (time < duration)
        {
            float t = time / duration;
            float scale = Mathf.Lerp(0.8f, 1f, t);
            feedbackText.transform.localScale = Vector3.one * scale;
            time += Time.deltaTime;
            yield return null;
        }

        feedbackText.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(0.5f);

        feedbackText.gameObject.SetActive(false);
    }
}