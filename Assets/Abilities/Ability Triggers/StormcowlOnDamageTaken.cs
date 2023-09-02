using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "StormcowlOnDamageTaken", menuName = "Ability Triggers/Stormcowl/OnDamageTaken")]
public class StormcowlOnDamageTaken : IAbilityTrigger
{
    public List<AttackEffect> currentAttackEffectTriggered;

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, MonsterAttack attackTrigger, bool displayLogMessage)
    {
        await Task.Delay(abilityTriggerDelay);

        //Debug.Log($"Triggering {targetMonster}'s {ability.abilityName} ability!", this);

        if (monsterAttackManager.currentMonsterAttack.monsterAttackElement == ElementClass.MonsterElement.Electric)
        {
            Debug.Log($"{targetMonster}'s {ability.abilityName} ability condition passed!", this);

            foreach (AttackEffect attackEffect in currentAttackEffectTriggered)
            {
                await attackEffect.TriggerEffects(targetMonster, targetMonsterGameObject, monsterAttackManager, ability.abilityName, monsterAttackManager.currentMonsterAttack, displayLogMessage);

                await Task.Delay(abilityTriggerDelay);
            }
        }

        return 1;
    }
}
