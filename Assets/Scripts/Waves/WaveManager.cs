using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private WaveConfig[] _waves;
    [SerializeField] private Spawner _spawner;
    [SerializeField] private LevelConfig _levelConfig;

    private int _currentWaveIndex = 0;
    private bool _isWaveActive = false;
    private int _enemiesRemaining = 0;

    public int CurrentWaveNumber => _currentWaveIndex + 1;
    public int TotalWaves => _waves != null ? _waves.Length : 0;
    public bool IsWaveActive => _isWaveActive;

    private void Start()
    {
        if (_spawner == null)
        {
            _spawner = FindFirstObjectByType<Spawner>();
        }

        EventBus.Subscribe<EnemyDiedEvent>(OnEnemyDied);
    }

    public void StartWave()
    {
        if (_isWaveActive)
        {
            Debug.LogWarning("Wave is already active!");
            return;
        }

        // Validate setup
        if (_waves == null || _waves.Length == 0)
        {
            Debug.LogError($"WaveManager on {gameObject.name}: No waves configured! Please assign WaveConfig array.");
            return;
        }

        if (_spawner == null)
        {
            _spawner = FindFirstObjectByType<Spawner>();
            if (_spawner == null)
            {
                Debug.LogError($"WaveManager on {gameObject.name}: Spawner not found! Please add a Spawner component to the scene.");
                return;
            }
        }

        if (_currentWaveIndex >= _waves.Length)
        {
            Debug.Log("All waves completed!");
            EventBus.Publish(new GameOverEvent { IsVictory = true });
            return;
        }

        var wave = _waves[_currentWaveIndex];
        if (wave == null)
        {
            Debug.LogError($"WaveManager on {gameObject.name}: Wave at index {_currentWaveIndex} is null! Please check WaveConfig array.");
            return;
        }

        if (wave.EnemyPrefab == null)
        {
            Debug.LogError($"WaveManager on {gameObject.name}: EnemyPrefab in WaveConfig (wave {_currentWaveIndex + 1}) is null! Please assign an enemy prefab.");
            return;
        }

        StartCoroutine(StartWaveCoroutine());
    }

    private IEnumerator StartWaveCoroutine()
    {
        _isWaveActive = true;
        var wave = _waves[_currentWaveIndex];

        if (wave.DelayBeforeWave > 0)
        {
            yield return new WaitForSeconds(wave.DelayBeforeWave);
        }

        EventBus.Publish(new WaveStartedEvent { WaveNumber = CurrentWaveNumber });

        _enemiesRemaining = wave.EnemyCount;

        for (int i = 0; i < wave.EnemyCount; i++)
        {
            if (_spawner != null && wave.EnemyPrefab != null)
            {
                _spawner.SpawnEnemy(wave.EnemyPrefab);
            }
            else
            {
                Debug.LogError($"Cannot spawn enemy: Spawner={_spawner != null}, EnemyPrefab={wave.EnemyPrefab != null}");
            }

            if (i < wave.EnemyCount - 1)
            {
                yield return new WaitForSeconds(wave.SpawnInterval);
            }
        }
    }

    private void OnEnemyDied(EnemyDiedEvent evt)
    {
        if (!_isWaveActive)
            return;

        _enemiesRemaining--;
        if (_enemiesRemaining <= 0)
        {
            CompleteWave();
        }
    }

    private void CompleteWave()
    {
        _isWaveActive = false;
        EventBus.Publish(new WaveCompletedEvent { WaveNumber = CurrentWaveNumber });
        
        // Notify systems that wave is completed and cleanup is needed
        EventBus.Publish(new WaveCompletedCleanupEvent());
        
        _currentWaveIndex++;

        if (_currentWaveIndex >= _waves.Length)
        {
            EventBus.Publish(new GameOverEvent { IsVictory = true });
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);
    }
}

