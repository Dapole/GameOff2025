using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [Header("Services")]
    [SerializeField] private ObjectPoolService _objectPoolService;
    [SerializeField] private EnemyRegistry _enemyRegistry;
    [SerializeField] private CorpseManager _corpseManager;

    [Header("Managers")]
    [SerializeField] private LifeManager _lifeManager;
    [SerializeField] private EconomyManager _economyManager;
    [SerializeField] private WaveManager _waveManager;

    [Header("UI")]
    [SerializeField] private HUDController _hudController;

    private void Awake()
    {
        InitializeServices();
        InitializeManagers();
        ConnectSystems();
    }

    private void InitializeServices()
    {
        if (_objectPoolService == null)
        {
            _objectPoolService = FindFirstObjectByType<ObjectPoolService>();
            if (_objectPoolService == null)
            {
                var go = new GameObject("ObjectPoolService");
                _objectPoolService = go.AddComponent<ObjectPoolService>();
            }
        }

        if (_enemyRegistry == null)
        {
            _enemyRegistry = FindFirstObjectByType<EnemyRegistry>();
            if (_enemyRegistry == null)
            {
                var go = new GameObject("EnemyRegistry");
                _enemyRegistry = go.AddComponent<EnemyRegistry>();
            }
        }

        if (_corpseManager == null)
        {
            _corpseManager = FindFirstObjectByType<CorpseManager>();
            if (_corpseManager == null)
            {
                var go = new GameObject("CorpseManager");
                _corpseManager = go.AddComponent<CorpseManager>();
            }
        }
    }

    private void InitializeManagers()
    {
        if (_lifeManager == null)
        {
            _lifeManager = FindFirstObjectByType<LifeManager>();
        }

        if (_economyManager == null)
        {
            _economyManager = FindFirstObjectByType<EconomyManager>();
        }

        if (_waveManager == null)
        {
            _waveManager = FindFirstObjectByType<WaveManager>();
        }

        // Initialize managers with level config if available
        // LevelConfig is a ScriptableObject, so we need to find it differently
        var configs = Resources.FindObjectsOfTypeAll<LevelConfig>();
        LevelConfig levelConfig = configs.Length > 0 ? configs[0] : null;

        if (levelConfig != null)
        {
            if (_lifeManager != null)
            {
                _lifeManager.Initialize(levelConfig.StartingLives);
            }

            if (_economyManager != null)
            {
                _economyManager.Initialize(levelConfig.StartingGold);
            }
        }
    }

    private void ConnectSystems()
    {
        // Subscribe to events for cross-system communication
        EventBus.Subscribe<EnemyDiedEvent>(OnEnemyDied);
        EventBus.Subscribe<EnemyReachedBaseEvent>(OnEnemyReachedBase);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
    }

    private void OnEnemyDied(EnemyDiedEvent evt)
    {
        if (evt.GoldReward > 0)
        {
            _economyManager?.AddGold(evt.GoldReward);
        }
    }

    private void OnEnemyReachedBase(EnemyReachedBaseEvent evt)
    {
        _lifeManager?.TakeDamage(evt.Damage);
    }

    private void OnGameOver(GameOverEvent evt)
    {
        // Game over handling
        Debug.Log($"Game Over! Victory: {evt.IsVictory}");
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);
        EventBus.Unsubscribe<EnemyReachedBaseEvent>(OnEnemyReachedBase);
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
    }
}

