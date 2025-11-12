using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 2f;
    private List<Vector3> _waypoints;
    private int _currentWaypointIndex = 0;
    private bool _isMoving = false;

    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    public Vector3 GetVelocity()
    {
        if (!_isMoving || _waypoints == null || _waypoints.Count == 0 || _currentWaypointIndex >= _waypoints.Count)
            return Vector3.zero;

        Vector3 target = _waypoints[_currentWaypointIndex];
        Vector3 direction = (target - transform.position).normalized;
        return direction * _speed;
    }

    public Vector3 GetDirection()
    {
        if (!_isMoving || _waypoints == null || _waypoints.Count == 0 || _currentWaypointIndex >= _waypoints.Count)
            return Vector3.zero;

        Vector3 target = _waypoints[_currentWaypointIndex];
        return (target - transform.position).normalized;
    }

    public void Initialize(List<Vector3> waypoints)
    {
        _waypoints = waypoints;
        _currentWaypointIndex = 0;
        _isMoving = waypoints != null && waypoints.Count > 0;
    }

    private void Update()
    {
        if (!_isMoving || _waypoints == null || _waypoints.Count == 0)
            return;

        if (_currentWaypointIndex >= _waypoints.Count)
        {
            _isMoving = false;
            return;
        }

        Vector3 target = _waypoints[_currentWaypointIndex];
        Vector3 direction = (target - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target);

        if (distance < 0.1f)
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= _waypoints.Count)
            {
                _isMoving = false;
                return;
            }
            target = _waypoints[_currentWaypointIndex];
            direction = (target - transform.position).normalized;
        }

        transform.position += direction * _speed * Time.deltaTime;

        // Rotate towards movement direction
        if (direction != Vector3.zero)
        {
            // For 3D
            transform.rotation = Quaternion.LookRotation(direction);
            
            // For 2D (if using SpriteRenderer, uncomment this instead)
            // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void Stop()
    {
        _isMoving = false;
    }
}

