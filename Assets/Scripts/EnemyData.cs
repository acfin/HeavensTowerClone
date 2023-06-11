using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptObjects/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public string enemyType;
    public int health;
    public float speed;
    public float aggroRange;
    public float atkRange;
    public int damage;
    public int defense;
}
