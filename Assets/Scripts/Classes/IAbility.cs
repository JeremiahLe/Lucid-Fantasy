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

    public virtual async Task<int> TriggerAbility(MonsterAttackManager monsterAttackManager, Ability ability)
    {
        return 1;
    }

    public virtual async Task<int> TriggerAttackEffect(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, AttackEffect attackEffect)
    {
        return 1;
    }
}
