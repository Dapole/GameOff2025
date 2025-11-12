using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "TD/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Base Settings")]
    public int StartingLives = 10;
    public int StartingGold = 100;

    [Header("Path")]
    public EnemyPath Path;
}

