using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
[CreateAssetMenu(fileName = "New Monster", menuName = "Monster")]
public class Monster : ScriptableObject
{
    [Header("Monster Identifier")]
    public new string name;
    public Sprite baseSprite;

    [Header("Monster Combat Stats")]
    public int level;

    public enum MonsterType { Fire, Ice, Earth, Wind, Neutral };
    public MonsterType monsterType;

    public enum AIType { Ally, Enemy };
    public AIType aiType;

    public enum AILevel { Smart, Random, Bad, Player };
    public AILevel aiLevel;

    public float health;
    [DisplayWithoutEdit] public float maxHealth;

    public float mana;
    public float maxMana;

    public float physicalAttack;
    public float magicAttack;

    public float physicalDefense;
    public float magicDefense;

    public float speed;
    public float evasion;

    [Header("Monster Attack List")]
    public List<MonsterAttack> ListOfMonsterAttacks;
}
