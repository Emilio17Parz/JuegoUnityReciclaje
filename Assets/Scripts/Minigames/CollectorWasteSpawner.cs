using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorWasteSpawner : MonoBehaviour
{
    [System.Serializable]
    public class WastePrefab
    {
        public GameObject prefab;
        public CollectorGameManager.WasteType type;
    }

    [Header("Waste Prefabs")]
    public List<WastePrefab> wastePrefabs = new List<WastePrefab>();

    [Header("Spawn Area")]
    public RectTransform playArea;
    public RectTransform wasteContainer;

    [Header("Round Settings")]
    public int startAmount = 3;
    public int amountIncreasePerRound = 1;
    public int maxAmount = 8;
    public float minDistanceBetweenWaste = 120f;

    private readonly List<Vector2> usedPositions = new List<Vector2>();

    public void SpawnRound(int roundNumber)
    {
        ClearOldWaste();

        usedPositions.Clear();

        int amount = startAmount + ((roundNumber - 1) * amountIncreasePerRound);
        amount = Mathf.Clamp(amount, startAmount, maxAmount);

        int spawned = 0;

        for (int i = 0; i < amount; i++)
        {
            if (wastePrefabs == null || wastePrefabs.Count == 0)
                return;

            WastePrefab data = wastePrefabs[Random.Range(0, wastePrefabs.Count)];

            if (data == null || data.prefab == null)
                continue;

            GameObject obj = Instantiate(data.prefab, wasteContainer);

            RectTransform rt = obj.GetComponent<RectTransform>();
            if (rt == null)
            {
                Destroy(obj);
                continue;
            }

            CollectibleWaste collectible = obj.GetComponent<CollectibleWaste>();
            if (collectible == null)
                collectible = obj.AddComponent<CollectibleWaste>();

            collectible.wasteType = data.type;

            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-10f, 10f));
            rt.anchoredPosition = GetRandomPosition();

            spawned++;
        }

        CollectorGameManager manager = FindFirstObjectByType<CollectorGameManager>();
        if (manager != null)
            manager.RegisterRoundWaste(spawned);
    }

    private Vector2 GetRandomPosition()
    {
        float padding = 90f;

        for (int i = 0; i < 80; i++)
        {
            float x = Random.Range(
                -playArea.rect.width / 2 + padding,
                playArea.rect.width / 2 - padding
            );

            float y = Random.Range(
                -playArea.rect.height / 2 + padding + 120f,
                playArea.rect.height / 2 - padding
            );

            Vector2 candidate = new Vector2(x, y);

            bool tooClose = false;

            foreach (Vector2 used in usedPositions)
            {
                if (Vector2.Distance(candidate, used) < minDistanceBetweenWaste)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                usedPositions.Add(candidate);
                return candidate;
            }
        }

        return Vector2.zero;
    }

    private void ClearOldWaste()
    {
        if (wasteContainer == null) return;

        foreach (Transform child in wasteContainer)
            Destroy(child.gameObject);
    }
}