using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CollectorGameManager : MonoBehaviour
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
    public TMP_Text roundText;
    public GameObject endPanel;
    public TMP_Text resultText;
    public ModalAnimator modalAnimator;
    public FeedbackAnimator feedbackAnimator;

    [Header("Player")]
    public PlayerCollectorController player;
    public Image carriedWasteImage;

    [Header("Rounds")]
    public CollectorWasteSpawner wasteSpawner;
    public int maxRounds = 5;

    [Header("Gameplay")]
    public float gameTime = 60f;
    public int startLives = 3;
    public int pointsPerCorrect = 100;
    public int penaltyPerWrong = 25;
    public int comboBonus = 25;

    private int score;
    private int lives;
    private float currentTime;
    private bool gameEnded;

    private WasteType carriedType;
    private bool carryingWaste = false;

    private int currentRound = 1;
    private int remainingWaste = 0;
    private int comboStreak = 0;

    private RectTransform playerRect;

    private void Start()
    {
        Time.timeScale = 1f;

        score = 0;
        lives = startLives;
        currentTime = gameTime;
        gameEnded = false;
        carryingWaste = false;
        comboStreak = 0;
        currentRound = 1;

        if (player != null)
            playerRect = player.GetComponent<RectTransform>();

        if (endPanel != null)
            endPanel.SetActive(false);

        if (carriedWasteImage != null)
            carriedWasteImage.gameObject.SetActive(false);

        UpdateUI();

        if (feedbackAnimator != null)
        {
            feedbackAnimator.ShowFeedback("Ronda " + currentRound, new Color(1f, 0.9f, 0.2f), 1.2f);
            feedbackAnimator.ShowCombo(0);
        }

        if (wasteSpawner != null)
            wasteSpawner.SpawnRound(currentRound);
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

        CheckPickup();
        CheckDeposit();

        UpdateUI();
    }

    public void RegisterRoundWaste(int count)
    {
        remainingWaste = count;
        UpdateUI();

        Debug.Log("Ronda " + currentRound + " residuos: " + remainingWaste);
    }

    private void CheckPickup()
    {
        if (carryingWaste || playerRect == null) return;

        CollectibleWaste[] wastes = FindObjectsByType<CollectibleWaste>(FindObjectsSortMode.None);

        foreach (CollectibleWaste waste in wastes)
        {
            RectTransform wasteRect = waste.GetComponent<RectTransform>();
            if (wasteRect == null) continue;

            if (IsOverlapping(playerRect, wasteRect))
            {
                carryingWaste = true;
                carriedType = waste.wasteType;

                if (carriedWasteImage != null)
                {
                    carriedWasteImage.sprite = waste.wasteSprite;
                    carriedWasteImage.gameObject.SetActive(true);
                }

                Destroy(waste.gameObject);

                if (feedbackAnimator != null)
                    feedbackAnimator.ShowFeedback("Residuo recogido", Color.white, 0.6f);

                break;
            }
        }
    }

    private void CheckDeposit()
    {
        if (!carryingWaste || playerRect == null) return;

        CollectorBin[] bins = FindObjectsByType<CollectorBin>(FindObjectsSortMode.None);

        foreach (CollectorBin bin in bins)
        {
            RectTransform binRect = bin.GetComponent<RectTransform>();
            if (binRect == null) continue;

            if (IsOverlapping(playerRect, binRect))
            {
                bool correct = bin.acceptedType == carriedType;

                if (correct)
                    CorrectDeposit();
                else
                    WrongDeposit();

                ClearCarriedWaste();

                remainingWaste--;

                if (remainingWaste <= 0 && !gameEnded)
                    AdvanceRound();

                break;
            }
        }
    }

    private void CorrectDeposit()
    {
        comboStreak++;

        int gained = pointsPerCorrect + ((comboStreak - 1) * comboBonus);
        score += gained;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMatch();

        if (feedbackAnimator != null)
        {
            feedbackAnimator.ShowFeedback(GetPositiveMessage() + " +" + gained, new Color(0.3f, 1f, 0.3f), 1f);
            feedbackAnimator.ShowCombo(comboStreak);
        }
    }

    private void WrongDeposit()
    {
        comboStreak = 0;

        score = Mathf.Max(0, score - penaltyPerWrong);
        lives--;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayFail();

        if (feedbackAnimator != null)
        {
            feedbackAnimator.ShowFeedback(GetNegativeMessage(), new Color(1f, 0.35f, 0.35f), 1f);
            feedbackAnimator.ShowCombo(0);
        }

        if (lives <= 0)
        {
            lives = 0;
            EndGame(false);
        }
    }

    private void ClearCarriedWaste()
    {
        carryingWaste = false;

        if (carriedWasteImage != null)
        {
            carriedWasteImage.sprite = null;
            carriedWasteImage.gameObject.SetActive(false);
        }
    }

    private void AdvanceRound()
    {
        currentRound++;

        if (currentRound > maxRounds)
        {
            EndGame(true);
            return;
        }

        if (feedbackAnimator != null)
        {
            feedbackAnimator.ShowFeedback("Ronda " + currentRound, new Color(1f, 0.9f, 0.2f), 1.3f);
            feedbackAnimator.ShowCombo(0);
        }

        if (wasteSpawner != null)
            wasteSpawner.SpawnRound(currentRound);

        UpdateUI();
    }

    private bool IsOverlapping(RectTransform a, RectTransform b)
    {
        Rect rectA = GetWorldRect(a);
        Rect rectB = GetWorldRect(b);

        rectA.xMin -= 15f;
        rectA.xMax += 15f;
        rectA.yMin -= 15f;
        rectA.yMax += 15f;

        return rectA.Overlaps(rectB);
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        return new Rect(
            corners[0].x,
            corners[0].y,
            corners[2].x - corners[0].x,
            corners[2].y - corners[0].y
        );
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Puntos: " + score;

        if (timeText != null)
            timeText.text = "Tiempo: " + Mathf.CeilToInt(currentTime);

        if (livesText != null)
            livesText.text = "Vidas: " + lives;

        if (roundText != null)
            roundText.text = "Ronda: " + currentRound + " / " + maxRounds;
    }

 private void EndGame(bool won)
    {
        if (gameEnded) return;

        gameEnded = true;
        currentTime = Mathf.Max(0f, currentTime);
        lives = Mathf.Max(0, lives);

        ClearCarriedWaste();
        UpdateUI();

        if (endPanel != null)
        {
            endPanel.SetActive(true);
            endPanel.transform.SetAsLastSibling();
        }

        if (resultText != null)
        {
            string title;
            string reason;

            if (won)
            {
                title = "¡Zona limpia!";
                reason = GetFinalFeedback();
            }
            else if (lives <= 0)
            {
                title = "Fin del juego";
                reason = "Te quedaste sin vidas al clasificar residuos incorrectamente.";
            }
            else if (currentTime <= 0)
            {
                title = "Tiempo agotado";
                reason = "No lograste limpiar la zona antes de que terminara el tiempo.";
            }
            else
            {
                title = "Fin del juego";
                reason = GetFinalFeedback();
            }

            resultText.text =
                title +
                "\n\n" +
                reason +
                "\n\nPuntaje: " +
                score;
        }

        if (AudioManager.Instance != null && won)
            AudioManager.Instance.PlayWin();

        if (modalAnimator != null)
            modalAnimator.PlayShowAnimation();

        Debug.Log("END GAME ACTIVADO: " + (won ? "GANASTE" : "PERDISTE"));
    }
    private string GetPositiveMessage()
    {
        string[] messages =
        {
            "¡Correcto!",
            "¡Excelente!",
            "¡Muy bien!",
            "¡Zona más limpia!"
        };

        return messages[Random.Range(0, messages.Length)];
    }

    private string GetNegativeMessage()
    {
        string[] messages =
        {
            "Bote incorrecto",
            "Ese no corresponde",
            "Revisa la clasificación",
            "Intenta otro bote"
        };

        return messages[Random.Range(0, messages.Length)];
    }

    private string GetFinalFeedback()
    {
        if (score >= 1000)
            return "Excelente limpieza. Clasificaste muchos residuos correctamente.";

        if (score >= 600)
            return "Buen trabajo. Lograste mantener la zona más limpia.";

        if (score >= 300)
            return "Puedes mejorar. Revisa bien el bote antes de depositar.";

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