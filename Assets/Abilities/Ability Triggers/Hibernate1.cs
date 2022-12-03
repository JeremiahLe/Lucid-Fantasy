using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "Hibernate", menuName = "Ability Triggers/Hibernate/1")]
public class Hibernate1 : IAbilityTrigger
{
    public List<AttackEffect> currentAttackEffectTriggered;

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, Modifier modifier)
    {
        if (modifier.statModified == AttackEffect.StatToChange.Health && modifier.statChangeType == AttackEffect.StatChangeType.Buff)
        {
            foreach (AttackEffect attackEffect in currentAttackEffectTriggered)
            {
                modifier.modifierAmount *= attackEffect.amountToChange / 100f;

                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name}'s {ability.abilityName} Ability increased its incoming {modifier.statModified.ToString()} (+{modifier.modifierAmount / 2})!");

                await Task.Delay(abilityTriggerDelay);
            }
        }

        return 1;
    }
}
