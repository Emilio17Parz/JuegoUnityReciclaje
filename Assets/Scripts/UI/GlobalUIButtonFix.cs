using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GlobalUIButtonFix : MonoBehaviour
{
    [Header("Reporte")]
    public Button reportButton;
    public Button submitReportButton;
    public Button closeReportButton;
    public GameObject reportPanel;
    public ReportManager reportManager;

    [Header("Musica")]
    public Button musicToggleButton;

    private void Update()
    {
        if (reportPanel != null && reportPanel.activeSelf)
            reportPanel.transform.SetAsLastSibling();

        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (IsClicked(reportButton, mousePos))
        {
            if (reportPanel != null)
            {
                reportPanel.SetActive(true);
                reportPanel.transform.SetAsLastSibling();
            }
            return;
        }

        if (IsClicked(closeReportButton, mousePos))
        {
            if (reportPanel != null)
                reportPanel.SetActive(false);

            return;
        }

        if (IsClicked(submitReportButton, mousePos))
        {
            if (reportManager != null)
                reportManager.SubmitReport();

            return;
        }

        if (IsClicked(musicToggleButton, mousePos))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.ToggleMusicMute();

            return;
        }
    }

    private bool IsClicked(Button button, Vector2 mousePos)
    {
        if (button == null) return false;
        if (!button.gameObject.activeInHierarchy) return false;

        RectTransform rt = button.GetComponent<RectTransform>();

        if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos, null))
            return true;

        Image[] images = button.GetComponentsInChildren<Image>();

        foreach (Image img in images)
        {
            RectTransform childRt = img.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(childRt, mousePos, null))
                return true;
        }

        return false;
    }
}