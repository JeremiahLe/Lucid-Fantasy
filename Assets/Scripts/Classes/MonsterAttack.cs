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

    // What element is the attack?
    public ElementClass.MonsterElement monsterAttackElement;
    public ElementClass monsterElementClass;

    // What type of attack is it? (Attack, Buff, other)
    public enum MonsterAttackType { Attack, Buff, Other };
    public MonsterAttackType monsterAttackType;

    // What type of damage does it deal?
    public enum MonsterAttackDamageType { Physical, Magical, True, Split };
    public MonsterAttackDamageType monsterAttackDamageType;

    // How many targets does the attack have?
    public enum MonsterAttackTargetCount { SingleTarget, MultiTarget, AllTargets, Everything };
    public MonsterAttackTargetCount monsterAttackTargetCount;

    // What should the attack be targeting, if any limits?
    public enum MonsterAttackTargetType { EnemyTarget, AllyTarget, SelfTarget, Any }
    public MonsterAttackTargetType monsterAttackTargetType;

    [AssetSelector(Paths = "Assets/Music/SoundEffects/MonsterAttacks")]
    public AudioClip monsterAttackSoundEffect;

    [Title("Monster Attack Combat Stats")]
    public float monsterAttackDamage;
    [DisplayWithoutEdit] public float monsterAttackFlatDamageBonus;
    public float monsterAttackAccuracy;
    public float monsterAttackCritChance = 5f; // default crit chance
    public bool monsterAttackNeverMiss = false;

    [Title("Monster Attack Other Data")]
    public bool attackHasCooldown;
    [DisplayWithoutEdit] public bool attackOnCooldown;
    public int attackBaseCooldown;
    [DisplayWithoutEdit] public int attackCurrentCooldown;
    public GameObject AttackVFX;

    [Header("Monster Attack Effect List")]
    public List<AttackEffect> ListOfAttackEffects;
}
