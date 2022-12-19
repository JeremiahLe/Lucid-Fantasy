using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;


[Serializable]
[CreateAssetMenu(fileName = "HealingSolvent", menuName = "UniqueAttackEffects/HealingSolvent")]
public class HealingSolvent : IAbilityTrigger
{
    public override async Task<int> TriggerAttackEffect(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, AttackEffect attackEffect)
    {
        Debug.Log("Unique Attack Effect Called!");

        if (targetMonsterGameObject.TryGetComponent(out CreateMonster monsterComponent) != false)
        {
            foreach (Modifier modifier in targetMonster.ListOfModifiers)
            {
                if (modifier.statChangeType == AttackEffect.StatChangeType.Debuff || modifier.isStatusEffect)
                {
                    await modifier.ResetModifiedStat(targetMonster, targetMonsterGameObject);
                }

                await Task.Delay(abilityTriggerDelay);
            }

            if (attackEffect.CheckAttackEffectHit())
            {   
                // Increase physical defense
                AttackEffect tempAttackEffect = Instantiate(attackEffect);

                tempAttackEffect.effectTriggerChance = 100f;

                tempAttackEffect.statToChange = AttackEffect.StatToChange.PhysicalDefense;

                await tempAttackEffect.AffectTargetStat(targetMonster, targetMonsterGameObject, monsterAttackManager, attackEffect.monsterAttackTrigger.monsterAttackName);

                // Increase magic defense
                await Task.Delay(abilityTriggerDelay);

                tempAttackEffect.statToChange = AttackEffect.StatToChange.MagicDefense;

                await tempAttackEffect.AffectTargetStat(targetMonster, targetMonsterGameObject, monsterAttackManager, attackEffect.monsterAttackTrigger.monsterAttackName);
            }

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }
}
