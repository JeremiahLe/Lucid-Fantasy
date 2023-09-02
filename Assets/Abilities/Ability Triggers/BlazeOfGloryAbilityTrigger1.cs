using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "BlazeOfGloryAbilityTrigger", menuName = "Ability Triggers/BlazeOfGlory/1")]
public class BlazeOfGloryAbilityTrigger1 : IAbilityTrigger
{
    public AttackEffect BlazeOfGloryBurnAll;
    public AttackEffect BlazeOfGloryDamage;

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, MonsterAttack attackTrigger, bool displayLogMessage)
    {
        foreach (GameObject _targetMonsterGameObject in monsterAttackManager.combatManagerScript.BattleSequence.ToArray())
        {
            //await Task.Delay(abilityTriggerDelay); // Weird Self-Kill recoil missing source bug

            if (_targetMonsterGameObject == null)
                continue;

            CreateMonster monsterComponent = _targetMonsterGameObject.GetComponent<CreateMonster>();
            Monster _targetMonster = monsterComponent.monsterReference;

            if (monsterComponent.listofCurrentStatusEffects.Contains(Modifier.StatusEffectType.Burning))
            {
                // Adjust missing target
                monsterAttackManager.currentTargetedMonsterGameObject = _targetMonsterGameObject;
                monsterAttackManager.currentTargetedMonster = monsterComponent.monsterReference;

                BlazeOfGloryDamage.monsterSource = targetMonster;
                await BlazeOfGloryDamage.TriggerEffects(_targetMonster, _targetMonsterGameObject, monsterAttackManager, ability.abilityName, monsterAttackManager.currentMonsterAttack, displayLogMessage);

                await Task.Delay(abilityTriggerDelay);
                continue;
            }

            await BlazeOfGloryBurnAll.TriggerEffects(_targetMonster, _targetMonsterGameObject, monsterAttackManager, ability.abilityName, monsterAttackManager.currentMonsterAttack, displayLogMessage);

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }

}
