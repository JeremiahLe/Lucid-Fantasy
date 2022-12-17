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
    public enum MonsterAttackType { Attack, Status, Other };
    public MonsterAttackType monsterAttackType;

    // What type of damage does it deal?
    public enum MonsterAttackDamageType { Physical, Magical, True, Split, None };
    public MonsterAttackDamageType monsterAttackDamageType;

    // How many targets does the attack have?
    public enum MonsterAttackTargetCount { SingleTarget, MultiTarget, AllTargets, Everything };
    public MonsterAttackTargetCount monsterAttackTargetCount;

    //[EnableIf("monsterAttackTargetCount", MonsterAttackTargetCount.MultiTarget)]
    public int monsterAttackTargetCountNumber = 1;

    // What should the attack be targeting, if any limits?
    public enum MonsterAttackTargetType { EnemyTarget, AllyTarget, SelfTarget, Any }
    public MonsterAttackTargetType monsterAttackTargetType;

    // What should the attack SP cost be?
    public int monsterAttackSPCost = 1;

    // Attack VFX
    [AssetSelector(Paths = "Assets/Prefabs/VFX")]
    public GameObject AttackVFX;

    [AssetSelector(Paths = "Assets/Music/SoundEffects/MonsterAttacks")]
    public AudioClip monsterAttackSoundEffect;

    [Title("Monster Attack Combat Stats")]
    [DisableIf("monsterAttackType", MonsterAttackType.Status)]
    public float monsterAttackDamageScalar;

    [DisplayWithoutEdit] public float monsterBaseAttackStat = 1;

    [DisplayWithoutEdit] public float monsterAttackFlatDamageBonus;

    [DisableIf("monsterAttackNeverMiss", true)]
    public float monsterAttackAccuracy;

    public bool monsterAttackNeverMiss = false;

    public float monsterAttackCritChance = 5f; // default crit chance

    [Title("Monster Attack Other Data")]
    public GameObject monsterAttackSourceGameObject;
    public Monster monsterAttackSource;

    [Header("Monster Attack Effect List")]
    public List<AttackEffect> ListOfAttackEffects;

    public MonsterAttack(string _monsterAttackName, ElementClass _elementClass, MonsterAttackDamageType _damageType, float _monsterAttackMultiplier, float _monsterAttackDamageScalar, Monster _monsterAttackSource, GameObject _monsterAttackSourceGameObject)
    {
        monsterAttackName = _monsterAttackName;

        monsterElementClass = _elementClass;
        monsterAttackDamageType = _damageType;

        monsterAttackDamageScalar = _monsterAttackMultiplier;
        monsterBaseAttackStat = _monsterAttackDamageScalar;

        monsterAttackSource = _monsterAttackSource;
        monsterAttackSourceGameObject = _monsterAttackSourceGameObject;
    }
}
