using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MemoryGameManager : MonoBehaviour
{
    [Header("Board")]
    public Transform gridContainer;
    public GameObject cardPrefab;

    [Header("Sprites de cartas")]
    public List<Sprite> ecoSprites;
    public Sprite backSprite;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text movesText;
    public GameObject endPanel;
    public TMP_Text resultText;
    public ModalAnimator modalAnimator;
    public FeedbackAnimator feedbackAnimator;

    [Header("Configuración")]
    public float gameTime = 60f;
    public float flipBackDelay = 0.8f;

    private readonly List<MemoryCard> spawnedCards = new List<MemoryCard>();

    private MemoryCard firstCard;
    private MemoryCard secondCard;

    private int score = 0;
    private int moves = 0;
    private int matchedPairs = 0;
    private int comboStreak = 0;

    private float currentTime;

    private bool canPlay = true;
    private bool gameEnded = false;

    private void Start()
    {
        currentTime = gameTime;

        if (endPanel != null)
            endPanel.SetActive(false);

        GenerateBoard();
        UpdateUI();
    }

    private void Update()
    {
        if (gameEnded) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            LoseGame();
            return;
        }

        if (timeText != null)
            timeText.text = "Tiempo: " + Mathf.CeilToInt(currentTime);
    }

    private void GenerateBoard()
    {
        if (gridContainer == null || cardPrefab == null || ecoSprites == null || ecoSprites.Count == 0)
        {
            Debug.LogError("Faltan referencias en MemoryGameManager.");
            return;
        }

        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);

        spawnedCards.Clear();

        List<CardData> deck = new List<CardData>();

        for (int i = 0; i < ecoSprites.Count; i++)
        {
            deck.Add(new CardData(i, ecoSprites[i]));
            deck.Add(new CardData(i, ecoSprites[i]));
        }

        Shuffle(deck);

        for (int i = 0; i < deck.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, gridContainer);

            MemoryCard card = cardObj.GetComponent<MemoryCard>();

            if (card == null)
            {
                Debug.LogError("El prefab no tiene MemoryCard.");
                continue;
            }

            card.Setup(deck[i].id, deck[i].sprite, backSprite, this);

            spawnedCards.Add(card);
        }
    }

    public void SelectCard(MemoryCard card)
    {
        if (!canPlay || gameEnded || card == null)
            return;

        card.FlipUp();

        if (firstCard == null)
        {
            firstCard = card;
            return;
        }

        if (secondCard == null && card != firstCard)
        {
            secondCard = card;

            moves++;

            UpdateUI();

            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        canPlay = false;

        yield return new WaitForSeconds(flipBackDelay);

        if (firstCard.cardID == secondCard.cardID)
        {
            firstCard.SetMatched();
            secondCard.SetMatched();

            comboStreak++;

            int gained = 100 + ((comboStreak - 1) * 25);

            score += gained;

            matchedPairs++;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMatch();

            if (feedbackAnimator != null)
            {
                feedbackAnimator.ShowFeedback(GetPositiveMessage() + " +" + gained,
                    new Color(0.3f, 1f, 0.3f),
                    0.8f);

                feedbackAnimator.ShowCombo(comboStreak);
            }

            if (matchedPairs >= ecoSprites.Count)
                WinGame();
        }
        else
        {
            firstCard.FlipDown();
            secondCard.FlipDown();

            comboStreak = 0;

            score = Mathf.Max(0, score - 10);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayFail();

            if (feedbackAnimator != null)
            {
                feedbackAnimator.ShowFeedback(GetNegativeMessage(),
                    new Color(1f, 0.35f, 0.35f),
                    0.9f);

                feedbackAnimator.ShowCombo(0);
            }
        }

        yield return new WaitForSeconds(0.2f);

        firstCard = null;
        secondCard = null;

        canPlay = true;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Puntos: " + score;

        if (movesText != null)
            movesText.text = "Movs: " + moves;
    }

    private void WinGame()
    {
        gameEnded = true;

        if (endPanel != null)
            endPanel.SetActive(true);

        if (resultText != null)
        {
            resultText.text =
                "¡Memoria ecológica completada!\n\n" +
                GetFinalFeedback() +
                "\n\nPuntaje: " + score;
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayWin();

        if (modalAnimator != null)
            modalAnimator.PlayShowAnimation();
    }

    private void LoseGame()
    {
        gameEnded = true;

        canPlay = false;

        if (endPanel != null)
            endPanel.SetActive(true);

        if (resultText != null)
        {
            resultText.text =
                "Se acabó el tiempo\n\n" +
                GetFinalFeedback() +
                "\n\nPuntaje: " + score;
        }

        if (modalAnimator != null)
            modalAnimator.PlayShowAnimation();
    }

    private void Shuffle(List<CardData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CardData temp = list[i];

            int randomIndex = Random.Range(i, list.Count);

            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
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
            "¡Excelente!",
            "¡Buen trabajo!",
            "¡Correcto!",
            "¡Muy bien!"
        };

        return messages[Random.Range(0, messages.Length)];
    }

    private string GetNegativeMessage()
    {
        string[] messages =
        {
            "Intenta otra vez",
            "No coinciden",
            "Sigue intentando",
            "Casi lo tienes"
        };

        return messages[Random.Range(0, messages.Length)];
    }

    private string GetFinalFeedback()
    {
        if (score >= 1000)
            return "Excelente memoria y clasificación visual.";

        if (score >= 600)
            return "Buen desempeño en reconocimiento de residuos.";

        if (score >= 300)
            return "Puedes mejorar tu velocidad y precisión.";

        return "Necesitas practicar más el reconocimiento visual.";
    }

    [System.Serializable]
    public class CardData
    {
        public int id;
        public Sprite sprite;

        public CardData(int newId, Sprite newSprite)
        {
            id = newId;
            sprite = newSprite;
        }
    }
}