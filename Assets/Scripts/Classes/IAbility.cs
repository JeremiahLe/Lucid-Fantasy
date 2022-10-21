using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "New Ability Trigger", menuName = "Ability Triggers")]
public abstract class IAbilityTrigger : ScriptableObject
{
    public int abilityTriggerDelay = 300;

    public abstract Task<int> TriggerAbility(MonsterAttackManager monsterAttackManager, Ability ability);
}
