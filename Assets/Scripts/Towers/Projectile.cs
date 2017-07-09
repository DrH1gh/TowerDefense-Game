using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField]
    private int attackDamage;
    [SerializeField]
    private proType projectileType;

    public int AttackDamage
    {
        get
        {
            return attackDamage;
        }
    }

    public proType ProjectileType
    {
        get
        {
            return projectileType;
        }
    }

}


    public enum proType
    {
        rock, arrow, fireball
    };