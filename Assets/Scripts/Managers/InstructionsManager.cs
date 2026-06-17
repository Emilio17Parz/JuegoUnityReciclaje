using UnityEngine;

public class InstructionsManager : MonoBehaviour
{
    public GameObject instructionsPanel;

    public void OpenInstructions()
    {
        instructionsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseInstructions()
    {
        instructionsPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}