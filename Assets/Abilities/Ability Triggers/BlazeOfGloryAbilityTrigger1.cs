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

    public override async Task<int> TriggerAbility(MonsterAttackManager monsterAttackManager, Ability ability)
    {
        foreach (GameObject targetMonsterGameObject in monsterAttackManager.combatManagerScript.BattleSequence.ToArray())
        {
            //if (targetMonsterGameObject == null || targetMonsterGameObject.GetComponent<CreateMonster>().monster.health <= 0)
                //continue;

            Debug.Log($"Current Monster Health: {targetMonsterGameObject.GetComponent<CreateMonster>().monster.health}");

            CreateMonster monsterComponent = targetMonsterGameObject.GetComponent<CreateMonster>();
            Monster targetMonster = monsterComponent.monster;

            if (monsterComponent.listofCurrentStatusEffects.Contains(Modifier.StatusEffectType.Burning))
            {
                currentAttackEffectTriggered = listOfCurrentAttackEffects[1];

                currentAttackEffectTriggered.AffectTargetStatByAnotherStat(targetMonster, targetMonsterGameObject, monsterAttackManager, ability.abilityName);

                await Task.Delay(abilityTriggerDelay);
                continue;
            }

            if (!AttackEffect.CheckTargetIsImmune(targetMonster, monsterAttackManager, targetMonsterGameObject, currentAttackEffectTriggered))
            {
                currentAttackEffectTriggered = listOfCurrentAttackEffects[0];

                currentAttackEffectTriggered.InflictStatusEffect(targetMonster, targetMonsterGameObject, monsterAttackManager, ability.abilityName);
            }

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }

}
