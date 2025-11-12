using UnityEngine;

public class TowerTargeting : MonoBehaviour
{
    [SerializeField] private float _range = 5f;
    [SerializeField] private float _updateInterval = 0.1f;
    [SerializeField] private bool _debugLog = true;

    private GameObject _currentTarget;
    private float _lastUpdateTime;
    private GameObject _previousTarget;
    private int _lastEnemyCount = 0;

    public GameObject CurrentTarget => _currentTarget;
    public float Range => _range;

    private void Update()
    {
        if (Time.time - _lastUpdateTime >= _updateInterval)
        {
            UpdateTarget();
            _lastUpdateTime = Time.time;
        }
    }

    private void UpdateTarget()
    {
        if (EnemyRegistry.Instance == null)
        {
            if (_debugLog && _currentTarget != null)
            {
                Debug.LogWarning($"[TowerTargeting] {gameObject.name}: EnemyRegistry is null!");
            }
            return;
        }

        var enemiesInRange = EnemyRegistry.Instance.GetEnemiesInRange(transform.position, _range);
        
        // Log only when enemy count changes to avoid spam
        if (_debugLog && enemiesInRange.Count != _lastEnemyCount)
        {
            if (enemiesInRange.Count > 0)
            {
                Debug.Log($"[TowerTargeting] {gameObject.name}: Found {enemiesInRange.Count} enemy(ies) in range ({_range}m)");
            }
            _lastEnemyCount = enemiesInRange.Count;
        }

        var newTarget = EnemyRegistry.Instance.GetNearestEnemy(transform.position, _range);
        
        // Log when target changes
        if (newTarget != _currentTarget)
        {
            if (_currentTarget != null && newTarget == null)
            {
                if (_debugLog)
                {
                    Debug.Log($"[TowerTargeting] {gameObject.name}: Lost target ({_previousTarget?.name})");
                }
            }
            else if (newTarget != null)
            {
                if (_debugLog)
                {
                    float distance = Vector3.Distance(transform.position, newTarget.transform.position);
                    Debug.Log($"[TowerTargeting] {gameObject.name}: Acquired new target: {newTarget.name} at distance {distance:F2}m");
                }
            }
            
            _previousTarget = _currentTarget;
            _currentTarget = newTarget;
        }
    }

    public void SetRange(float range)
    {
        _range = range;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw range circle
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _range);
        
        // Draw line to current target
        if (_currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _currentTarget.transform.position);
        }
    }
}

