using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCollectorController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 500f;
    public RectTransform movementArea;

    [Header("Movement Feel")]
    public float tiltAmount = 8f;
    public float squashAmount = 0.12f;
    public float animationSmooth = 10f;

    [Header("Trail / Mancha opcional")]
    public bool useTrail = true;
    public Image trailImage;
    public float trailDistance = 18f;
    public float trailAlphaMoving = 0.35f;
    public float trailAlphaIdle = 0f;

    private RectTransform rectTransform;
    private Vector2 input;
    private Vector3 baseScale;
    private Vector3 targetScale;
    private Quaternion targetRotation;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        baseScale = rectTransform.localScale;
        targetScale = baseScale;
        targetRotation = Quaternion.identity;
    }

    private void Start()
    {
        Image playerImage = GetComponent<Image>();
        if (playerImage != null)
            playerImage.raycastTarget = false;

        if (trailImage != null)
        {
            trailImage.raycastTarget = false;
            trailImage.color = new Color(
                trailImage.color.r,
                trailImage.color.g,
                trailImage.color.b,
                0f
            );
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            input = Vector2.zero;
            AnimateMovement();
            UpdateTrail();
            return;
        }

        ReadInput();
        MovePlayer();
        AnimateMovement();
        UpdateTrail();
    }

    private void ReadInput()
    {
        input = Vector2.zero;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            input.x = -1f;

        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            input.x = 1f;

        if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed)
            input.y = 1f;

        if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed)
            input.y = -1f;

        input = input.normalized;
    }

    private void MovePlayer()
    {
        Vector2 pos = rectTransform.anchoredPosition;
        pos += input * moveSpeed * Time.deltaTime;

        if (movementArea != null)
        {
            float halfAreaW = movementArea.rect.width * 0.5f;
            float halfAreaH = movementArea.rect.height * 0.5f;

            float halfPlayerW = rectTransform.rect.width * 0.5f;
            float halfPlayerH = rectTransform.rect.height * 0.5f;

            pos.x = Mathf.Clamp(pos.x, -halfAreaW + halfPlayerW, halfAreaW - halfPlayerW);
            pos.y = Mathf.Clamp(pos.y, -halfAreaH + halfPlayerH, halfAreaH - halfPlayerH);
        }

        rectTransform.anchoredPosition = pos;
    }

    private void AnimateMovement()
    {
        bool moving = input.sqrMagnitude > 0.01f;

        if (moving)
        {
            float squashX = 1f + Mathf.Abs(input.x) * squashAmount;
            float squashY = 1f + Mathf.Abs(input.y) * squashAmount;

            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                targetScale = new Vector3(baseScale.x * squashX, baseScale.y * (1f - squashAmount), baseScale.z);
            else
                targetScale = new Vector3(baseScale.x * (1f - squashAmount), baseScale.y * squashY, baseScale.z);

            targetRotation = Quaternion.Euler(0f, 0f, -input.x * tiltAmount);
        }
        else
        {
            targetScale = baseScale;
            targetRotation = Quaternion.identity;
        }

        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.deltaTime * animationSmooth
        );

        rectTransform.localRotation = Quaternion.Lerp(
            rectTransform.localRotation,
            targetRotation,
            Time.deltaTime * animationSmooth
        );
    }

    private void UpdateTrail()
    {
        if (!useTrail || trailImage == null)
            return;

        bool moving = input.sqrMagnitude > 0.01f;

        RectTransform trailRect = trailImage.GetComponent<RectTransform>();

        if (trailRect != null)
        {
            trailRect.anchoredPosition = rectTransform.anchoredPosition - input * trailDistance;
            trailRect.SetAsFirstSibling();
        }

        Color color = trailImage.color;
        color.a = Mathf.Lerp(
            color.a,
            moving ? trailAlphaMoving : trailAlphaIdle,
            Time.deltaTime * 8f
        );

        trailImage.color = color;
    }
}