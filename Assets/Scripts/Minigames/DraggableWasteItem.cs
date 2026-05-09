using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWasteItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SortingGameManager.WasteType wasteType;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 startPosition;
    private SortingGameManager gameManager;
    private bool wasDroppedCorrectly;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        startPosition = rectTransform.anchoredPosition;
    }

    public void Initialize(SortingGameManager manager)
    {
        gameManager = manager;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (wasDroppedCorrectly) return;

        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.85f;
        transform.localScale = Vector3.one * 1.08f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayFlip();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (wasDroppedCorrectly) return;

        if (canvas != null)
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (wasDroppedCorrectly) return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDrop();
    }

    public void ReturnToStart()
    {
        rectTransform.anchoredPosition = startPosition;
        transform.localScale = Vector3.one;
    }

    public void MarkAsCorrectlyDropped()
    {
        wasDroppedCorrectly = true;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }

    public void NotifyCorrectDrop()
    {
        if (gameManager != null)
        {
            Debug.Log("NotifyCorrectDrop -> manager OK");
            gameManager.HandleCorrectDrop(this);
        }
        else
        {
            Debug.LogError("NotifyCorrectDrop -> gameManager es NULL");
        }
    }

    public void NotifyWrongDrop()
    {
        if (gameManager != null)
        {
            Debug.Log("NotifyWrongDrop -> manager OK");
            gameManager.HandleWrongDrop(this);
        }
        else
        {
            Debug.LogError("NotifyWrongDrop -> gameManager es NULL");
        }
    }
}