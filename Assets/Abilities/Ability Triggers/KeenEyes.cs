using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "KeenEyes", menuName = "Ability Triggers/KeenEyes/1")]
public class KeenEyes : IAbilityTrigger
{
    public List<AttackEffect> currentAttackEffectTriggered;

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, bool displayLogMessage)
    {
        await Task.Delay(abilityTriggerDelay);

        if (targetMonsterGameObject != null)
            targetMonsterGameObject.GetComponent<CreateMonster>().monsterCannotMissAttacks = true;

        await Task.Delay(abilityTriggerDelay);

        return 1;
    }
}
