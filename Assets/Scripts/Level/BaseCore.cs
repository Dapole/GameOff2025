using UnityEngine;

public class BaseCore : MonoBehaviour
{
    [SerializeField] private int _damagePerEnemy = 1;

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            EventBus.Publish(new EnemyReachedBaseEvent
            {
                Damage = _damagePerEnemy
            });
            enemy.ReachBase();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            EventBus.Publish(new EnemyReachedBaseEvent
            {
                Damage = _damagePerEnemy
            });
            enemy.ReachBase();
        }
    }
}


