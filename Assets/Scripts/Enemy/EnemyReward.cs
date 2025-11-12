using UnityEngine;

public class EnemyReward : MonoBehaviour
{
    [SerializeField] private int _goldReward = 10;

    public int GoldReward => _goldReward;
}


