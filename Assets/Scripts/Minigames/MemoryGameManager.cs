using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Configuración")]
    public float gameTime = 60f;
    public float flipBackDelay = 0.8f;

    private readonly List<MemoryCard> spawnedCards = new List<MemoryCard>();
    private MemoryCard firstCard;
    private MemoryCard secondCard;

    private int score = 0;
    private int moves = 0;
    private int matchedPairs = 0;
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
                Debug.LogError("El prefab no tiene el script MemoryCard.");
                continue;
            }

            card.Setup(deck[i].id, deck[i].sprite, backSprite, this);
            spawnedCards.Add(card);
        }
    }

    public void SelectCard(MemoryCard card)
    {
        if (!canPlay || gameEnded || card == null) return;

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

            score += 100;
            matchedPairs++;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMatch();

            if (matchedPairs >= ecoSprites.Count)
                WinGame();
        }
        else
        {
            firstCard.FlipDown();
            secondCard.FlipDown();
            score = Mathf.Max(0, score - 10);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayFail();
        }

        yield return new WaitForSeconds(0.25f);

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
            resultText.text = "¡Ganaste!\nPuntaje: " + score;

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
            resultText.text = "Se acabó el tiempo";

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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