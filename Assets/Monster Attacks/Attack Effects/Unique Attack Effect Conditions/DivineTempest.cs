using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "DivineTempest", menuName = "UniqueAttackEffects/DivineTempest")]
public class DivineTempest : IAbilityTrigger
{
    public override async Task<int> TriggerAttackEffect(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, AttackEffect attackEffect)
    {
        Debug.Log($"Unique Attack Effect Called! {attackEffect.name}");

        foreach (GameObject allyMonsterGameObject in monsterAttackManager.combatManagerScript.ListOfAllys.ToArray())
        {
            if (allyMonsterGameObject == null)
                continue;

            Monster allyMonsterReference = allyMonsterGameObject.GetComponent<CreateMonster>().monsterReference;

            await attackEffect.AffectTargetStat(allyMonsterReference, allyMonsterGameObject, monsterAttackManager, attackEffect.name);

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }
}
