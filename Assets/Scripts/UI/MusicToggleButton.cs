using TMPro;
using UnityEngine;

public class MusicToggleButton : MonoBehaviour
{
    public TMP_Text buttonText;

    private void Start()
    {
        UpdateText();
    }

    public void ToggleMusic()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.ToggleMusicMute();

        UpdateText();
    }

    private void UpdateText()
    {
        if (buttonText == null || AudioManager.Instance == null) return;

        buttonText.text = AudioManager.Instance.IsMusicMuted()
            ? "Activar música"
            : "Silenciar música";
    }
}