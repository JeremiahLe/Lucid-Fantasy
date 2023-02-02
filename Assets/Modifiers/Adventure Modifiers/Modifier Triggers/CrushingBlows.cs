using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "CrushingBlows", menuName = "AdventureModifierTriggers/AddAttackEffects/CrushingBlows")]
public class CrushingBlows : IAbilityTrigger
{
    public AttackEffect OnPostAttackEffect;

    public AttackEffect OnDuringAttackEffect;

    public override async Task<int> TriggerModifier(CombatManagerScript combatManagerScript, Monster.AIType aiType, AttackEffect.EffectTime effectTime)
    {
        MonsterAttack currentMonsterAttack = combatManagerScript.monsterAttackManager.currentMonsterAttack;

        if (currentMonsterAttack.monsterAttackType != MonsterAttack.MonsterAttackType.Attack)
            return 1;

        if (currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.SingleTarget && effectTime == AttackEffect.EffectTime.PostAttack)
        {
            GameObject targetMonsterGameObject = combatManagerScript.monsterAttackManager.currentTargetedMonsterGameObject;
            Monster targetMonster = null;

            if (targetMonsterGameObject == null)
                return 1;

            if (targetMonsterGameObject.TryGetComponent(out CreateMonster monsterComponent))
                targetMonster = monsterComponent.monsterReference;

            await Task.Delay(abilityTriggerDelay);

            await OnPostAttackEffect.TriggerEffects(targetMonster, targetMonsterGameObject, combatManagerScript.monsterAttackManager, OnPostAttackEffect.name);

            return 1;
        }

        if (currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.AllTargets && effectTime == AttackEffect.EffectTime.DuringAttack)
        {
            GameObject targetMonsterGameObject = combatManagerScript.monsterAttackManager.currentTargetedMonsterGameObject;
            Monster targetMonster = null;

            if (targetMonsterGameObject.TryGetComponent(out CreateMonster monsterComponent))
                targetMonster = monsterComponent.monsterReference;

            await Task.Delay(abilityTriggerDelay);

            await OnDuringAttackEffect.TriggerEffects(targetMonster, targetMonsterGameObject, combatManagerScript.monsterAttackManager, OnDuringAttackEffect.name);

            return 1;
        }

        return 1;
    }
}
