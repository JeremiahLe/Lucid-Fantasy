using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "New Ability Trigger", menuName = "Ability Triggers")]
public abstract class IAbilityTrigger : ScriptableObject
{
    public int abilityTriggerDelay = 150;

    public AttackEffect.EffectTime abilityTriggerTime;

    public virtual async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability)
    {
        return 1;
    }

    public virtual async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, MonsterAttack attackTrigger)
    {
        return 1;
    }


    public virtual async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, Modifier modifier)
    {
        return 1;
    }

    public virtual async Task<int> TriggerAttackEffect(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, AttackEffect attackEffect)
    {
        return 1;
    }

    public virtual async Task<int> TriggerAttackEffect(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, string effectName)
    {
        return 1;
    }

    public virtual async Task<int> TriggerItem(Item currentItem, CombatManagerScript combatManagerScript)
    {
        return 1;
    }

    public virtual async Task<int> TriggerModifier(CombatManagerScript combatManagerScript, Monster.AIType aiType, AttackEffect.EffectTime effectTime)
    {
        return 1;
    }

    public virtual async Task<int> TriggerModifier(AdventureManager adventureManager)
    {
        return 1;
    }
}
