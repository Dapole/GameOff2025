using System.Collections.Generic;
using UnityEngine;

public class EnemyRegistry : MonoBehaviour
{
    private static EnemyRegistry _instance;
    public static EnemyRegistry Instance => _instance;

    [SerializeField] private bool _debugLog = true;
    private HashSet<GameObject> _activeEnemies = new HashSet<GameObject>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Register(GameObject enemy)
    {
        if (enemy != null)
        {
            _activeEnemies.Add(enemy);
            if (_debugLog)
            {
                Debug.Log($"[EnemyRegistry] Registered enemy: {enemy.name}. Total active enemies: {_activeEnemies.Count}");
            }
        }
    }

    public void Unregister(GameObject enemy)
    {
        if (enemy != null && _activeEnemies.Remove(enemy))
        {
            if (_debugLog)
            {
                Debug.Log($"[EnemyRegistry] Unregistered enemy: {enemy.name}. Total active enemies: {_activeEnemies.Count}");
            }
        }
    }

    public List<GameObject> GetEnemiesInRange(Vector3 position, float range)
    {
        var result = new List<GameObject>();
        var rangeSqr = range * range;

        foreach (var enemy in _activeEnemies)
        {
            if (enemy == null || !enemy.activeInHierarchy)
            {
                continue;
            }

            var distanceSqr = (enemy.transform.position - position).sqrMagnitude;
            if (distanceSqr <= rangeSqr)
            {
                result.Add(enemy);
            }
        }

        return result;
    }

    public GameObject GetNearestEnemy(Vector3 position, float range)
    {
        GameObject nearest = null;
        float nearestDistanceSqr = range * range;

        foreach (var enemy in _activeEnemies)
        {
            if (enemy == null || !enemy.activeInHierarchy)
            {
                continue;
            }

            var distanceSqr = (enemy.transform.position - position).sqrMagnitude;
            if (distanceSqr <= nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearest = enemy;
            }
        }

        return nearest;
    }

    public int GetActiveEnemyCount()
    {
        // Clean up null references
        _activeEnemies.RemoveWhere(e => e == null);
        return _activeEnemies.Count;
    }

    public void Clear()
    {
        _activeEnemies.Clear();
    }
}

