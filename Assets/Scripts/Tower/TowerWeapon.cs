using UnityEngine;

public class TowerWeapon : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileSpeed = 10f;
    [SerializeField] private int _damage = 10;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private bool _debugLog = true;

    private void Awake()
    {
        if (_firePoint == null)
        {
            _firePoint = transform;
        }
    }

    public void Fire(Vector3 targetPosition)
    {
        Fire(targetPosition, null);
    }

    public void Fire(GameObject target)
    {
        if (target != null)
        {
            Fire(target.transform.position, target);
        }
        else
        {
            Debug.LogWarning($"[TowerWeapon] {gameObject.name}: Fire called with null target!");
        }
    }

    private void Fire(Vector3 targetPosition, GameObject target)
    {
        if (_projectilePrefab == null)
        {
            if (_debugLog)
            {
                Debug.LogError($"[TowerWeapon] {gameObject.name}: Cannot fire - ProjectilePrefab is null!");
            }
            return;
        }

        if (ObjectPoolService.Instance == null)
        {
            if (_debugLog)
            {
                Debug.LogError($"[TowerWeapon] {gameObject.name}: Cannot fire - ObjectPoolService.Instance is null!");
            }
            return;
        }

        var projectile = ObjectPoolService.Instance.Get(_projectilePrefab);
        if (projectile == null)
        {
            if (_debugLog)
            {
                Debug.LogError($"[TowerWeapon] {gameObject.name}: Failed to get projectile from pool!");
            }
            return;
        }

        projectile.transform.position = _firePoint.position;

        var projectileController = projectile.GetComponent<ProjectileController>();
        if (projectileController != null)
        {
            // Calculate predicted position if we have a target with movement
            Vector3 predictedPosition = targetPosition;
            if (target != null)
            {
                var enemyMovement = target.GetComponent<EnemyMovement>();
                if (enemyMovement != null)
                {
                    predictedPosition = CalculatePredictedPosition(target.transform.position, enemyMovement, _firePoint.position, _projectileSpeed);
                }
            }

            projectileController.Initialize(predictedPosition, _projectileSpeed, _damage, target);
            
            if (_debugLog)
            {
                float distance = Vector3.Distance(_firePoint.position, predictedPosition);
                string targetInfo = target != null ? $" (target: {target.name})" : "";
                Debug.Log($"[TowerWeapon] {gameObject.name}: Projectile fired towards predicted position {predictedPosition} (distance: {distance:F2}m, damage: {_damage}, speed: {_projectileSpeed}){targetInfo}");
            }
        }
        else if (_debugLog)
        {
            Debug.LogWarning($"[TowerWeapon] {gameObject.name}: Projectile {projectile.name} doesn't have ProjectileController component!");
        }
    }

    private Vector3 CalculatePredictedPosition(Vector3 targetPosition, EnemyMovement enemyMovement, Vector3 firePosition, float projectileSpeed)
    {
        Vector3 enemyVelocity = enemyMovement.GetVelocity();
        
        // If enemy is not moving, just aim at current position
        if (enemyVelocity.magnitude < 0.01f)
        {
            return targetPosition;
        }

        // Calculate time to reach target (simplified prediction)
        Vector3 toTarget = targetPosition - firePosition;
        float distance = toTarget.magnitude;
        
        // Solve quadratic equation for interception time
        // Based on: distance = projectileSpeed * time
        // And: predictedPos = targetPos + enemyVelocity * time
        // We need to find time where projectile reaches predicted position
        
        float timeToIntercept = distance / projectileSpeed;
        
        // Simple prediction: assume enemy continues at current velocity
        Vector3 predictedPosition = targetPosition + enemyVelocity * timeToIntercept;
        
        // Refine prediction (one iteration for better accuracy)
        float refinedDistance = Vector3.Distance(firePosition, predictedPosition);
        float refinedTime = refinedDistance / projectileSpeed;
        predictedPosition = targetPosition + enemyVelocity * refinedTime;
        
        return predictedPosition;
    }

    public void SetProjectilePrefab(GameObject prefab)
    {
        _projectilePrefab = prefab;
    }

    public void SetDamage(int damage)
    {
        _damage = damage;
    }

    public void SetProjectileSpeed(float speed)
    {
        _projectileSpeed = speed;
    }

    // Getter for checking current prefab without exposing private field
    public GameObject GetProjectilePrefab()
    {
        return _projectilePrefab;
    }
}

