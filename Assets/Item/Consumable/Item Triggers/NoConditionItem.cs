using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

[Serializable]
[CreateAssetMenu(fileName = "NoConditionItem", menuName = "Items")]
public class NoConditionItem : IAbilityTrigger
{
    public override async Task<int> TriggerItem(Item currentItem, CombatManagerScript combatManagerScript)
    {
        Monster targetMonster = combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;

        Debug.Log($"Triggering Item ({currentItem.itemName}) on {targetMonster.aiType} {targetMonster.name}!", this);

        await Task.Delay(abilityTriggerDelay);

        foreach (AttackEffect effect in currentItem.listOfItemEffects)
        {
            await effect.TriggerEffects(targetMonster, combatManagerScript.CurrentTargetedMonster, combatManagerScript.monsterAttackManager, currentItem.itemName, null, true);

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }
}
