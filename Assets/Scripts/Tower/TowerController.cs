using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] private TowerConfig _config;
    [SerializeField] private TowerTargeting _targeting;
    [SerializeField] private TowerWeapon _weapon;
    [SerializeField] private bool _debugLog = true;

    private float _lastFireTime;
    private float _fireRate;

    private void Awake()
    {
        if (_targeting == null)
        {
            _targeting = GetComponent<TowerTargeting>();
            if (_targeting == null && _debugLog)
            {
                Debug.LogWarning($"[TowerController] {gameObject.name}: TowerTargeting component not found!");
            }
        }

        if (_weapon == null)
        {
            _weapon = GetComponent<TowerWeapon>();
            if (_weapon == null && _debugLog)
            {
                Debug.LogWarning($"[TowerController] {gameObject.name}: TowerWeapon component not found!");
            }
        }
    }

    private void Start()
    {
        if (_config != null)
        {
            Initialize(_config);
        }
        else if (_debugLog)
        {
            Debug.LogWarning($"[TowerController] {gameObject.name}: TowerConfig is not assigned!");
        }
    }

    public void Initialize(TowerConfig config)
    {
        _config = config;
        _fireRate = config.FireRate;

        if (_debugLog)
        {
            Debug.Log($"[TowerController] {gameObject.name}: Initialized with Range={config.Range}, FireRate={config.FireRate}, Damage={config.Damage}");
        }

        if (_targeting != null)
        {
            _targeting.SetRange(config.Range);
        }
        else if (_debugLog)
        {
            Debug.LogError($"[TowerController] {gameObject.name}: Cannot set range - TowerTargeting is null!");
        }

        if (_weapon != null)
        {
            // Only set ProjectilePrefab from config if it's not null
            // This allows setting prefab directly in TowerWeapon component as fallback
            if (config.ProjectilePrefab != null)
            {
                _weapon.SetProjectilePrefab(config.ProjectilePrefab);
                if (_debugLog)
                {
                    Debug.Log($"[TowerController] {gameObject.name}: ProjectilePrefab set from TowerConfig: {config.ProjectilePrefab.name}");
                }
            }
            else
            {
                // Check if TowerWeapon already has a prefab set (from Inspector)
                var currentPrefab = _weapon.GetProjectilePrefab();
                if (currentPrefab == null && _debugLog)
                {
                    Debug.LogWarning($"[TowerController] {gameObject.name}: ProjectilePrefab is null in TowerConfig AND TowerWeapon! Please assign it in one of these places.");
                }
                else if (_debugLog && currentPrefab != null)
                {
                    Debug.Log($"[TowerController] {gameObject.name}: Using ProjectilePrefab from TowerWeapon component: {currentPrefab.name} (TowerConfig has none)");
                }
            }
            
            _weapon.SetDamage(config.Damage);
            _weapon.SetProjectileSpeed(config.ProjectileSpeed);
            
            // Final check - verify that weapon has a prefab after initialization
            var finalPrefab = _weapon.GetProjectilePrefab();
            if (finalPrefab == null && _debugLog)
            {
                Debug.LogError($"[TowerController] {gameObject.name}: CRITICAL - ProjectilePrefab is still null after initialization! " +
                    $"Please assign ProjectilePrefab in either TowerConfig '{config.name}' or in TowerWeapon component.");
            }
            else if (finalPrefab != null && _debugLog)
            {
                Debug.Log($"[TowerController] {gameObject.name}: Weapon ready - ProjectilePrefab: {finalPrefab.name}, Damage: {config.Damage}, Speed: {config.ProjectileSpeed}");
            }
        }
        else if (_debugLog)
        {
            Debug.LogError($"[TowerController] {gameObject.name}: Cannot set weapon - TowerWeapon is null!");
        }
    }

    private void Update()
    {
        if (_targeting == null || _weapon == null)
        {
            if (_debugLog && Time.frameCount % 60 == 0) // Log every 60 frames to avoid spam
            {
                Debug.LogWarning($"[TowerController] {gameObject.name}: Targeting={_targeting != null}, Weapon={_weapon != null}");
            }
            return;
        }

        var target = _targeting.CurrentTarget;
        
        if (target != null)
        {
            float timeSinceLastFire = Time.time - _lastFireTime;
            float fireInterval = 1f / _fireRate;
            
            if (timeSinceLastFire >= fireInterval)
            {
                if (_debugLog)
                {
                    float distance = Vector3.Distance(transform.position, target.transform.position);
                    Debug.Log($"[TowerController] {gameObject.name}: Firing at target {target.name} at distance {distance:F2}m");
                }
                
                // Pass GameObject target for prediction, not just position
                _weapon.Fire(target);
                _lastFireTime = Time.time;
            }
        }
    }
}

