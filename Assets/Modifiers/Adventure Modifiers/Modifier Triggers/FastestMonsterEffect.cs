using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Linq;
[Serializable]
[CreateAssetMenu(fileName = "FastestMonsterEffect", menuName = "AdventureModifierTriggers/FastestMonsterEffect")]
public class FastestMonsterEffect : IAbilityTrigger
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

        GameObject monsterObj = targetMonsterList.OrderByDescending(monster => monster.GetComponent<CreateMonster>().monsterReference.speed).ToList().First();
        Monster targetMonster = monsterObj.GetComponent<CreateMonster>().monsterReference;

        foreach (AttackEffect modifierEffect in listOfAdventureModifierEffects)
        {
            await modifierEffect.TriggerEffects(targetMonster, monsterObj, combatManagerScript.monsterAttackManager, adventureModifier.modifierName);

            await Task.Delay(abilityTriggerDelay);
        }

        await Task.Delay(abilityTriggerDelay);

        return 1;
    }
}
