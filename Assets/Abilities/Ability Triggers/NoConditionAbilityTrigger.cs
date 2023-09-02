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

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, bool displayLogMesasge)
    {
        await Task.Delay(abilityTriggerDelay);

        Debug.Log($"Triggering {targetMonster}'s {ability.abilityName} ability!", this);

        foreach (AttackEffect attackEffect in currentAttackEffectTriggered)
        {
            await attackEffect.TriggerEffects(targetMonster, targetMonsterGameObject, monsterAttackManager, ability.abilityName, monsterAttackManager.currentMonsterAttack);

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }
}
