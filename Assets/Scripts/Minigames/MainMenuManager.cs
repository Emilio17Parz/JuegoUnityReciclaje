using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject infoPanel;
    public TMP_Text gameTitleText;
    public TMP_Text instructionsText;

    private string selectedSceneName;

    public void ShowMemoryInfo()
    {
        selectedSceneName = "Minigame_Memory";
        gameTitleText.text = "Memorama ecológico";
        instructionsText.text =
            "Encuentra las parejas de residuos antes de que termine el tiempo.\n\n" +
            "Toca dos cartas para voltearlas. Si coinciden, ganas puntos.";
        infoPanel.SetActive(true);
    }

    public void ShowSortingInfo()
    {
        selectedSceneName = "Minigame_Sorting";
        gameTitleText.text = "Clasificación de residuos";
        instructionsText.text =
            "Arrastra cada residuo al bote correcto.\n\n" +
            "Gris: reciclables\nVerde: orgánicos\nNaranja: no reciclables.";
        infoPanel.SetActive(true);
    }

    public void ShowCatcherInfo()
    {
        selectedSceneName = "Minigame_Catcher";
        gameTitleText.text = "Atrapa residuos";
        instructionsText.text =
            "Mueve el bote con las flechas izquierda y derecha.\n\n" +
            "Atrapa solo el tipo de residuo indicado en cada ronda.";
        infoPanel.SetActive(true);
    }

    public void ShowExpressInfo()
    {
        selectedSceneName = "Minigame_Express";
        gameTitleText.text = "Clasificación exprés";
        instructionsText.text =
            "Observa el residuo que aparece en pantalla y elige rápidamente el bote correcto.\n\n" +
            "Cada acierto suma puntos y aumenta tu combo. Cada error te quita una vida.\n\n" +
            "El tiempo para responder disminuye conforme avanzan las rondas.";
        infoPanel.SetActive(true);
    }

    public void ShowCollectorInfo()
    {
        selectedSceneName = "Minigame_Collector";
        gameTitleText.text = "Recolector ecológico";
        instructionsText.text =
            "Mueve el cubito con las flechas o las teclas WASD.\n\n" +
            "Recoge los residuos de la zona y llévalos al bote correcto.\n\n" +
            "Solo puedes cargar un residuo a la vez. Si te equivocas de bote, pierdes una vida.";
        infoPanel.SetActive(true);
    }

    public void PlaySelectedGame()
    {
        if (!string.IsNullOrEmpty(selectedSceneName))
            SceneManager.LoadScene(selectedSceneName);
    }

    public void CloseInfoPanel()
    {
        infoPanel.SetActive(false);
        selectedSceneName = "";
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToInfo()
    {
        SceneManager.LoadScene("InfoScene");
    }
}