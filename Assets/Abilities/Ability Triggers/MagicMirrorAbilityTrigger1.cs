using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "MagicMirrorAbilityTrigger", menuName = "Ability Triggers/MagicMirror/1")]
public class MagicMirrorAbilityTrigger1 : IAbilityTrigger
{
    public AttackEffect currentAttackEffectTriggered;

    public override async Task<int> TriggerAbility(MonsterAttackManager monsterAttackManager, Ability ability)
    {
        if (monsterAttackManager.currentMonsterAttack.monsterAttackDamageType == MonsterAttack.MonsterAttackDamageType.Magical)
        {
            await Task.Delay(abilityTriggerDelay);

            currentAttackEffectTriggered.TriggerEffects(monsterAttackManager, ability.abilityName, monsterAttackManager.currentMonsterAttack);

            return 1;
        }

        return 1;
    }
}