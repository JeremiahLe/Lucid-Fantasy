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
    [PreviewField(150)]
    public Sprite baseSprite;

    [AssetSelector(Paths = "Assets/Prefabs/UI/Monster Select Cards")]
    public GameObject monsterSelectCard;

    public enum MonsterAscensionPhase { Basic, Ascended }
    public MonsterAscensionPhase monsterAscensionPhase;

    // The monster's elements
    [Title("Setup")]
    public ElementClass monsterElement;
    public ElementClass monsterSubElement;

    // The monster's AI Type and Level
    public enum AIType { Ally, Enemy };
    public AIType aiType;

    public enum AILevel { Offensive, Random, Supportive, Player };
    public AILevel aiLevel;

    // The monster's flavour text and ability
    [TextArea]
    public string monsterFlavourText;

    public Ability monsterAbility;

    public List<Ability> listOfPotentialMonsterAbilities;

    [Title("Monster Combat Stats")]
    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 50)] public int level;

    [Title("Monster Basic Stats")]
    [Range(1, 10000)] public float health;
    [Range(1, 10000)] public float maxHealth;

    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 300)] public float physicalAttack;
    [Range(1, 300)] public float magicAttack;

    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 1000)] public float physicalDefense;
    [Range(1, 1000)] public float magicDefense;

    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 100)] public float speed;

    [Title("Monster Advanced Stats")]
    [Range(0, 99)] public float evasion;
    [Range(0, 100)] public float critChance;
    [Range(1.25f, 2.5f)] public float critDamage;
    [Range(0f, 100f)] public float bonusAccuracy;

    [PropertySpace(SpaceBefore = 15)]
    [Range(0, 10)] public float initialSP = 3;
    [Range(0, 10)] public float currentSP = 3;
    [Range(1, 10)] public float maxSP = 3;
    [Range(1, 3)] public float spRegen = 1;

    [PropertySpace(SpaceBefore = 15)]
    [Range(0, 100)] public float lifeStealPercent = 0;
    [Range(0, 100)] public float bonusDamagePercent = 0;
    [Range(0, 99)] public float damageReductionPercent = 0;

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

    [Title("Adventure - Monster Cached Stats")]
    [PropertySpace(SpaceBefore = 15)]
    [DisplayWithoutEdit] public float cachedHealth;
    [DisplayWithoutEdit] public float cachedMaxHealth;

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

    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 10)] public float cachedMaxSP;
    [Range(1, 3)] public float cachedSpRegen;
    [Range(0, 100)] public float cachedLifeStealPercent;
    [Range(0, 100)] public float cachedBonusDamagePercent;
    [Range(0, 99)] public float cachedDamageReductionPercent;

    public int monsterCachedBattleIndex;

    public List<MonsterAttack> ListOfCachedMonsterAttacks;

    [Title("Adventure - Monster Cached Stats Before Level Up")]
    [PropertySpace(SpaceBefore = 15)]
    [Range(0f, 100f)] public float previouslyCachedMaxHealth;

    [PropertySpace(SpaceBefore = 15)]
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
    public CreateMonster.MonsterStance cachedMonsterRowPosition;

    [Title("Adventure - Monster Other Stats")]
    public bool monsterIsOwned = false;
    public float cachedDamageDone;
    public int monsterKills;

    public int monsterCurrentExp;
    public int monsterExpToNextLevel = 105;

    [DisplayWithoutEdit] public float cachedHealthAtBattleStart;

    [Title("Ascension Stats")]
    [EnableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    public MonsterAttack monsterAscensionAttack;
    [EnableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    public int ascensionGoldRequirement = 10;

    [EnableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    public enum AscensionType { Gale, Storm, Spirit, Nightmare, Tundra, Hydro, Harmony, Dissonance, Magma, Inferno, Steel, Mineral, Plasma, Machine, Decay, Purity, Soil, Verdant, Chaos, Order, Justice, Divine };
    public AscensionType ascensionType;

    [Title("First Ascension")]
    [DisableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    public Monster firstEvolutionPath;
    [DisableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    public int firstEvolutionLevelReq = 20;
    [DisableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    public Item ascensionOneMaterial;
    [DisableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    public int ascensionOneMaterialAmount = 1;

    [Title("Second Ascension")]
    [DisableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    public Monster secondEvolutionPath;
    [DisableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    public int secondEvolutionLevelReq = 20;
    [DisableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    public Item ascensionTwoMaterial;
    [DisableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    public int ascensionTwoMaterialAmount = 1;

    [Title("Ascension Stat Growths")]
    [Title("Monster Scaling Stats")]
    [PropertySpace(SpaceBefore = 15)]
    [EnableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    [Range(50, 500)] public int healthGrowth;

    [PropertySpace(SpaceBefore = 15)]
    [EnableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    [Range(-10, 10)] public int physicalAttackGrowth;
    [EnableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    [Range(-10, 10)] public int magicAttackGrowth;

    [PropertySpace(SpaceBefore = 15)]
    [EnableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    [Range(-10, 10)] public int physicalDefenseGrowth;
    [EnableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    [Range(-10, 10)] public int magicDefenseGrowth;

    [PropertySpace(SpaceBefore = 15)]
    [EnableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    [Range(-10, 10)] public int speedGrowth;

    [PropertySpace(SpaceBefore = 15)]
    [EnableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    [Range(-10, 25)] public int evasionGrowth;
    [EnableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    [Range(-10, 25)] public int critChanceGrowth;
    [EnableIf("monsterAscensionPhase", MonsterAscensionPhase.Ascended)]
    [Range(-10, 25)] public int bonusAccuracyGrowth;

}
