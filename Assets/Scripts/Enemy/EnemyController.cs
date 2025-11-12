using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject _corpsePrefab;
    
    private EnemyMovement _movement;
    private EnemyHealth _health;
    private EnemyPath _path;

    private void Awake()
    {
        _movement = GetComponent<EnemyMovement>();
        _health = GetComponent<EnemyHealth>();
    }

    public void Initialize(EnemyPath path)
    {
        _path = path;
        if (_path != null && _movement != null)
        {
            _movement.Initialize(_path.GetWaypoints());
        }
    }

    public void ReachBase()
    {
        if (_movement != null)
        {
            _movement.Stop();
        }
        EnemyRegistry.Instance.Unregister(gameObject);
        ObjectPoolService.Instance.Return(gameObject);
    }

    public void SpawnCorpse()
    {
        if (_corpsePrefab != null)
        {
            // Spawn corpse at enemy position
            GameObject corpse = Instantiate(_corpsePrefab, transform.position, transform.rotation);
            
            // Register corpse for cleanup
            CorpseManager.Instance?.RegisterCorpse(corpse);
        }
    }
}

