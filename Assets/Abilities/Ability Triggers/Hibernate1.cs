using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "Hibernate", menuName = "Ability Triggers/Hibernate/1")]
public class Hibernate1 : IAbilityTrigger
{
    public AttackEffect hibernateHealingIncreaseAttackEffect;
    public AttackEffect hibernateReduceSpeedAttackEffect;

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, Modifier modifier, AttackEffect attackEffect)
    {
        if (modifier.statModified == AttackEffect.StatToChange.Health && modifier.statChangeType == AttackEffect.StatChangeType.Buff)
        {
            modifier.modifierAmount *= hibernateHealingIncreaseAttackEffect.amountToChange / 100f;

            modifier.modifierAmount = Mathf.RoundToInt(modifier.modifierAmount);

            monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name}'s {ability.abilityName} Ability increased its incoming {modifier.statModified} (+{Mathf.RoundToInt(modifier.modifierAmount / 1.5f)})!");

            await Task.Delay(abilityTriggerDelay);

            await hibernateReduceSpeedAttackEffect.TriggerEffects(targetMonster, targetMonsterGameObject, monsterAttackManager, ability.abilityName);

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }
}
