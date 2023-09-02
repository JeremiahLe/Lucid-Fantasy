using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "StormcowlOnRoundStart", menuName = "Ability Triggers/Stormcowl/OnRoundStart")]
public class StormcowlOnRoundStart : IAbilityTrigger
{
    public AttackEffect EnrageAttackEffect;
    public override async Task<int> TriggerAbility(Monster abilitySourceMonster, GameObject abilitySourceMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, bool displayLogMessage)
    {
        await Task.Delay(abilityTriggerDelay);

        Debug.Log($"Triggering {abilitySourceMonster}'s {ability.abilityName} ability!", this);

        List<GameObject> listOfTargetMonsters;

        if (abilitySourceMonster.aiType == Monster.AIType.Ally)
            listOfTargetMonsters = monsterAttackManager.combatManagerScript.ListOfEnemies;
        else
            listOfTargetMonsters = monsterAttackManager.combatManagerScript.ListOfAllys;

        foreach (GameObject enemyMonster in listOfTargetMonsters.ToArray())
        {
            if (enemyMonster == null)
                continue;

            Monster monsterReference = enemyMonster.GetComponent<CreateMonster>().monsterReference;

            if (monsterReference.monsterElement.element == ElementClass.MonsterElement.Electric || monsterReference.monsterSubElement.element == ElementClass.MonsterElement.Electric)
            {
                MonsterAttack blankAttack = new MonsterAttack(ability.abilityName, EnrageAttackEffect.elementClass, EnrageAttackEffect.effectDamageType, 0, 0, abilitySourceMonster, abilitySourceMonsterGameObject);
                EnrageAttackEffect.monsterAttackTrigger = blankAttack;
                await EnrageAttackEffect.TriggerEffects(monsterReference, enemyMonster, monsterAttackManager, EnrageAttackEffect.name, blankAttack, displayLogMessage);
            }

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }
}
