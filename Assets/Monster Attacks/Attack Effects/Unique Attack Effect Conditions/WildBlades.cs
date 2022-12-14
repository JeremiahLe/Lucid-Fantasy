using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "WildBlades", menuName = "UniqueAttackEffects/WildBlades")]
public class WildBlades : IAbilityTrigger
{
    public override async Task<int> TriggerAttackEffect(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, AttackEffect attackEffect)
    {
        Debug.Log("Unique Attack Effect Called!");

        if (targetMonsterGameObject.TryGetComponent(out CreateMonster monsterComponent) != false)
        {
            if (monsterComponent.monsterCriticallyStrikedThisRound)
            {
                Debug.Log("Unique Attack Effect Condition Met!");

                await attackEffect.AffectTargetStat(targetMonster, targetMonsterGameObject, monsterAttackManager, attackEffect.name);

                monsterComponent.monsterCriticallyStrikedThisRound = false;

                await Task.Delay(abilityTriggerDelay);
            }
        }

        return 1;
    }
}
