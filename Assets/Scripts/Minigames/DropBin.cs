using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropBin : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SortingGameManager.WasteType acceptedType;
    public Image targetImage;

    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 1f, 0.8f, 1f);

    private void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        if (targetImage != null)
            targetImage.color = normalColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DraggableWasteItem item = eventData.pointerDrag.GetComponent<DraggableWasteItem>();
        if (item == null) return;

        ResetColor();

        Debug.Log("Residuo soltado: " + item.wasteType + " | Bote acepta: " + acceptedType);

        if (item.wasteType == acceptedType)
        {
            Debug.Log("DROP CORRECTO");
            item.NotifyCorrectDrop();
            item.MarkAsCorrectlyDropped();
        }
        else
        {
            Debug.Log("DROP INCORRECTO");
            item.NotifyWrongDrop();
            item.ReturnToStart();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetImage != null)
            targetImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetColor();
    }

    private void ResetColor()
    {
        if (targetImage != null)
            targetImage.color = normalColor;
    }
}