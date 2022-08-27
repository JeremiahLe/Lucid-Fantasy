using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;

[Serializable]
[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities")]
public class Ability : ScriptableObject
{
    public string abilityName;
    public string abilityDescription;

    public enum AbilityTriggerTime { GameStart, RoundStart, RoundEnd, PreAttack, PostAttack, OnStatChange, OnDamageTaken, OnDeath, Passive };
    public AbilityTriggerTime abilityTriggerTime;

    [EnableIf("abilityTriggerTime", AbilityTriggerTime.OnStatChange)]
    public AttackEffect.StatEnumToChange onWhatStatChange;

    public bool gainsImmunity;
    [EnableIf("gainsImmunity")]
    public AttackEffect.StatEnumToChange immunityGained;

    public void TriggerAbility()
    {
        // Trigger ability effects
        Debug.Log("Abiltiy Triggered!");
    }
}
