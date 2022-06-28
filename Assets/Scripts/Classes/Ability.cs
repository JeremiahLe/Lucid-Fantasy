using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities")]
public class NewBehaviourScript : MonoBehaviour
{
    public string abilityName;
    public string abilityDescription;

    public enum AbilityTriggerTime { GameStart, RoundStart, PreAttack, PostAttack, OnStatChange };

}
