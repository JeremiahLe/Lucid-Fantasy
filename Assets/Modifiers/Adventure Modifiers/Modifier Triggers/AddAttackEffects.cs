using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "AddAttackEffects", menuName = "AdventureModifierTriggers/AddAttackEffects")]
public class AddAttackEffects : IAbilityTrigger
{
    public Modifier adventureModifier;

    public List<IAbilityTrigger> listOfAdventureModifierAbilityTriggers;

    public override async Task<int> TriggerModifier(CombatManagerScript combatManagerScript, Monster.AIType aiType, AttackEffect.EffectTime effectTime)
    {
        Debug.Log($"Triggering {adventureModifier.modifierName}!");

        List<IAbilityTrigger> targetCombatPassiveList;

        if (aiType == Monster.AIType.Ally)
            targetCombatPassiveList = combatManagerScript.ListOfAllyCombatPassives;
        else
            targetCombatPassiveList = combatManagerScript.ListOfEnemyCombatPassives;

        foreach (IAbilityTrigger abilityTrigger in listOfAdventureModifierAbilityTriggers)
        {
            targetCombatPassiveList.Add(abilityTrigger);
        }

        await Task.Delay(abilityTriggerDelay);

        return 1;
    }
}
