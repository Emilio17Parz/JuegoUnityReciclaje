using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExpressGameManager : MonoBehaviour
{
    public enum WasteType { Recyclable, Organic, NonRecyclable }

    [System.Serializable]
    public class WasteQuestion
    {
        public string name;
        public Sprite sprite;
        public WasteType type;
    }

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text livesText;
    public TMP_Text roundText;
    public Image wasteImage;
    public Image timerBar;
    public GameObject endPanel;
    public TMP_Text resultText;
    public ModalAnimator modalAnimator;
    public FeedbackAnimator feedbackAnimator;

    [Header("Questions")]
    public List<WasteQuestion> questions = new List<WasteQuestion>();

    [Header("Gameplay")]
    public int startLives = 3;
    public int maxRounds = 10;
    public int pointsPerCorrect = 100;
    public int penaltyPerWrong = 25;
    public int comboBonus = 25;
    public float startAnswerTime = 8f;
    public float minAnswerTime = 3f;
    public float difficultyStep = 0.5f;

    private int score;
    private int lives;
    private int currentRound;
    private int comboStreak;
    private float answerTime;
    private float currentTimer;
    private bool gameEnded;

    private WasteQuestion currentQuestion;

    private void Start()
    {
        Time.timeScale = 1f;

        score = 0;
        lives = startLives;
        currentRound = 0;
        comboStreak = 0;
        gameEnded = false;

        if (endPanel != null)
            endPanel.SetActive(false);

        NextQuestion();
        UpdateUI();
    }

    private void Update()
    {
        if (gameEnded) return;

        currentTimer -= Time.deltaTime;

        if (timerBar != null)
            timerBar.fillAmount = Mathf.Clamp01(currentTimer / answerTime);

        if (currentTimer <= 0f)
        {
            HandleWrong("Tiempo agotado");
        }

        UpdateUI();
    }

    private void NextQuestion()
    {
        if (questions == null || questions.Count == 0)
        {
            Debug.LogError("No hay preguntas/residuos en ExpressGameManager.");
            EndGame(false);
            return;
        }

        currentRound++;

        if (currentRound > maxRounds)
        {
            EndGame(true);
            return;
        }

        answerTime = Mathf.Max(minAnswerTime, startAnswerTime - ((currentRound - 1) * difficultyStep));
        currentTimer = answerTime;

        currentQuestion = questions[Random.Range(0, questions.Count)];

        if (wasteImage != null)
        {
            wasteImage.sprite = currentQuestion.sprite;
            wasteImage.enabled = currentQuestion.sprite != null;
            wasteImage.preserveAspect = true;
        }

        if (feedbackAnimator != null)
        {
            feedbackAnimator.ShowFeedback("Clasifica rápido", Color.green, 0.6f);
            feedbackAnimator.ShowCombo(comboStreak);
        }

        UpdateUI();
    }

    public void ChooseRecyclable()
    {
        ChooseAnswer(WasteType.Recyclable);
    }

    public void ChooseOrganic()
    {
        ChooseAnswer(WasteType.Organic);
    }

    public void ChooseNonRecyclable()
    {
        ChooseAnswer(WasteType.NonRecyclable);
    }

    private void ChooseAnswer(WasteType selectedType)
    {
        if (gameEnded || currentQuestion == null) return;

        if (selectedType == currentQuestion.type)
            HandleCorrect();
        else
            HandleWrong("Incorrecto");
    }

    private void HandleCorrect()
    {
        comboStreak++;

        int gained = pointsPerCorrect + ((comboStreak - 1) * comboBonus);
        score += gained;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMatch();

        if (feedbackAnimator != null)
        {
            feedbackAnimator.ShowFeedback(GetPositiveMessage() + " +" + gained, new Color(0.3f, 1f, 0.3f), 0.8f);
            feedbackAnimator.ShowCombo(comboStreak);
        }

        NextQuestion();
    }

    private void HandleWrong(string reason)
    {
        comboStreak = 0;
        score = Mathf.Max(0, score - penaltyPerWrong);
        lives--;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayFail();

        if (feedbackAnimator != null)
        {
            feedbackAnimator.ShowFeedback(reason, new Color(1f, 0.35f, 0.35f), 0.8f);
            feedbackAnimator.ShowCombo(0);
        }

        if (lives <= 0)
        {
            lives = 0;
            EndGame(false);
            return;
        }

        NextQuestion();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Puntos: " + score;

        if (timeText != null)
            timeText.text = "Tiempo: " + Mathf.CeilToInt(currentTimer);

        if (livesText != null)
            livesText.text = "Vidas: " + lives;

        if (roundText != null)
            roundText.text = "Ronda: " + Mathf.Min(currentRound, maxRounds) + " / " + maxRounds;
    }

    private void EndGame(bool won)
    {
        if (gameEnded) return;

        gameEnded = true;

        if (endPanel != null)
        {
            endPanel.SetActive(true);
            endPanel.transform.SetAsLastSibling();
        }

        if (resultText != null)
        {
            resultText.text = won
                ? "¡Clasificación completada!\n\n" + GetFinalFeedback() + "\n\nPuntaje: " + score
                : "Fin del juego\n\n" + GetFinalFeedback() + "\n\nPuntaje: " + score;
        }

        if (AudioManager.Instance != null && won)
            AudioManager.Instance.PlayWin();

        if (modalAnimator != null)
            modalAnimator.PlayShowAnimation();
    }

    private string GetPositiveMessage()
    {
        string[] messages =
        {
            "¡Correcto!",
            "¡Excelente!",
            "¡Muy bien!",
            "¡Rápido y preciso!"
        };

        return messages[Random.Range(0, messages.Length)];
    }

    private string GetFinalFeedback()
    {
        if (score >= 1200)
            return "Excelente desempeño. Clasificaste con rapidez y precisión.";

        if (score >= 700)
            return "Buen trabajo. Tu clasificación fue bastante acertada.";

        if (score >= 300)
            return "Puedes mejorar. Observa mejor cada residuo antes de responder.";

        return "Necesitas practicar más la clasificación de residuos.";
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
}