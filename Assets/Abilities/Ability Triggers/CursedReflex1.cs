using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "CursedReflex1", menuName = "Ability Triggers/CursedReflex/1")]
public class CursedReflex1 : IAbilityTrigger
{
    public List<AttackEffect> currentAttackEffectTriggered;

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, MonsterAttack attackTrigger)
    {
        await Task.Delay(abilityTriggerDelay);


        foreach (AttackEffect attackEffect in currentAttackEffectTriggered)
        {
            attackEffect.monsterAttackTrigger = attackTrigger;
            attackEffect.monsterSource = targetMonster;
            //attackEffect.monsterAttackTrigger.monsterAttackSource = targetMonster;
            //attackEffect.monsterAttackTrigger.monsterAttackSourceGameObject = targetMonsterGameObject;

            await attackEffect.TriggerEffects(targetMonster, targetMonsterGameObject, monsterAttackManager, ability.abilityName, attackTrigger);

            await Task.Delay(abilityTriggerDelay);
        }

        foreach (AttackEffect attackEffect in currentAttackEffectTriggered)
        {
            attackEffect.monsterAttackTrigger = attackTrigger;
            attackEffect.monsterSource = targetMonster;
            //attackEffect.monsterAttackTrigger.monsterAttackSource = targetMonster;
            //attackEffect.monsterAttackTrigger.monsterAttackSourceGameObject = targetMonsterGameObject;

            await attackEffect.TriggerEffects(monsterAttackManager.currentMonsterTurn, monsterAttackManager.currentMonsterTurnGameObject, monsterAttackManager, ability.abilityName, attackTrigger);

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }
}