using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolService : MonoBehaviour
{
    private static ObjectPoolService _instance;
    public static ObjectPoolService Instance => _instance;

    private Dictionary<GameObject, Queue<GameObject>> _pools = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<GameObject, GameObject> _prefabMap = new Dictionary<GameObject, GameObject>();

    [SerializeField] private Transform _poolParent;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            if (_poolParent == null)
            {
                _poolParent = new GameObject("PoolParent").transform;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PrewarmPool(GameObject prefab, int count)
    {
        if (!_pools.ContainsKey(prefab))
        {
            _pools[prefab] = new Queue<GameObject>();
        }

        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(prefab, _poolParent);
            obj.SetActive(false);
            _pools[prefab].Enqueue(obj);
            _prefabMap[obj] = prefab;
        }
    }

    public GameObject Get(GameObject prefab)
    {
        if (!_pools.ContainsKey(prefab))
        {
            _pools[prefab] = new Queue<GameObject>();
        }

        GameObject obj;
        if (_pools[prefab].Count > 0)
        {
            obj = _pools[prefab].Dequeue();
        }
        else
        {
            obj = Instantiate(prefab, _poolParent);
            _prefabMap[obj] = prefab;
        }

        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        if (_prefabMap.TryGetValue(obj, out var prefab))
        {
            obj.SetActive(false);
            obj.transform.SetParent(_poolParent);
            _pools[prefab].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }

    public void ClearPool(GameObject prefab)
    {
        if (_pools.ContainsKey(prefab))
        {
            while (_pools[prefab].Count > 0)
            {
                var obj = _pools[prefab].Dequeue();
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            _pools.Remove(prefab);
        }
    }
}

