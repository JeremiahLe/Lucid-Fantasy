using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

    public int health;
    public int maxHealth;

    public int mana;
    public int maxMana;

    public int physicalAttack;
    public int magicAttack;

    public int physicalDefense;
    public int magicDefense;

    public int speed;
    public int evasion;

    [Header("Monster Attack List")]
    public List<MonsterAttack> ListOfMonsterAttacks;
}
