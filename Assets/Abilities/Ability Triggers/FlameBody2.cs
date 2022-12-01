using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "FlameBody2", menuName = "Ability Triggers/FlameBodyPassive/2")]
public class FlameBody2 : IAbilityTrigger
{
    public List<AttackEffect> currentAttackEffectTriggered;

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, MonsterAttack attackTrigger)
    {
        await Task.Delay(abilityTriggerDelay);

        if (monsterAttackManager.currentMonsterAttack.monsterAttackDamageType == MonsterAttack.MonsterAttackDamageType.Physical)
        {
            foreach (AttackEffect attackEffect in currentAttackEffectTriggered)
            {
                attackEffect.TriggerEffects(monsterAttackManager, ability.abilityName, attackTrigger);

                await Task.Delay(abilityTriggerDelay);
            }
        }

        return 1;
    }
}
