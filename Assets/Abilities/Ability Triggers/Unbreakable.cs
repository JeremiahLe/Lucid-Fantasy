using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "UnbreakableOnStatChange", menuName = "Ability Triggers/Unbreakable/OnStatChange")]
public class Unbreakable : IAbilityTrigger
{
    public AttackEffect UnbreakablePhysicalDefenseBuff;
    public AttackEffect UnbreakablePhysicalAttackBuff;

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, Modifier modifier, AttackEffect attackEffect)
    {
        Debug.Log($"Triggering {targetMonster}'s {ability.abilityName} ability!", this);

        if (modifier.statChangeType == AttackEffect.StatChangeType.Debuff && modifier.statModified == AttackEffect.StatToChange.PhysicalDefense)
        {
            monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name}'s {ability.abilityName} Ability prevented Physical Defense Debuffs!");

            await UnbreakablePhysicalDefenseBuff.TriggerEffects(targetMonster, targetMonsterGameObject, monsterAttackManager, ability.abilityName);

            await Task.Delay(abilityTriggerDelay);

            await UnbreakablePhysicalAttackBuff.TriggerEffects(targetMonster, targetMonsterGameObject, monsterAttackManager, ability.abilityName);

            await Task.Delay(abilityTriggerDelay);

            attackEffect.monsterSource = targetMonster;

            modifier.modifierAmount = 0;

            return 1;
        }

        return 1;
    }
}

