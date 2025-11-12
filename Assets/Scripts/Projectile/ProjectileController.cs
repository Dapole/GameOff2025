using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Vector3 _targetPosition;
    private GameObject _target; // Optional: track target for homing
    private float _speed;
    private int _damage;
    private bool _isInitialized = false;
    private float _maxTravelDistance = 100f;
    private Vector3 _startPosition;
    [SerializeField] private bool _useHoming = false; // Enable homing correction
    [SerializeField] private float _homingRange = 5f; // Only correct course if target is within this range
    [SerializeField] private float _homingStrength = 0.5f; // How much to correct (0-1)

    public void Initialize(Vector3 targetPosition, float speed, int damage, GameObject target = null)
    {
        _targetPosition = targetPosition;
        _target = target;
        _speed = speed;
        _damage = damage;
        _startPosition = transform.position;
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized)
            return;

        float distanceTraveled = Vector3.Distance(transform.position, _startPosition);
        if (distanceTraveled >= _maxTravelDistance)
        {
            ReturnToPool();
            return;
        }

        // Optional homing: if target is close and still alive, slightly adjust course
        Vector3 targetPos = _targetPosition;
        if (_useHoming && _target != null && _target.activeInHierarchy)
        {
            float distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
            if (distanceToTarget < _homingRange)
            {
                // Blend between predicted position and actual target position
                targetPos = Vector3.Lerp(_targetPosition, _target.transform.position, _homingStrength);
            }
        }

        Vector3 direction = (targetPos - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPos);

        // Check if we're close enough to hit (check actual target if we have one)
        if (_target != null && _target.activeInHierarchy)
        {
            float distanceToActualTarget = Vector3.Distance(transform.position, _target.transform.position);
            if (distanceToActualTarget < 0.8f) // Hit detection radius (increased for better hit detection)
            {
                HitTarget();
                return;
            }
        }
        else if (distance < 0.2f) // Hit predicted position if no target
        {
            HitTarget();
            return;
        }

        transform.position += direction * _speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void HitTarget()
    {
        // Prioritize hitting the actual target if we have one
        if (_target != null && _target.activeInHierarchy)
        {
            var damageable = _target.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                damageable.TakeDamage(_damage);
                ReturnToPool();
                return;
            }
        }

        // Check for enemies in range (3D)
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (var collider in colliders)
        {
            var damageable = collider.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                damageable.TakeDamage(_damage);
                ReturnToPool();
                return;
            }
        }

        // Check for enemies in range (2D)
        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (var collider in colliders2D)
        {
            var damageable = collider.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                damageable.TakeDamage(_damage);
                ReturnToPool();
                return;
            }
        }

        ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        
        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null && damageable.IsAlive)
        {
            damageable.TakeDamage(_damage);
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;
        
        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null && damageable.IsAlive)
        {
            damageable.TakeDamage(_damage);
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (ObjectPoolService.Instance != null)
        {
            ObjectPoolService.Instance.Return(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

