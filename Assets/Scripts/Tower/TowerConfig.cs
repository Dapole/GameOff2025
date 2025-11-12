using UnityEngine;

[CreateAssetMenu(fileName = "TowerConfig", menuName = "TD/Tower Config")]
public class TowerConfig : ScriptableObject
{
    [Header("Combat")]
    public float Range = 5f;
    public float FireRate = 1f;
    public int Damage = 10;

    [Header("Projectile")]
    public GameObject ProjectilePrefab;
    public float ProjectileSpeed = 10f;
}


