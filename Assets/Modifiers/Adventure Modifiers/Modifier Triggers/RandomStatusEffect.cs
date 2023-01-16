using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Linq;

[Serializable]
[CreateAssetMenu(fileName = "RandomStatusEffect", menuName = "AdventureModifierTriggers/RandomStatusEffect")]
public class RandomStatusEffect : IAbilityTrigger
{
    public Modifier adventureModifier;

    public AttackEffect statusEffect;

    public override async Task<int> TriggerModifier(CombatManagerScript combatManagerScript, Monster.AIType aiType, AttackEffect.EffectTime effectTime)
    {
        Debug.Log($"Triggering {adventureModifier.modifierName}!");

        List<GameObject> targetMonsterList;

        if (aiType == Monster.AIType.Ally)
            targetMonsterList = combatManagerScript.ListOfEnemies;
        else
            targetMonsterList = combatManagerScript.ListOfAllys;

        targetMonsterList = targetMonsterList.Where(monster => !monster.GetComponent<CreateMonster>().listofCurrentStatusEffects.Contains(statusEffect.attackEffectStatus)).ToList();

        if (targetMonsterList.Count <= 0)
            return 1;

        GameObject monsterObj =
            combatManagerScript.GetRandomTarget(targetMonsterList.Where(monster => !monster.GetComponent<CreateMonster>().listofCurrentStatusEffects.Contains(statusEffect.attackEffectStatus)).ToList());

        Monster targetMonster = monsterObj.GetComponent<CreateMonster>().monsterReference;

        await Task.Delay(abilityTriggerDelay);

        await statusEffect.TriggerEffects(targetMonster, monsterObj, combatManagerScript.monsterAttackManager, adventureModifier.modifierName);

        await Task.Delay(abilityTriggerDelay);

        return 1;
    }
}
