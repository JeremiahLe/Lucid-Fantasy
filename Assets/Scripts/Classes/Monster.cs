using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;

[System.Serializable]
[CreateAssetMenu(fileName = "New Monster", menuName = "Monster")]
public class Monster : ScriptableObject
{
    // The monster's name and sprite
    [Title("Monster Identifier")]
    public new string name;
    [AssetSelector(Paths = "Assets/Sprites/Renders")]
    public Sprite baseSprite;

    // The monster's elements
    [Title("Setup")]
    public enum MonsterElement
    {
        Fire, Water, Earth, Wind,
        Shadow, Neutral, None, Light, Time,
        Elixir, Electric, Stone, Sound
    };

    public ElementClass monsterElement;
    public ElementClass monsterSubElement;

    // The monster's AI Type and Level
    public enum AIType { Ally, Enemy };
    public AIType aiType;

    public enum AILevel { Smart, Random, Bad, Player };
    public AILevel aiLevel;

    // The monster's flavour text and ability
    [TextArea]
    public string monsterFlavourText;

    public Ability monsterAbility;

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
    [Range(1.25f, 2.5f)] public float critDamage;
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
    [Range(0f, 100f)] public float cachedCritDamage;

    [Title("Adventure - Monster Combat Stats Previous Level Cached")]
    [PropertySpace(SpaceBefore = 15)]
    [Range(0f, 100f)] public float previouslyCachedMaxHealth;

    [Range(1, 300)] public float previouslyCachedPhysicalAttack;
    [Range(1, 300)] public float previouslyCachedMagicAttack;

    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 1000)] public float previouslyCachedPhysicalDefense;
    [Range(1, 1000)] public float previouslyCachedMagicDefense;

    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 100)] public float previouslyCachedSpeed;
    [Range(0, 99)] public float previouslyCachedEvasion;
    [Range(0, 100)] public float previouslyCachedCritChance;
    [Range(0f, 100f)] public float previouslyCachedBonusAccuracy;
    [Range(0f, 100f)] public float previouslyCachedCritDamage;

    public int cachedLevel;
    public CreateMonster.MonsterRowPosition cachedMonsterRowPosition;

    [Title("Adventure - Monster Other Stats")]
    public bool monsterIsOwned = false;
    public float cachedDamageDone;
    public int monsterKills;

    public int monsterCurrentExp;
    public int monsterExpToNextLevel = 105;

    [Title("Ascension Stats")]
    public MonsterAttack monsterAscensionAttack;
    public int ascensionGoldRequirement = 10;

    public enum AscensionType { Gale, Storm, Spirit, Nightmare, Tundra, Hydro, Harmony, Dissonance, Magma, Inferno, Steel, Mineral, Plasma, Machine, Decay, Purity, Soil, Verdant, Chaos, Order, Justice, Divine };
    public AscensionType ascensionType;

    public Monster firstEvolutionPath;
    public int firstEvolutionLevelReq = 20;
    public Item ascensionOneMaterial;
    public int ascensionOneMaterialAmount = 1;

    public Monster secondEvolutionPath;
    public int secondEvolutionLevelReq = 20;
    public Item ascensionTwoMaterial;
    public int ascensionTwoMaterialAmount = 1;

    [Title("Ascension Stat Growths")]
    [Title("Monster Scaling Stats")]
    [PropertySpace(SpaceBefore = 15)]
    [Range(50, 500)] public int healthGrowth;

    [PropertySpace(SpaceBefore = 15)]
    [Range(-10, 10)] public int physicalAttackGrowth;
    [Range(-10, 10)] public int magicAttackGrowth;

    [PropertySpace(SpaceBefore = 15)]
    [Range(-10, 10)] public int physicalDefenseGrowth;
    [Range(-10, 10)] public int magicDefenseGrowth;

    [PropertySpace(SpaceBefore = 15)]
    [Range(-10, 10)] public int speedGrowth;

    [PropertySpace(SpaceBefore = 15)]
    [Range(-10, 25)] public int evasionGrowth;
    [Range(-10, 25)] public int critChanceGrowth;
    [Range(-10, 25)] public int bonusAccuracyGrowth;

}
