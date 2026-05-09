using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ModalAnimator : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform modalTransform;
    public float duration = 0.25f;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (modalTransform == null)
            modalTransform = GetComponent<RectTransform>();
    }

    public void PlayShowAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (modalTransform != null)
            modalTransform.localScale = new Vector3(0.8f, 0.8f, 1f);

        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float eased = EaseOutBack(t);

            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            if (modalTransform != null)
            {
                float scale = Mathf.Lerp(0.8f, 1f, eased);
                modalTransform.localScale = new Vector3(scale, scale, 1f);
            }

            time += Time.deltaTime;
            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (modalTransform != null)
            modalTransform.localScale = Vector3.one;
    }

    private float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}