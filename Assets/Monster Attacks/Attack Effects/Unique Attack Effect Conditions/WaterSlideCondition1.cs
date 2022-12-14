using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "WaterSlideCondition1", menuName = "UniqueAttackEffects/WaterSlide/1")]
public class WaterSlideCondition1 : IAbilityTrigger
{
    public override async Task<int> TriggerAttackEffect(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, AttackEffect attackEffect)
    {
        Debug.Log("Unique Attack Effect Called!");

        if (monsterAttackManager.currentMonsterTurn.monsterElement.element == ElementClass.MonsterElement.Water || monsterAttackManager.currentMonsterTurn.monsterSubElement.element == ElementClass.MonsterElement.Water)
        {
            Debug.Log("Unique Attack Effect Condition Met!");

            await attackEffect.AffectTargetStat(targetMonster, targetMonsterGameObject, monsterAttackManager, attackEffect.name);

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }
}

