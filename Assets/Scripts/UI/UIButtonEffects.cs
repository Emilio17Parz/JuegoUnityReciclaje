using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform targetTransform;
    public Image targetImage;

    public Vector3 normalScale = Vector3.one;
    public Vector3 hoverScale = new Vector3(1.06f, 1.06f, 1f);
    public Vector3 pressedScale = new Vector3(0.96f, 0.96f, 1f);

    public Color normalColor = Color.white;
    public Color hoverColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    public Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);

    public float speed = 12f;

    private Vector3 desiredScale;

    private void Awake()
    {
        if (targetTransform == null)
            targetTransform = GetComponent<RectTransform>();

        if (targetImage == null)
            targetImage = GetComponent<Image>();

        desiredScale = normalScale;

        if (targetImage != null)
            targetImage.color = normalColor;
    }

    private void Update()
    {
        if (targetTransform != null)
        {
            targetTransform.localScale = Vector3.Lerp(
                targetTransform.localScale,
                desiredScale,
                Time.deltaTime * speed
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        desiredScale = hoverScale;
        if (targetImage != null)
            targetImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        desiredScale = normalScale;
        if (targetImage != null)
            targetImage.color = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        desiredScale = pressedScale;
        if (targetImage != null)
            targetImage.color = pressedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        desiredScale = hoverScale;
        if (targetImage != null)
            targetImage.color = hoverColor;
    }
}