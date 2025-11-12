using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHealth = 10;
    [SerializeField] private bool _debugLog = false;
    private int _currentHealth;

    public bool IsAlive => _currentHealth > 0;
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;

    private EnemyReward _reward;
    private EnemyController _enemyController;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _reward = GetComponent<EnemyReward>();
        _enemyController = GetComponent<EnemyController>();
    }

    private void OnEnable()
    {
        // Reset health when enemy is spawned from pool
        _currentHealth = _maxHealth;
    }

    public void Initialize(int health)
    {
        _maxHealth = health;
        _currentHealth = health;
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive)
        {
            if (_debugLog)
            {
                Debug.Log($"[EnemyHealth] {gameObject.name}: Already dead, ignoring damage");
            }
            return;
        }

        _currentHealth = Mathf.Max(0, _currentHealth - damage);

        if (_debugLog)
        {
            Debug.Log($"[EnemyHealth] {gameObject.name}: Took {damage} damage. Health: {_currentHealth}/{_maxHealth}");
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_debugLog)
        {
            Debug.Log($"[EnemyHealth] {gameObject.name}: Died!");
        }

        // Stop movement
        if (_enemyController != null)
        {
            var movement = GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.Stop();
            }
        }

        // Spawn corpse
        SpawnCorpse();

        int goldReward = _reward != null ? _reward.GoldReward : 0;

        EventBus.Publish(new EnemyDiedEvent
        {
            Enemy = gameObject,
            GoldReward = goldReward
        });

        EnemyRegistry.Instance.Unregister(gameObject);
        ObjectPoolService.Instance.Return(gameObject);
    }

    private void SpawnCorpse()
    {
        var enemyController = GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.SpawnCorpse();
        }
    }
}

