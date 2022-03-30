using System; // allows serialization of custom classes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable]
[CreateAssetMenu(fileName = "New Monster Attack", menuName = "Monster Attacks")]
public class MonsterAttack : ScriptableObject
{
    public string monsterAttackName;

    public Monster.MonsterType monsterAttackElement;
    public enum MonsterAttackType { Physical, Magical };
    public MonsterAttackType monsterAttackType;

    public float monsterAttackDamage;
    public string monsterAttackDescription;

    public float monsterAttackAccuracy;
}
