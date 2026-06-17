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
    public GameObject endPanel;
    public TMP_Text resultText;
    public ModalAnimator modalAnimator;
    public FeedbackAnimator feedbackAnimator;

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
            return;
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

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMatch();

        if (feedbackAnimator != null)
        {
            feedbackAnimator.ShowFeedback(GetPositiveMessage() + " +" + gained, new Color(0.3f, 1f, 0.3f), 0.8f);
            feedbackAnimator.ShowCombo(comboStreak);
        }

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

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayFail();

        if (feedbackAnimator != null)
        {
            feedbackAnimator.ShowFeedback(GetNegativeMessage(), new Color(1f, 0.35f, 0.35f), 0.9f);
            feedbackAnimator.ShowCombo(0);
        }

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
                ? GetFinalFeedback() + "\n\nPuntaje: " + score
                : "Fin del juego\n\n" + GetFinalFeedback() + "\n\nPuntaje: " + score;
        }

        if (AudioManager.Instance != null && won)
            AudioManager.Instance.PlayWin();

        if (modalAnimator != null)
            modalAnimator.PlayShowAnimation();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private string GetPositiveMessage()
    {
        string[] messages =
        {
            "¡Correcto!",
            "¡Excelente!",
            "¡Muy bien!",
            "¡Buen reciclaje!"
        };

        return messages[Random.Range(0, messages.Length)];
    }

    private string GetNegativeMessage()
    {
        string[] messages =
        {
            "Residuo incorrecto",
            "Intenta otra vez",
            "Ese no corresponde",
            "Cuidado con la clasificación"
        };

        return messages[Random.Range(0, messages.Length)];
    }

    private string GetFinalFeedback()
    {
        if (score >= 800)
            return "Excelente desempeño. Clasificaste los residuos con gran precisión.";

        if (score >= 500)
            return "Buen trabajo. Tu desempeño fue sólido.";

        if (score >= 200)
            return "Puedes mejorar. Revisa mejor el tipo de residuo antes de soltarlo.";

        return "Necesitas practicar más. Observa las instrucciones y vuelve a intentarlo.";
    }
}