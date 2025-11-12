using System.Collections.Generic;
using UnityEngine;

public class CorpseManager : MonoBehaviour
{
    private static CorpseManager _instance;
    public static CorpseManager Instance => _instance;

    private List<GameObject> _corpses = new List<GameObject>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Subscribe to wave completion events
        EventBus.Subscribe<WaveCompletedCleanupEvent>(OnWaveCompleted);
    }

    private void OnWaveCompleted(WaveCompletedCleanupEvent evt)
    {
        ClearAllCorpses();
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<WaveCompletedCleanupEvent>(OnWaveCompleted);
    }

    public void RegisterCorpse(GameObject corpse)
    {
        if (corpse != null)
        {
            _corpses.Add(corpse);
        }
    }

    public void ClearAllCorpses()
    {
        foreach (var corpse in _corpses)
        {
            if (corpse != null)
            {
                Destroy(corpse);
            }
        }
        _corpses.Clear();
    }

    public int GetCorpseCount()
    {
        // Clean up null references
        _corpses.RemoveAll(c => c == null);
        return _corpses.Count;
    }
}

