using System; // allows serialization of custom classes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Monster Attack Effect", menuName = "Monster Attack Effects")]
public class AttackEffect : ScriptableObject
{
    public enum TypeOfEffect { MinorDrain }
    public TypeOfEffect typeOfEffect;

    public enum StatEnumToChange { Health, Mana, PhysicalAttack, MagicAttack, PhysicalDefense, MagicDefense, Speed, Evasion, CritChance }
    public StatEnumToChange statEnumToChange;

    public float amountToChange;

    public CombatManagerScript combatManagerScript;

    // Initial function that is called by monsterAttackManager that enacts attack after effects
    public void TriggerEffects(MonsterAttackManager monsterAttackManager)
    {
        switch (typeOfEffect)
        {
            case TypeOfEffect.MinorDrain:
                Monster targetMonster = monsterAttackManager.currentMonsterTurn; // should still be the monster who used the move, NOT the one next in Queue
                GameObject targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                MinorDrain(targetMonster, monsterAttackManager, targetMonsterGameObject, monsterAttackManager.cachedDamage);
                break;

            default:
                Debug.Log("Missing effect or attack reference?", this);
                break;
        }
    }

    // Minor Drain function
    public void MinorDrain(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, float modifier)
    {
        // Calculate drained health (percentage of damage dealt)
        float fromValue = monsterReference.health;
        float toValue = Mathf.RoundToInt(modifier * amountToChange / 100);
        monsterReference.health += toValue;

        // Send drain health message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} drained {toValue} health!");

        // Make sure health doesn't exceed maximum
        if (monsterReference.health > monsterReference.maxHealth)
        {
            monsterReference.health = monsterReference.maxHealth;
        }

        // Update monster's UI health element
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats();
    }
}
