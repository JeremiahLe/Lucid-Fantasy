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
            //monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name}'s {ability.abilityName} Ability prevented Physical Defense Debuffs!");

            //await UnbreakablePhysicalDefenseBuff.TriggerEffects(targetMonster, targetMonsterGameObject, monsterAttackManager, ability.abilityName);

            modifier.statChangeType = AttackEffect.StatChangeType.Buff;

            modifier.modifierAmount = (UnbreakablePhysicalDefenseBuff.amountToChange / 100f) * UnbreakablePhysicalDefenseBuff.GetBonusDamageSource(UnbreakablePhysicalDefenseBuff.statToChange, targetMonster);

            modifier.modifierAmount = Mathf.RoundToInt(modifier.modifierAmount);

            if (modifier.modifierAmount < 1)
            {
                modifier.modifierAmount = 1;
            }

            monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name}'s {ability.abilityName} Ability negated the incoming debuff!");

            await Task.Delay(abilityTriggerDelay);

            await UnbreakablePhysicalAttackBuff.TriggerEffects(targetMonster, targetMonsterGameObject, monsterAttackManager, ability.abilityName);

            await Task.Delay(abilityTriggerDelay);

            return 1;
        }

        return 1;
    }
}

