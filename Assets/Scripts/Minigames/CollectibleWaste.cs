using UnityEngine;
using UnityEngine.UI;

public class CollectibleWaste : MonoBehaviour
{
    public CollectorGameManager.WasteType wasteType;
    public Sprite wasteSprite;

    private void Awake()
    {
        Image img = GetComponent<Image>();
        if (img != null && wasteSprite == null)
            wasteSprite = img.sprite;
    }
}