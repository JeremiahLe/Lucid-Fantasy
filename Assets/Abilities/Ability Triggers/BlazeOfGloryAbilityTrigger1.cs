using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "BlazeOfGloryAbilityTrigger", menuName = "Ability Triggers/BlazeOfGlory/1")]
public class BlazeOfGloryAbilityTrigger1 : IAbilityTrigger
{
    public List<AttackEffect> listOfCurrentAttackEffects;

    public AttackEffect currentAttackEffectTriggered;

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, MonsterAttack attackTrigger)
    {
        foreach (GameObject _targetMonsterGameObject in monsterAttackManager.combatManagerScript.BattleSequence.ToArray())
        {
            if (_targetMonsterGameObject == null)
                continue;

            //Debug.Log($"Current Monster Health: {targetMonsterGameObject.GetComponent<CreateMonster>().monster.health}");

            CreateMonster monsterComponent = _targetMonsterGameObject.GetComponent<CreateMonster>();
            Monster _targetMonster = monsterComponent.monsterReference;

            if (monsterComponent.listofCurrentStatusEffects.Contains(Modifier.StatusEffectType.Burning))
            {
                currentAttackEffectTriggered = listOfCurrentAttackEffects[1];

                // Adjust missing target
                monsterAttackManager.currentTargetedMonsterGameObject = _targetMonsterGameObject;
                monsterAttackManager.currentTargetedMonster = monsterComponent.monsterReference;

                currentAttackEffectTriggered.monsterSource = targetMonster;
                await currentAttackEffectTriggered.AffectTargetStatByAnotherStat(_targetMonster, _targetMonsterGameObject, monsterAttackManager, ability.abilityName, attackTrigger);

                await Task.Delay(abilityTriggerDelay);
                continue;
            }

            if (!AttackEffect.CheckTargetIsImmune(_targetMonster, monsterAttackManager, _targetMonsterGameObject, currentAttackEffectTriggered))
            {
                currentAttackEffectTriggered = listOfCurrentAttackEffects[0];

                currentAttackEffectTriggered.InflictStatusEffect(_targetMonster, _targetMonsterGameObject, monsterAttackManager, ability.abilityName);
            }

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }

}
