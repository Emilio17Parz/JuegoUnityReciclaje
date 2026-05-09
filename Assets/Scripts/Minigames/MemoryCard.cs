using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MemoryCard : MonoBehaviour
{
    [Header("UI")]
    public Image cardImage;
    public Button cardButton;

    [Header("Sprites")]
    public Sprite backSprite;
    public Sprite frontSprite;

    [HideInInspector] public int cardID;
    [HideInInspector] public bool isMatched = false;

    private bool isFaceUp = false;
    private bool isAnimating = false;
    private MemoryGameManager gameManager;

    public void Setup(int id, Sprite front, Sprite back, MemoryGameManager manager)
    {
        cardID = id;
        frontSprite = front;
        backSprite = back;
        gameManager = manager;

        isMatched = false;
        isFaceUp = false;
        isAnimating = false;

        if (cardImage != null)
            cardImage.sprite = backSprite;

        if (cardButton != null)
            cardButton.interactable = true;

        transform.localScale = Vector3.one;
    }

    public void OnCardClicked()
    {
        if (isMatched || isFaceUp || isAnimating) return;
        if (gameManager == null) return;

        gameManager.SelectCard(this);
    }

    public void FlipUp()
    {
        if (!gameObject.activeInHierarchy) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayFlip();

        StartCoroutine(FlipAnimation(frontSprite, true));
    }

    public void FlipDown()
    {
        if (isMatched) return;
        if (!gameObject.activeInHierarchy) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayFlip();

        StartCoroutine(FlipAnimation(backSprite, false));
    }

    public void SetMatched()
    {
        isMatched = true;

        if (cardButton != null)
            cardButton.interactable = false;
    }

    private IEnumerator FlipAnimation(Sprite targetSprite, bool faceUpState)
    {
        isAnimating = true;

        float duration = 0.12f;
        float time = 0f;

        while (time < duration)
        {
            float scaleX = Mathf.Lerp(1f, 0f, time / duration);
            transform.localScale = new Vector3(scaleX, 1f, 1f);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = new Vector3(0f, 1f, 1f);

        if (cardImage != null)
            cardImage.sprite = targetSprite;

        isFaceUp = faceUpState;

        time = 0f;

        while (time < duration)
        {
            float scaleX = Mathf.Lerp(0f, 1f, time / duration);
            transform.localScale = new Vector3(scaleX, 1f, 1f);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;
        isAnimating = false;
    }
}