using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "NoConditionAbilityTrigger", menuName = "Ability Triggers/NoConditionAbilityTrigger")]
public class NoConditionAbilityTrigger : IAbilityTrigger
{
    public List<AttackEffect> currentAttackEffectTriggered;

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability)
    {
        await Task.Delay(abilityTriggerDelay);

        foreach (AttackEffect attackEffect in currentAttackEffectTriggered)
        {
            attackEffect.TriggerEffects(targetMonster, targetMonsterGameObject, monsterAttackManager, ability.abilityName, monsterAttackManager.currentMonsterAttack);

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }
}