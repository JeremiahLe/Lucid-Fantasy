using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "NoConditionAdventureModifierTrigger", menuName = "AdventureModifierTriggers/NoConditionAdventureModifierTrigger")]
public class NoConditionAdventureModifierTrigger : IAbilityTrigger
{
    public Modifier adventureModifier;

    public List<AttackEffect> listOfAdventureModifierEffects;

    public override async Task<int> TriggerModifier(CombatManagerScript combatManagerScript, Monster.AIType aiType)
    {
        Debug.Log($"Triggering {adventureModifier.modifierName}!");

        List<GameObject> targetMonsterList;

        if (aiType == Monster.AIType.Ally)
            targetMonsterList = combatManagerScript.ListOfAllys;
        else
            targetMonsterList = combatManagerScript.ListOfEnemies;

        foreach (GameObject monsterObj in targetMonsterList)
        {
            Monster targetMonster = monsterObj.GetComponent<CreateMonster>().monsterReference;

            foreach (AttackEffect modifierEffect in listOfAdventureModifierEffects)
            {
                await modifierEffect.TriggerEffects(targetMonster, monsterObj, combatManagerScript.monsterAttackManager, adventureModifier.modifierName);

                await Task.Delay(abilityTriggerDelay);
            }

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }
}
