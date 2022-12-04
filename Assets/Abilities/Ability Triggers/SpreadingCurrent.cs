using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "SpreadingCurrent", menuName = "Ability Triggers/SpreadingCurrent")]
public class SpreadingCurrent : IAbilityTrigger
{
    public AttackEffect physicalAttackBuffEffect;
    public AttackEffect magicAttackBuffEffect;
    public override async Task<int> TriggerAbility(Monster abilityMonster, GameObject abilityMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability)
    {
        await Task.Delay(abilityTriggerDelay);

        Debug.Log($"Triggering {abilityMonster}'s {ability.abilityName} ability!", this);

        foreach (GameObject enemyMonster in monsterAttackManager.combatManagerScript.ListOfEnemies)
        {
            Monster monsterReference = enemyMonster.GetComponent<CreateMonster>().monsterReference;

            if (monsterReference.monsterElement.element == ElementClass.MonsterElement.Time || monsterReference.monsterElement.element == ElementClass.MonsterElement.Water ||
                monsterReference.monsterSubElement.element == ElementClass.MonsterElement.Time || monsterReference.monsterSubElement.element == ElementClass.MonsterElement.Water)
            {
                Debug.Log($"Time or Water element monster found!", this);

                foreach (GameObject allyMonster in monsterAttackManager.combatManagerScript.ListOfAllys)
                {
                    Monster allyMonsterReference = allyMonster.GetComponent<CreateMonster>().monsterReference;

                    if (allyMonsterReference.monsterElement.element == ElementClass.MonsterElement.Electric 
                        || allyMonsterReference.monsterSubElement.element == ElementClass.MonsterElement.Electric)
                    {
                        await physicalAttackBuffEffect.TriggerEffects(allyMonsterReference, allyMonster, monsterAttackManager, ability.abilityName, monsterAttackManager.currentMonsterAttack);

                        await Task.Delay(abilityTriggerDelay);

                        await magicAttackBuffEffect.TriggerEffects(allyMonsterReference, allyMonster, monsterAttackManager, ability.abilityName, monsterAttackManager.currentMonsterAttack);
                    }

                    await Task.Delay(abilityTriggerDelay);
                }

                return 1;
            }
        }

        Debug.Log($"Did not trigger {abilityMonster}'s {ability.abilityName} ability!", this);
        return 1;
    }
}
