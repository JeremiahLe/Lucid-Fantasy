using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "NimbusBarricade", menuName = "UniqueAttackEffects/NimbusBarricade")]
public class NimbusBarricade : IAbilityTrigger
{
    public override async Task<int> TriggerAttackEffect(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, AttackEffect attackEffect)
    {
        Debug.Log("Unique Attack Effect Called!");

        if (targetMonsterGameObject.TryGetComponent(out CreateMonster monsterComponent) != false)
        {
            if (monsterComponent.monsterDamageTakenThisRound > 0)
            {
                Debug.Log("Unique Attack Effect Condition Met!");

                float bonusDamage = ((attackEffect.amountToChange / 100f) * monsterComponent.monsterDamageTakenThisRound) / 100f;

                if (bonusDamage <= 0)
                    bonusDamage = 0.01f;

                monsterAttackManager.currentMonsterAttack.monsterAttackFlatDamageBonus += bonusDamage;

                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} gained {bonusDamage * 100}% bonus damage!");

                await Task.Delay(abilityTriggerDelay);
            }
        }

        return 1;
    }
}
