using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CollectorUIInputFix : MonoBehaviour
{
    [Header("Buttons")]
    public RectTransform helpButton;
    public RectTransform pauseButton;
    public RectTransform closeInstructionsButton;
    public RectTransform resumeButton;
    public RectTransform restartButton;
    public RectTransform pauseMenuButton;
    public RectTransform endRestartButton;
    public RectTransform endMenuButton;

    [Header("Panels")]
    public GameObject instructionsPanel;
    public GameObject pausePanel;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (IsClicked(helpButton, mousePos))
        {
            Debug.Log("CLICK HELP");
            OpenInstructions();
            return;
        }

        if (IsClicked(pauseButton, mousePos))
        {
            Debug.Log("CLICK PAUSA");
            OpenPause();
            return;
        }

        if (instructionsPanel != null && instructionsPanel.activeSelf)
        {
            if (IsClicked(closeInstructionsButton, mousePos))
            {
                CloseInstructions();
                return;
            }
        }

        if (pausePanel != null && pausePanel.activeSelf)
        {
            if (IsClicked(resumeButton, mousePos))
            {
                ClosePause();
                return;
            }

            if (IsClicked(restartButton, mousePos))
            {
                RestartScene();
                return;
            }

            if (IsClicked(pauseMenuButton, mousePos))
            {
                GoToMenu();
                return;
            }
        }

        if (IsClicked(endRestartButton, mousePos))
        {
            RestartScene();
            return;
        }

        if (IsClicked(endMenuButton, mousePos))
        {
            GoToMenu();
            return;
        }
    }

    private bool IsClicked(RectTransform target, Vector2 mousePos)
    {
        if (target == null)
            return false;

        if (!target.gameObject.activeInHierarchy)
            return false;

        return RectTransformUtility.RectangleContainsScreenPoint(
            target,
            mousePos,
            null
        );
    }

    private void OpenInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    private void CloseInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    private void OpenPause()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    private void ClosePause()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    private void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}