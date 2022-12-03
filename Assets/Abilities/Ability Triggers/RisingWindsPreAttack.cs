using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "RisingWinds", menuName = "Ability Triggers/RisingWinds")]
public class RisingWindsPreAttack : IAbilityTrigger
{
    public AttackEffect currentAttackEffectTriggered;
    public override async Task<int> TriggerAbility(Monster abilityMonster, GameObject abilityMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, MonsterAttack attackTrigger)
    {
        await Task.Delay(abilityTriggerDelay);

        Debug.Log($"Triggering {abilityMonster}'s {ability.abilityName} ability!", this);

        GameObject targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
        Monster targetMonster = targetMonsterGameObject.GetComponent<CreateMonster>().monsterReference;

        float bonusDamage = (abilityMonster.speed - targetMonster.speed) / 100f;

        if (bonusDamage <= 0)
            bonusDamage = 0.01f;

        attackTrigger.monsterAttackFlatDamageBonus += bonusDamage;

        monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{abilityMonster.aiType} {abilityMonster.name} gained {bonusDamage * 100}% bonus damage from its {ability.name} ability!");

        return 1;
    }
}
