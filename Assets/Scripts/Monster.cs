using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "Monster")]
public class Monster : ScriptableObject
{
    [Header("Monster Identifier")]
    public new string name;
    public Sprite baseSprite;

    [Header("Monster Combat Stats")]
    public int level;

    public enum MonsterType { Fire, Ice, Earth, Wind };
    public MonsterType monsterType;

    public int health;
    public int maxHealth;

    public int mana;
    public int maxMana;

    public int physicalAttack;
    public int magicAttack;

    public int physicalDefense;
    public int magicDefense;

    public int speed;
}
