using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExpressUIInputFix : MonoBehaviour
{
    public Button recyclableButton;
    public Button organicButton;
    public Button nonRecyclableButton;
    public Button helpButton;
    public Button pauseButton;
    public Button closeInstructionsButton;
    public Button resumeButton;
    public Button restartButton;
    public Button pauseMenuButton;
    public Button endRestartButton;
    public Button endMenuButton;

    public GameObject instructionsPanel;
    public GameObject pausePanel;

    public ExpressGameManager gameManager;

    private void Update()
    {
        bool click = false;
        Vector2 mousePos = Vector2.zero;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            click = true;
            mousePos = Mouse.current.position.ReadValue();
        }

        if (Input.GetMouseButtonDown(0))
        {
            click = true;
            mousePos = Input.mousePosition;
        }

        if (!click) return;

        Debug.Log("CLICK DETECTADO EN EXPRESS");

        if (ManualClick(recyclableButton, mousePos))
        {
            Debug.Log("CLICK RECICLABLE");
            gameManager.ChooseRecyclable();
            return;
        }

        if (ManualClick(organicButton, mousePos))
        {
            Debug.Log("CLICK ORGANICO");
            gameManager.ChooseOrganic();
            return;
        }

        if (ManualClick(nonRecyclableButton, mousePos))
        {
            Debug.Log("CLICK NO RECICLABLE");
            gameManager.ChooseNonRecyclable();
            return;
        }

        if (ManualClick(helpButton, mousePos))
        {
            OpenInstructions();
            return;
        }

        if (ManualClick(pauseButton, mousePos))
        {
            OpenPause();
            return;
        }

        if (ManualClick(closeInstructionsButton, mousePos))
        {
            CloseInstructions();
            return;
        }

        if (ManualClick(resumeButton, mousePos))
        {
            ClosePause();
            return;
        }

        if (ManualClick(restartButton, mousePos) || ManualClick(endRestartButton, mousePos))
        {
            RestartScene();
            return;
        }

        if (ManualClick(pauseMenuButton, mousePos) || ManualClick(endMenuButton, mousePos))
        {
            GoToMenu();
            return;
        }
    }

    private bool ManualClick(Button button, Vector2 mousePos)
    {
        if (button == null) return false;
        if (!button.gameObject.activeInHierarchy) return false;

        RectTransform rt = button.GetComponent<RectTransform>();

        if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos, null))
            return true;

        Image img = button.GetComponent<Image>();

        if (img != null)
        {
            RectTransform imageRt = img.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(imageRt, mousePos, null))
                return true;
        }

        Image[] childImages = button.GetComponentsInChildren<Image>();

        foreach (Image childImg in childImages)
        {
            RectTransform childRt = childImg.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(childRt, mousePos, null))
            {
                Debug.Log("CLICK BOTON POR IMAGEN: " + button.name);
                return true;
            }
        }

        return false;
    }
    private void OpenInstructions()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);
            instructionsPanel.transform.SetAsLastSibling();
        }

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
        {
            pausePanel.SetActive(true);
            pausePanel.transform.SetAsLastSibling();
        }

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