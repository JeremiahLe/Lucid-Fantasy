using System; // allows serialization of custom classes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Monster Attack", menuName = "Monster Attacks")]
public class MonsterAttack : ScriptableObject
{
    public string monsterAttackName;
    public Monster.MonsterType monsterAttackType;
    public float monsterAttackDamage;
}
