using System.Collections.Generic;
using UnityEngine;

public class WasteSpawner : MonoBehaviour
{
    [System.Serializable]
    public class WastePrefab
    {
        public GameObject prefab;
        public SortingGameManager.WasteType type;
    }

    [Header("Waste Prefabs")]
    public List<WastePrefab> wastePrefabs = new List<WastePrefab>();

    [Header("Spawn Settings")]
    public int minSpawn = 4;
    public int maxSpawn = 8;
    public RectTransform playArea;

    [Header("Distribution")]
    public float paddingX = 120f;
    public float paddingTop = 120f;
    public float paddingBottom = 210f;
    public float minDistanceBetweenWaste = 170f;
    public int maxAttemptsPerWaste = 100;

    private SortingGameManager gameManager;
    private readonly List<Vector2> usedPositions = new();

    private void Start()
    {
        gameManager = FindFirstObjectByType<SortingGameManager>();
        SpawnWaste();
    }

    private void SpawnWaste()
    {
        if (playArea == null)
        {
            Debug.LogError("WasteSpawner: falta asignar PlayArea.");
            return;
        }

        if (wastePrefabs == null || wastePrefabs.Count == 0)
        {
            Debug.LogError("WasteSpawner: no hay prefabs asignados.");
            return;
        }

        usedPositions.Clear();

        int amount = Random.Range(minSpawn, maxSpawn + 1);
        int spawnedCount = 0;

        for (int i = 0; i < amount; i++)
        {
            WastePrefab data = wastePrefabs[Random.Range(0, wastePrefabs.Count)];

            if (data == null || data.prefab == null)
                continue;

            GameObject obj = Instantiate(data.prefab, playArea);

            RectTransform rt = obj.GetComponent<RectTransform>();
            if (rt == null)
            {
                Debug.LogWarning($"El prefab {data.prefab.name} no tiene RectTransform.");
                Destroy(obj);
                continue;
            }

            DraggableWasteItem item = obj.GetComponent<DraggableWasteItem>();
            if (item == null)
            {
                Debug.LogWarning($"El prefab {data.prefab.name} no tiene DraggableWasteItem.");
                Destroy(obj);
                continue;
            }

            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            rt.anchoredPosition = GetRandomPosition();
            rt.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-12f, 12f));
            rt.localScale = Vector3.one;

            item.wasteType = data.type;
            item.Initialize(gameManager);

            spawnedCount++;
        }

        if (gameManager != null)
            gameManager.RegisterSpawnedWaste(spawnedCount);

        Debug.Log("Residuos generados: " + spawnedCount);
    }

    private Vector2 GetRandomPosition()
    {
        float minX = -playArea.rect.width * 0.5f + paddingX;
        float maxX = playArea.rect.width * 0.5f - paddingX;

        float minY = -playArea.rect.height * 0.5f + paddingBottom;
        float maxY = playArea.rect.height * 0.5f - paddingTop;

        for (int i = 0; i < maxAttemptsPerWaste; i++)
        {
            Vector2 candidate = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            if (IsFarEnough(candidate))
            {
                usedPositions.Add(candidate);
                return candidate;
            }
        }

        return GetFallbackPosition(minX, maxX, minY, maxY);
    }

    private bool IsFarEnough(Vector2 candidate)
    {
        foreach (Vector2 used in usedPositions)
        {
            if (Vector2.Distance(candidate, used) < minDistanceBetweenWaste)
                return false;
        }

        return true;
    }

    private Vector2 GetFallbackPosition(float minX, float maxX, float minY, float maxY)
    {
        int index = usedPositions.Count;

        int columns = 4;
        float spacingX = (maxX - minX) / Mathf.Max(1, columns - 1);
        float spacingY = minDistanceBetweenWaste;

        int col = index % columns;
        int row = index / columns;

        float x = minX + col * spacingX;
        float y = maxY - row * spacingY;

        y = Mathf.Clamp(y, minY, maxY);

        Vector2 fallback = new Vector2(x, y);
        usedPositions.Add(fallback);

        return fallback;
    }
}