using System.Collections;
using TMPro;
using UnityEngine;

public class FeedbackAnimator : MonoBehaviour
{
    public TMP_Text feedbackText;
    public TMP_Text comboText;

    private Coroutine feedbackRoutine;
    private Coroutine comboRoutine;

    private void Start()
    {
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (comboText != null)
            comboText.gameObject.SetActive(false);
    }

    public void ShowFeedback(string message, Color color, float duration = 0.8f)
    {
        if (feedbackText == null) return;

        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        feedbackRoutine = StartCoroutine(AnimateText(feedbackText, message, color, duration));
    }

    public void ShowCombo(int combo)
    {
        if (comboText == null) return;

        if (combo <= 1)
        {
            comboText.gameObject.SetActive(false);
            return;
        }

        if (comboRoutine != null)
            StopCoroutine(comboRoutine);

        comboRoutine = StartCoroutine(AnimateText(comboText, "Combo x" + combo, new Color(1f, 0.85f, 0.1f), 1f));
    }

    private IEnumerator AnimateText(TMP_Text text, string message, Color color, float duration)
    {
        text.gameObject.SetActive(true);
        text.text = message;
        text.color = color;
        text.transform.localScale = Vector3.one * 0.7f;

        float growTime = 0.15f;
        float timer = 0f;

        while (timer < growTime)
        {
            float t = timer / growTime;
            text.transform.localScale = Vector3.Lerp(Vector3.one * 0.7f, Vector3.one * 1.1f, t);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        text.transform.localScale = Vector3.one;

        yield return new WaitForSecondsRealtime(duration);

        text.gameObject.SetActive(false);
    }
}