using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ReportButtonFix : MonoBehaviour
{
    public Button reportButton;
    public GameObject reportPanel;

    private void Update()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (IsClicked(reportButton, mousePos))
        {
            Debug.Log("CLICK REPORTE");

            if (reportPanel != null)
            {
                reportPanel.SetActive(true);
                reportPanel.transform.SetAsLastSibling();
                Debug.Log("REPORT PANEL ACTIVADO");
            }
            else
            {
                Debug.LogError("ReportPanel no está asignado en ReportButtonFix.");
            }
        }
    }

    private bool IsClicked(Button button, Vector2 mousePos)
    {
        if (button == null) return false;
        if (!button.gameObject.activeInHierarchy) return false;

        RectTransform rt = button.GetComponent<RectTransform>();

        if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos, null))
            return true;

        Image[] childImages = button.GetComponentsInChildren<Image>();

        foreach (Image img in childImages)
        {
            RectTransform childRt = img.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(childRt, mousePos, null))
                return true;
        }

        return false;
    }
}