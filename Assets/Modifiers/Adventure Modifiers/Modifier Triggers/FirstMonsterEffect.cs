using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "FirstMonsterEffect", menuName = "AdventureModifierTriggers/FirstMonsterEffect")]
public class FirstMonsterEffect : IAbilityTrigger
{
    public Modifier adventureModifier;

    public List<AttackEffect> listOfAdventureModifierEffects;

    public override async Task<int> TriggerModifier(CombatManagerScript combatManagerScript, Monster.AIType aiType, AttackEffect.EffectTime effectTime)
    {
        Debug.Log($"Triggering {adventureModifier.modifierName}!");

        List<GameObject> targetMonsterList;

        if (aiType == Monster.AIType.Ally)
            targetMonsterList = combatManagerScript.ListOfAllys;
        else
            targetMonsterList = combatManagerScript.ListOfEnemies;

        GameObject monsterObj = targetMonsterList[0];
        Monster targetMonster = monsterObj.GetComponent<CreateMonster>().monsterReference;

        foreach (AttackEffect modifierEffect in listOfAdventureModifierEffects)
        {
            await modifierEffect.TriggerEffects(targetMonster, monsterObj, combatManagerScript.monsterAttackManager, adventureModifier.modifierName, true);

            await Task.Delay(abilityTriggerDelay);
        }

        await Task.Delay(abilityTriggerDelay);

        return 1;
    }
}
