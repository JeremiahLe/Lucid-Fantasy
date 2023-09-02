using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "UnstableEssence", menuName = "Ability Triggers/UnstableEssence/1")]
public class UnstableEssenceCondition1 : IAbilityTrigger
{
    public List<AttackEffect> currentAttackEffectTriggered;

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, Modifier modifier, AttackEffect effect, bool displayLogMessage)
    {
        if (modifier.statusEffectType == Modifier.StatusEffectType.None)
        {
            foreach (AttackEffect attackEffect in currentAttackEffectTriggered)
            {
                modifier.modifierAmount *= attackEffect.amountToChange / 100f;

                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name}'s {ability.abilityName} Ability doubled its incoming {modifier.statModified} stat change ({modifier.modifierAmount / 2})!");

                await Task.Delay(abilityTriggerDelay);
            }
        }

        return 1;
    }
}
