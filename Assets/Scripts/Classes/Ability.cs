using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities")]
public class Ability : MonoBehaviour
{
    public string abilityName;
    public string abilityDescription;

    public enum AbilityTriggerTime { GameStart, RoundStart, RoundEnd, PreAttack, PostAttack, OnStatChange };
    public AbilityTriggerTime abilityTriggerTime;

    public void TriggerAbility()
    {
        // Trigger ability effects
        Debug.Log("Abiltiy Triggered!");
    }
}
