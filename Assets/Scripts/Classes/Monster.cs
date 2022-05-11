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
    public enum MonsterType { Fire, Ice, Earth, Wind, Shadow, Neutral };
    public MonsterType monsterType;

    public enum AIType { Ally, Enemy };
    public AIType aiType;

    public enum AILevel { Smart, Random, Bad, Player };
    public AILevel aiLevel;

    [Title("Monster Combat Stats")]
    [PropertySpace(SpaceBefore = 15)]
    [Range(1, 50)] public float level;

    [Range(1, 10000)] public float health;
    [DisplayWithoutEdit] public float maxHealth;

    //[PropertySpace(SpaceBefore = 15)]
    //public float mana;
    //public float maxMana;

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

    [Header("Monster Attack List")]
    public List<MonsterAttack> ListOfMonsterAttacks;

    [Header("Monster Modifier List")]
    public List<Modifier> ListOfModifiers;
}
