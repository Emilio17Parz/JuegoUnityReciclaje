using UnityEngine;

public class FallingWaste : MonoBehaviour
{
    public CatcherGameManager.WasteType wasteType;
    public float fallSpeed = 280f;

    private RectTransform rectTransform;
    private CatcherGameManager gameManager;
    private RectTransform catchZone;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gameManager = FindFirstObjectByType<CatcherGameManager>();

        PlayerBinController playerBin = FindFirstObjectByType<PlayerBinController>();
        if (playerBin != null)
            catchZone = playerBin.catchZone;

        if (catchZone == null)
            Debug.LogError("FallingWaste: catchZone no fue asignada desde PlayerBinController.");
    }

    private void Update()
    {
        if (gameManager != null && gameManager.IsGameEnded)
        {
            Destroy(gameObject);
            return;
        }

        rectTransform.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;

        if (catchZone != null && IsCaught())
        {
            Debug.Log("CAPTURADO -> " + wasteType);

            if (gameManager != null)
                gameManager.HandleCaughtWaste(wasteType);

            Destroy(gameObject);
            return;
        }

        if (rectTransform.anchoredPosition.y < -1400f)
            Destroy(gameObject);
    }

    private bool IsCaught()
    {
        Rect wasteRect = GetWorldRect(rectTransform);
        Rect zoneRect = GetWorldRect(catchZone);

        return wasteRect.Overlaps(zoneRect);
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        return new Rect(
            corners[0].x,
            corners[0].y,
            corners[2].x - corners[0].x,
            corners[2].y - corners[0].y
        );
    }
}