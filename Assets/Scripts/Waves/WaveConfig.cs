using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "TD/Wave Config")]
[Serializable]
public class WaveConfig : ScriptableObject
{
    [Header("Enemy Settings")]
    public GameObject EnemyPrefab;
    public int EnemyCount = 10;
    public float SpawnInterval = 1f;
    public float DelayBeforeWave = 0f;
}

