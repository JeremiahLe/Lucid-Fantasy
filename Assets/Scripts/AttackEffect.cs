using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    public enum TypeOfEffect { BuffSelf, DebuffSelf, BuffAllies, BuffEnemy, BuffEnemies, DebuffEnemies }
    public TypeOfEffect typeOfEffect;

    public float statToChange;
    public float amountToChange;

    public int statChangeDuration;
}
