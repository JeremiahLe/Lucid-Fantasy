using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "CripplingFearCondition1", menuName = "UniqueAttackEffects/CripplingFear/1")]
public class CripplingFearCondition1 : IAbilityTrigger
{
    public List<AttackEffect> currentAttackEffectTriggered;

    public override async Task<int> TriggerAttackEffect(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, AttackEffect attackEffect)
    {
        Debug.Log("Unique Attack Effect Called!");

        if (targetMonster.health <= 0)
            return 1;

        if (monsterAttackManager.combatManagerScript.BattleSequence.IndexOf(attackEffect.monsterAttackTrigger.monsterAttackSourceGameObject) == monsterAttackManager.combatManagerScript.BattleSequence.Count - 1)
        {
            Debug.Log("Unique Attack Effect Condition Met!");

            foreach (GameObject monster in monsterAttackManager.ListOfCurrentlyTargetedMonsters)
            {
                if (monster == null)
                    continue;

                targetMonster = monster.GetComponent<CreateMonster>().monsterReference;
                targetMonsterGameObject = monster;

                currentAttackEffectTriggered[0].monsterAttackTrigger = attackEffect.monsterAttackTrigger;
                currentAttackEffectTriggered[0].monsterAttackTrigger.monsterAttackSource = attackEffect.monsterAttackTrigger.monsterAttackSource;
                currentAttackEffectTriggered[0].monsterAttackTrigger.monsterAttackSourceGameObject = attackEffect.monsterAttackTrigger.monsterAttackSourceGameObject;

                await currentAttackEffectTriggered[0].AffectTargetStat(targetMonster, targetMonsterGameObject, monsterAttackManager, currentAttackEffectTriggered[0].name);

                await Task.Delay(abilityTriggerDelay);
            }
        }
        else
        {
            if (targetMonster.health <= 0)
                return 1;

            targetMonster = attackEffect.monsterAttackTrigger.monsterAttackSource;
            targetMonsterGameObject = attackEffect.monsterAttackTrigger.monsterAttackSourceGameObject;

            currentAttackEffectTriggered[1].monsterAttackTrigger = attackEffect.monsterAttackTrigger;
            currentAttackEffectTriggered[1].monsterAttackTrigger.monsterAttackSource = attackEffect.monsterAttackTrigger.monsterAttackSource;
            currentAttackEffectTriggered[1].monsterAttackTrigger.monsterAttackSourceGameObject = attackEffect.monsterAttackTrigger.monsterAttackSourceGameObject;

            await currentAttackEffectTriggered[1].AffectTargetStat(targetMonster, targetMonsterGameObject, monsterAttackManager, currentAttackEffectTriggered[1].name);

            await Task.Delay(abilityTriggerDelay);

            currentAttackEffectTriggered[2].monsterAttackTrigger = attackEffect.monsterAttackTrigger;
            currentAttackEffectTriggered[2].monsterAttackTrigger.monsterAttackSource = attackEffect.monsterAttackTrigger.monsterAttackSource;
            currentAttackEffectTriggered[2].monsterAttackTrigger.monsterAttackSourceGameObject = attackEffect.monsterAttackTrigger.monsterAttackSourceGameObject;

            await currentAttackEffectTriggered[2].AffectTargetStat(targetMonster, targetMonsterGameObject, monsterAttackManager, currentAttackEffectTriggered[2].name);

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }
}
