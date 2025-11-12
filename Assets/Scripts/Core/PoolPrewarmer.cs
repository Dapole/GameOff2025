using UnityEngine;

public class PoolPrewarmer : MonoBehaviour
{
    [Header("Prefabs to Prewarm")]
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private int _enemyPoolSize = 20;

    [SerializeField] private GameObject[] _projectilePrefabs;
    [SerializeField] private int _projectilePoolSize = 50;

    private void Start()
    {
        PrewarmPools();
    }

    public void PrewarmPools()
    {
        if (ObjectPoolService.Instance == null)
        {
            Debug.LogError("ObjectPoolService not found! Make sure it exists in the scene.");
            return;
        }

        // Prewarm enemies
        if (_enemyPrefabs != null)
        {
            foreach (var prefab in _enemyPrefabs)
            {
                if (prefab != null)
                {
                    ObjectPoolService.Instance.PrewarmPool(prefab, _enemyPoolSize);
                }
            }
        }

        // Prewarm projectiles
        if (_projectilePrefabs != null)
        {
            foreach (var prefab in _projectilePrefabs)
            {
                if (prefab != null)
                {
                    ObjectPoolService.Instance.PrewarmPool(prefab, _projectilePoolSize);
                }
            }
        }
    }
}


