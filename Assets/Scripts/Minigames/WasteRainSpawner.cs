using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteRainSpawner : MonoBehaviour
{
    [System.Serializable]
    public class FallingWastePrefab
    {
        public GameObject prefab;
        public CatcherGameManager.WasteType wasteType;
    }

    public List<FallingWastePrefab> wastePrefabs = new List<FallingWastePrefab>();

    public RectTransform spawnArea;
    public float spawnInterval = 1f;
    public float minXPadding = 80f;

    private CatcherGameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<CatcherGameManager>();
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnOne()
    {
        if (spawnArea == null || wastePrefabs.Count == 0 || gameManager == null)
            return;

        CatcherGameManager.WasteType target = gameManager.CurrentTargetType;

        List<FallingWastePrefab> preferred = new List<FallingWastePrefab>();
        List<FallingWastePrefab> other = new List<FallingWastePrefab>();

        foreach (var item in wastePrefabs)
        {
            if (item.wasteType == target)
                preferred.Add(item);
            else
                other.Add(item);
        }

        FallingWastePrefab selected;

        bool spawnPreferred = Random.value < 0.65f;

        if (spawnPreferred && preferred.Count > 0)
            selected = preferred[Random.Range(0, preferred.Count)];
        else if (other.Count > 0)
            selected = other[Random.Range(0, other.Count)];
        else
            return;

        GameObject obj = Instantiate(selected.prefab, spawnArea.parent);
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt == null) return;

        // Forzar configuración UI consistente
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        float halfWidth = spawnArea.rect.width * 0.5f;
        float x = Random.Range(-halfWidth + minXPadding, halfWidth - minXPadding);

        rt.SetParent(spawnArea.parent, false);
        rt.anchoredPosition = new Vector2(x, spawnArea.anchoredPosition.y);
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;

        FallingWaste falling = obj.GetComponent<FallingWaste>();
        if (falling != null)
            falling.wasteType = selected.wasteType;
    }
}