using System; // allows serialization of custom classes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;

[Serializable]
[CreateAssetMenu(fileName = "New Monster Attack", menuName = "Monster Attacks")]
public class MonsterAttack : ScriptableObject
{
    [Title("Monster Attack Identifier")]
    public string monsterAttackName;
    [TextArea]
    public string monsterAttackDescription;

    // Using Monster's Enums Title
    public Monster.MonsterType monsterAttackElement;
    public enum MonsterAttackType { Physical, Magical, True, Split, SelfTarget, AllyTarget };
    public MonsterAttackType monsterAttackType;

    [AssetSelector(Paths = "Assets/Music/SoundEffects/MonsterAttacks")]
    public AudioClip monsterAttackSoundEffect;

    [Title("Monster Attack Combat Stats")]
    public float monsterAttackDamage;
    [DisplayWithoutEdit] public float monsterAttackFlatDamageBonus;
    public float monsterAttackAccuracy;
    public float monsterAttackCritChance = 10f; // default crit chance

    [Title("Monster Attack Other Data")]
    public bool attackHasCooldown;
    [DisplayWithoutEdit] public bool attackOnCooldown;
    public int attackBaseCooldown;
    [DisplayWithoutEdit] public int attackCurrentCooldown;

    [Header("Monster Attack Effect List")]
    public List<AttackEffect> ListOfAttackEffects;
}
