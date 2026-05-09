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
        float padding = 80f;

        for (int i = 0; i < 60; i++)
        {
            float x = Random.Range(
                -playArea.rect.width * 0.5f + padding,
                playArea.rect.width * 0.5f - padding
            );

            float y = Random.Range(
                -playArea.rect.height * 0.5f + padding,
                playArea.rect.height * 0.5f - padding
            );

            Vector2 candidate = new(x, y);

            bool overlaps = false;
            foreach (Vector2 used in usedPositions)
            {
                if (Vector2.Distance(candidate, used) < 130f)
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
            {
                usedPositions.Add(candidate);
                return candidate;
            }
        }

        return Vector2.zero;
    }
}