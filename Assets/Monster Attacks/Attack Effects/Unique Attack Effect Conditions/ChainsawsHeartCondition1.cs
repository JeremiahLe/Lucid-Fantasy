using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "ChainsawsHeartCondition1", menuName = "UniqueAttackEffects/ChainsawsHeart/1")]
public class ChainsawsHeartCondition1 : IAbilityTrigger
{
    public override async Task<int> TriggerAttackEffect(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, AttackEffect attackEffect)
    {
        Debug.Log("Unique Attack Effect Called!");

        if (targetMonster.health <= 0)
            return 1;

        // Is target's health 50% or less of maximum?
        float currentHealth = targetMonster.health;
        float maxHealth = targetMonster.maxHealth;

        if (currentHealth / maxHealth <= 0.5f)
        {
            Debug.Log("Unique Attack Effect Condition Met!");

            // Send execute message to combat log
            CombatManagerScript combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} " +
                $"was executed!");

            // remove monster
            targetMonster.health = 0;

            await targetMonsterGameObject.GetComponent<CreateMonster>().UpdateStats(true, monsterAttackManager.currentMonsterTurnGameObject, false, currentHealth);

            await monsterAttackManager.TriggerAbilityEffects(attackEffect.monsterAttackTrigger.monsterAttackSource, attackEffect.monsterAttackTrigger.monsterAttackSourceGameObject, AttackEffect.EffectTime.OnKill, attackEffect.monsterAttackTrigger);

            await monsterAttackManager.TriggerAbilityEffects(targetMonster, targetMonsterGameObject, AttackEffect.EffectTime.OnDeath, attackEffect.monsterAttackTrigger);

            attackEffect.monsterAttackTrigger.monsterAttackSource.monsterKills += 1;

            attackEffect.monsterAttackTrigger.monsterAttackSourceGameObject.GetComponent<CreateMonster>().GrantExp(monsterAttackManager.CalculateExp(targetMonster));

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }

}