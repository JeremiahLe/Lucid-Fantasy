using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Linq;

[Serializable]
[CreateAssetMenu(fileName = "BlessingOfEarth", menuName = "AdventureModifierTriggers/BlessingOfEarth")]
public class BlessingOfEarth : IAbilityTrigger
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

        GameObject monsterObj = targetMonsterList.OrderByDescending(monsterWithLowestHealth => monsterWithLowestHealth.GetComponent<CreateMonster>().monsterReference.health / monsterWithLowestHealth.GetComponent<CreateMonster>().monsterReference.maxHealth).ToList().Last();
        
        Monster targetMonster = monsterObj.GetComponent<CreateMonster>().monsterReference;

        if (targetMonster.health / targetMonster.maxHealth == 1)
        {
            return 1;
        }

        combatManagerScript.CombatLog.SendMessageToCombatLog($"Activating {aiType} {adventureModifier.modifierName}!");

        foreach (AttackEffect modifierEffect in listOfAdventureModifierEffects)
        {
            await modifierEffect.TriggerEffects(targetMonster, monsterObj, combatManagerScript.monsterAttackManager, adventureModifier.modifierName);

            await Task.Delay(abilityTriggerDelay);
        }

        await Task.Delay(abilityTriggerDelay);

        return 1;
    }
}