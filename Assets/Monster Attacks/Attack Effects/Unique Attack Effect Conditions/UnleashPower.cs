using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "UnleashPower", menuName = "UniqueAttackEffects/UnleashPower")]
public class UnleashPower : IAbilityTrigger
{
    public override async Task<int> TriggerAttackEffect(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, AttackEffect attackEffect)
    {
        Debug.Log("Unique Attack Effect Called!");

        if (targetMonsterGameObject.TryGetComponent(out CreateMonster monsterComponent) != false)
        {
            if (monsterComponent.monsterRecievedStatBoostThisRound)
            {
                Debug.Log("Unique Attack Effect Condition Met!");

                monsterAttackManager.currentMonsterAttack.monsterAttackFlatDamageBonus += 2;

                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} gained 50% bonus damage!");

                await Task.Delay(abilityTriggerDelay);
            }
        }

        return 1;
    }
}
