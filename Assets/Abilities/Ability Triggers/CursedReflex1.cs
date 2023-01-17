using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "CursedReflex1", menuName = "Ability Triggers/CursedReflex/1")]
public class CursedReflex1 : IAbilityTrigger
{
    public AttackEffect cursedReflexSlow;
    public AttackEffect cursedReflexEvasion;

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, MonsterAttack attackTrigger)
    {
        // Slow Attacker
        await Task.Delay(abilityTriggerDelay);

        cursedReflexSlow.monsterAttackTrigger = attackTrigger;
        cursedReflexSlow.monsterSource = targetMonster;

        await cursedReflexSlow.TriggerEffects(targetMonster, targetMonsterGameObject, monsterAttackManager, ability.abilityName, attackTrigger);

        // Reduce Attacker Evasion
        await Task.Delay(abilityTriggerDelay);

        cursedReflexEvasion.monsterAttackTrigger = attackTrigger;
        cursedReflexEvasion.monsterSource = targetMonster;

        await cursedReflexEvasion.TriggerEffects(monsterAttackManager.currentMonsterTurn, monsterAttackManager.currentMonsterTurnGameObject, monsterAttackManager, ability.abilityName, attackTrigger);

        // Slow Self
        await Task.Delay(abilityTriggerDelay);

        cursedReflexSlow.monsterAttackTrigger = attackTrigger;
        cursedReflexSlow.monsterSource = targetMonster;

        await cursedReflexSlow.TriggerEffects(monsterAttackManager.currentMonsterTurn, monsterAttackManager.currentMonsterTurnGameObject, monsterAttackManager, ability.abilityName, attackTrigger);

        await Task.Delay(abilityTriggerDelay);

        return 1;
    }
}