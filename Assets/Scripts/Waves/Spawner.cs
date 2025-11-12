using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private EnemyPath _path;

    private void Awake()
    {
        FindPath();
    }

    private void Start()
    {
        // Double-check in Start in case path was added after Awake
        if (_path == null)
        {
            FindPath();
        }
    }

    private void FindPath()
    {
        if (_path == null)
        {
            _path = FindFirstObjectByType<EnemyPath>();
            if (_path == null)
            {
                Debug.LogError($"Spawner on {gameObject.name}: EnemyPath not found in scene! Please add an EnemyPath component to a GameObject in the scene.");
            }
            else
            {
                Debug.Log($"Spawner found EnemyPath on {_path.gameObject.name}");
            }
        }
    }

    public GameObject SpawnEnemy(GameObject enemyPrefab)
    {
        // Try to find path one more time if it's still null
        if (_path == null)
        {
            FindPath();
        }

        if (_path == null)
        {
            Debug.LogError($"Spawner on {gameObject.name}: EnemyPath is null! Cannot spawn enemy.");
            return null;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError($"Spawner on {gameObject.name}: EnemyPrefab is null! Please assign an enemy prefab in WaveConfig.");
            return null;
        }

        var spawnPosition = _path.GetStartPosition();
        var enemy = ObjectPoolService.Instance.Get(enemyPrefab);
        enemy.transform.position = spawnPosition;

        var enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.Initialize(_path);
        }

        EnemyRegistry.Instance.Register(enemy);
        EventBus.Publish(new EnemySpawnedEvent { Enemy = enemy });

        return enemy;
    }
}

