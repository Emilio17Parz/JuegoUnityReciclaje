using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CatcherGameManager : MonoBehaviour
{
    public enum WasteType
    {
        Recyclable,
        Organic,
        NonRecyclable
    }

    [System.Serializable]
    public class RoundData
    {
        public WasteType targetType;
        public string roundLabel;
        public Sprite binSprite;
    }

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text livesText;
    public TMP_Text roundText;
    public TMP_Text feedbackText;
    public TMP_Text comboText;
    public GameObject endPanel;
    public TMP_Text resultText;
    public ModalAnimator modalAnimator;

    [Header("Player Bin")]
    public Image playerBinImage;

    [Header("Rounds")]
    public List<RoundData> rounds = new List<RoundData>();

    [Header("Gameplay")]
    public float gameTime = 45f;
    public int startLives = 3;
    public int pointsPerCorrect = 100;
    public int penaltyPerWrong = 25;
    public int comboBonus = 25;
    public float roundDuration = 15f;

    private int score;
    private int lives;
    private float currentTime;
    private bool gameEnded;
    private int currentRoundIndex = 0;
    private int comboStreak = 0;
    private float roundTimer;

    public WasteType CurrentTargetType =>
        rounds != null && rounds.Count > 0 ? rounds[currentRoundIndex].targetType : WasteType.Recyclable;

    public bool IsGameEnded => gameEnded;

    private void Start()
    {
        score = 0;
        lives = startLives;
        currentTime = gameTime;
        roundTimer = roundDuration;
        gameEnded = false;
        comboStreak = 0;

        if (endPanel != null)
            endPanel.SetActive(false);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        ApplyCurrentRound();
        UpdateUI();
    }

    private void Update()
    {
        if (gameEnded) return;

        currentTime -= Time.deltaTime;
        roundTimer -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            EndGame(false);
            return;
        }

        if (roundTimer <= 0f)
        {
            AdvanceRound();
            roundTimer = roundDuration;
        }

        UpdateUI();
    }

    public void HandleCaughtWaste(WasteType caughtType)
    {
        if (gameEnded) return;

        Debug.Log("Atrapa -> " + caughtType + " | objetivo -> " + CurrentTargetType);

        if (caughtType == CurrentTargetType)
        {
            comboStreak++;
            int gained = pointsPerCorrect + ((comboStreak - 1) * comboBonus);
            score += gained;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMatch();

            ShowFeedback("¡Correcto! +" + gained, new Color(0.3f, 1f, 0.3f), 0.9f);
        }
        else
        {
            comboStreak = 0;
            score = Mathf.Max(0, score - penaltyPerWrong);
            lives--;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayFail();

            ShowFeedback("Incorrecto", new Color(1f, 0.35f, 0.35f), 1.0f);

            if (lives <= 0)
            {
                lives = 0;
                EndGame(false);
                return;
            }
        }

        UpdateUI();
    }

    public void AdvanceRound()
    {
        if (rounds == null || rounds.Count == 0) return;

        currentRoundIndex++;
        if (currentRoundIndex >= rounds.Count)
        {
            EndGame(true);
            return;
        }

        ApplyCurrentRound();
    }

    private void ApplyCurrentRound()
    {
        if (rounds == null || rounds.Count == 0) return;

        RoundData round = rounds[currentRoundIndex];

        if (roundText != null)
            roundText.text = "Atrapa: " + round.roundLabel;

        if (playerBinImage != null && round.binSprite != null)
            playerBinImage.sprite = round.binSprite;

        ShowFeedback("Ronda: " + round.roundLabel, new Color(1f, 0.9f, 0.2f), 1.8f);
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
        {
            if (comboStreak > 1)
                comboText.text = "Combo x" + comboStreak;
            else
                comboText.text = "";
        }
    }

    private void EndGame(bool won)
    {
        gameEnded = true;

        if (endPanel != null)
            endPanel.SetActive(true);

        if (resultText != null)
            resultText.text = won
                ? "¡Ganaste!\nPuntaje: " + score
                : "Fin del juego\nPuntaje: " + score;

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

    private void ShowFeedback(string message, Color color, float duration)
    {
        if (feedbackText == null) return;

        StopAllCoroutines();
        StartCoroutine(FeedbackRoutine(message, color, duration));
    }

    private IEnumerator FeedbackRoutine(string message, Color color, float duration)
    {
        feedbackText.gameObject.SetActive(true);
        feedbackText.text = message;
        feedbackText.color = color;

        yield return new WaitForSeconds(duration);

        feedbackText.gameObject.SetActive(false);
    }
}