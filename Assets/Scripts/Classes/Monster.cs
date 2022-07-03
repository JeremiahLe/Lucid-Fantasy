using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;

[System.Serializable]
[CreateAssetMenu(fileName = "New Monster", menuName = "Monster")]
public class Monster : ScriptableObject
{
    [Title("Monster Identifier")]
    public new string name;
    [AssetSelector(Paths = "Assets/Sprites/Renders")]
    public Sprite baseSprite;

    [Title("Setup")]
    public enum MonsterElement
    { Fire, Water, Earth, Wind,
        Shadow, Neutral, None, Light, Time,
        Elixir, Electric, Stone, Sound };

    public ElementClass monsterElement;
    public ElementClass monsterSubElement;

    public enum AIType { Ally, Enemy };
    public AIType aiType;

    public enum AILevel { Smart, Random, Bad, Player };
    public AILevel aiLevel;

    [Title("Monster Combat Stats")]
    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 50)] public int level;

    [Range(1, 10000)] public float health;
    [Range(1, 10000)] public float maxHealth;
    [DisplayWithoutEdit] public float cachedHealth;
    [DisplayWithoutEdit] public float cachedMaxHealth;

    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 300)] public float physicalAttack;
    [Range(1, 300)] public float magicAttack;

    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 1000)] public float physicalDefense;
    [Range(1, 1000)] public float magicDefense;

    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 100)] public float speed;
    [Range(0, 99)] public float evasion;
    [Range(0, 100)] public float critChance;
    [Range(1.5f, 2.5f)] public float critDamage;
    [Range(0f, 100f)] public float bonusAccuracy;

    [Title("Monster Scaling Stats")]
    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 25)] public int healthScaler;

    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 5)] public int physicalAttackScaler;
    [Range(1, 5)] public int magicAttackScaler;

    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 5)] public int physicalDefenseScaler;
    [Range(1, 5)] public int magicDefenseScaler;

    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 5)] public int speedScaler;

    //[PropertySpace(SpaceBefore = 15)]
    //public float mana;
    //public float maxMana;

    [Header("Monster Attack List")]
    public List<MonsterAttack> ListOfMonsterAttacks;

    [Header("Monster Modifier List")]
    public List<Modifier> ListOfModifiers;

    [Header("Monster Available Attack List")]
    public List<MonsterAttack> ListOfMonsterAttacksAvailable;

    [Title("Adventure - Monster Combat Stats Cached")]
    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 300)] public float cachedPhysicalAttack;
    [Range(1, 300)] public float cachedMagicAttack;

    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 1000)] public float cachedPhysicalDefense;
    [Range(1, 1000)] public float cachedMagicDefense;

    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 100)] public float cachedSpeed;
    [Range(0, 99)] public float cachedEvasion;
    [Range(0, 100)] public float cachedCritChance;
    [Range(0f, 100f)] public float cachedBonusAccuracy;

    public int cachedLevel;

    [Title("Adventure - Monster Other Stats")]
    public float cachedDamageDone;
    public int monsterKills;

    public int monsterCurrentExp;
    public int monsterExpToNextLevel = 125;

    public Monster firstEvolutionPath;
    public Monster secondEvolutionPath;

    public int firstEvolutionLevelReq = 20;
    public int secondEvolutionLevelReq = 20;
}
