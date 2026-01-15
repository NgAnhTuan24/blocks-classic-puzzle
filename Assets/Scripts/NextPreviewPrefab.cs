using System.Collections.Generic;
using UnityEngine;

public class NextPreviewPrefab : MonoBehaviour
{
    [System.Serializable]
    public struct TetrominoPrefab
    {
        public Tetromino tetromino;
        public GameObject prefab;
    }

    public Transform spawnPoint;
    public TetrominoPrefab[] prefabs;

    private GameObject currentPreview;
    private Dictionary<Tetromino, GameObject> prefabMap;

    private void Awake()
    {
        prefabMap = new Dictionary<Tetromino, GameObject>();
        foreach (var item in prefabs)
        {
            prefabMap[item.tetromino] = item.prefab;
        }
    }

    public void SetNext(TetrominoData data)
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }

        if (prefabMap.TryGetValue(data.tetromino, out GameObject prefab))
        {
            currentPreview = Instantiate(prefab, spawnPoint.position, Quaternion.identity, spawnPoint);
        }
    }

    public void Clear()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }
    }
}
