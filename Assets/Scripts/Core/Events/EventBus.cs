using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    private static Dictionary<Type, List<object>> _subscribers = new Dictionary<Type, List<object>>();

    public static void Subscribe<T>(Action<T> handler) where T : class
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
        {
            _subscribers[type] = new List<object>();
        }
        _subscribers[type].Add(handler);
    }

    public static void Unsubscribe<T>(Action<T> handler) where T : class
    {
        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
        {
            _subscribers[type].Remove(handler);
        }
    }

    public static void Publish<T>(T eventData) where T : class
    {
        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
        {
            foreach (var subscriber in _subscribers[type])
            {
                if (subscriber is Action<T> handler)
                {
                    handler.Invoke(eventData);
                }
            }
        }
    }

    public static void Clear()
    {
        _subscribers.Clear();
    }
}

// Event definitions
public class EnemySpawnedEvent
{
    public GameObject Enemy { get; set; }
}

public class EnemyDiedEvent
{
    public GameObject Enemy { get; set; }
    public int GoldReward { get; set; }
}

public class EnemyReachedBaseEvent
{
    public int Damage { get; set; }
}

public class LivesChangedEvent
{
    public int CurrentLives { get; set; }
    public int MaxLives { get; set; }
}

public class GoldChangedEvent
{
    public int CurrentGold { get; set; }
}

public class WaveStartedEvent
{
    public int WaveNumber { get; set; }
}

public class WaveCompletedEvent
{
    public int WaveNumber { get; set; }
}

public class GameOverEvent
{
    public bool IsVictory { get; set; }
}

public class WaveCompletedCleanupEvent
{
    // Event for cleaning up corpses and other wave-end cleanup
}

