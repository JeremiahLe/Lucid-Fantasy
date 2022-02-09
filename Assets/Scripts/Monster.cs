using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "Monster")]
public class Monster : ScriptableObject
{
    public new string name;
    public int level;

    public enum MonsterType { Fire, Ice, Earth, Wind };
    public MonsterType monsterType;

    public int health;
    public int maxHealth;

    public Sprite baseSprite;
}
