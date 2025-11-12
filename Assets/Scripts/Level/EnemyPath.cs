using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    [SerializeField] private List<Transform> _waypoints = new List<Transform>();

    public List<Vector3> GetWaypoints()
    {
        var points = new List<Vector3>();
        foreach (var waypoint in _waypoints)
        {
            if (waypoint != null)
            {
                points.Add(waypoint.position);
            }
        }
        return points;
    }

    public Vector3 GetStartPosition()
    {
        if (_waypoints.Count > 0 && _waypoints[0] != null)
        {
            return _waypoints[0].position;
        }
        return transform.position;
    }

    public Vector3 GetEndPosition()
    {
        if (_waypoints.Count > 0 && _waypoints[_waypoints.Count - 1] != null)
        {
            return _waypoints[_waypoints.Count - 1].position;
        }
        return transform.position;
    }

    private void OnDrawGizmos()
    {
        if (_waypoints == null || _waypoints.Count < 2)
            return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < _waypoints.Count - 1; i++)
        {
            if (_waypoints[i] != null && _waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(_waypoints[i].position, _waypoints[i + 1].position);
            }
        }

        Gizmos.color = Color.green;
        foreach (var waypoint in _waypoints)
        {
            if (waypoint != null)
            {
                Gizmos.DrawSphere(waypoint.position, 0.3f);
            }
        }
    }
}


