using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "ReaperEffect1", menuName = "Ability Triggers/Reaper/1")]
public class ReaperTrigger1 : IAbilityTrigger
{
    public List<AttackEffect> currentAttackEffectTriggered;

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, MonsterAttack attackTrigger)
    {
        await Task.Delay(abilityTriggerDelay);

        foreach (AttackEffect attackEffect in currentAttackEffectTriggered)
        {
            await attackEffect.TriggerEffects(monsterAttackManager, ability.abilityName, monsterAttackManager.currentMonsterAttack);

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }
}
