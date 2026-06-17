using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfoSceneManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text pageTitleText;
    public TMP_Text infoText;
    public Image infoImage;

    public Button previousButton;
    public Button nextButton;

    [Header("Images opcionales")]
    public Sprite recyclableImage;
    public Sprite organicImage;
    public Sprite nonRecyclableImage;

    private int currentPage = 0;

    private string[] titles;
    private string[] descriptions;
    private Sprite[] images;

    private void Start()
    {
        titles = new string[]
        {
            "Residuos reciclables",
            "Residuos orgánicos",
            "Residuos no reciclables"
        };

        descriptions = new string[]
        {
            "Los residuos reciclables son aquellos materiales que pueden transformarse y utilizarse nuevamente.\n\n" +
            "Ejemplos:\n" +
            "- Papel\n" +
            "- Cartón\n" +
            "- Plástico\n" +
            "- Vidrio\n" +
            "- Latas metálicas\n\n" +
            "Estos residuos deben colocarse en el bote de reciclables.",

            "Los residuos orgánicos son aquellos de origen natural que pueden descomponerse con el tiempo.\n\n" +
            "Ejemplos:\n" +
            "- Cáscaras de fruta\n" +
            "- Restos de comida\n" +
            "- Hojas\n" +
            "- Residuos vegetales\n\n" +
            "Estos residuos deben colocarse en el bote de orgánicos.",

            "Los residuos no reciclables son aquellos que no pueden reutilizarse fácilmente o requieren un tratamiento especial.\n\n" +
            "Ejemplos:\n" +
            "- Baterías\n" +
            "- Aerosoles\n" +
            "- Envolturas sucias\n" +
            "- Residuos contaminados\n\n" +
            "Estos residuos deben colocarse en el bote de no reciclables."
        };

        images = new Sprite[]
        {
            recyclableImage,
            organicImage,
            nonRecyclableImage
        };

        ShowPage();
    }

    public void NextPage()
    {
        if (currentPage < titles.Length - 1)
        {
            currentPage++;
            ShowPage();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage();
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowPage()
    {
        pageTitleText.text = titles[currentPage];
        infoText.text = descriptions[currentPage];

        if (infoImage != null)
        {
            infoImage.sprite = images[currentPage];
            infoImage.enabled = images[currentPage] != null;
        }

        previousButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < titles.Length - 1;
    }
}