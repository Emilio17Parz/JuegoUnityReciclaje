using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBinController : MonoBehaviour
{
    public float moveSpeed = 700f;
    public RectTransform movementArea;
    public RectTransform catchZone;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        float input = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed)
                input = -1f;
            else if (Keyboard.current.rightArrowKey.isPressed)
                input = 1f;
        }

        Vector2 pos = rectTransform.anchoredPosition;
        pos.x += input * moveSpeed * Time.deltaTime;

        if (movementArea != null)
        {
            float halfArea = movementArea.rect.width * 0.5f;
            float halfBin = rectTransform.rect.width * 0.5f;
            pos.x = Mathf.Clamp(pos.x, -halfArea + halfBin, halfArea - halfBin);
        }

        rectTransform.anchoredPosition = pos;
    }
}