using System; // allows serialization of custom classes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Monster Attack Effect", menuName = "Monster Attack Effects")]
public class AttackEffect : ScriptableObject
{
    public enum TypeOfEffect { MinorDrain, MagicalAttackBuffSelf, HalfHealthExecute, SpeedBuffAllies }
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

            case TypeOfEffect.MagicalAttackBuffSelf:
                targetMonster = monsterAttackManager.currentMonsterTurn; // should still be the monster who used the move, NOT the one next in Queue
                targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                MagicalAttackBuffSelf(targetMonster, monsterAttackManager, targetMonsterGameObject, targetMonster.magicAttack);
                break;

            case TypeOfEffect.HalfHealthExecute:
                targetMonster = monsterAttackManager.currentMonsterTurn; // should still be the monster who used the move, NOT the one next in Queue
                targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                HalfHealthExecute(targetMonster, monsterAttackManager, targetMonsterGameObject);
                break;

            case TypeOfEffect.SpeedBuffAllies:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = Instantiate(monsterAttackManager.currentMonsterTurn); // should still be the monster who used the move, NOT the one next in Queue
                    targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                    SpeedBuffAllies(targetMonster, monsterAttackManager, targetMonsterGameObject);
                }
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
        if (toValue <= 1)
        {
            toValue = 1; // prevent buffs of 0
        }
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

    // Minor Drain function
    public void MagicalAttackBuffSelf(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, float modifier)
    {
        // Calculate buff magical attack (25% percent of current magic attack)
        float fromValue = monsterReference.magicAttack;
        float toValue = Mathf.RoundToInt(modifier * amountToChange / 100);
        if (toValue <= 1)
        {
            toValue = 1; // prevent buffs of 0
        }
        monsterReference.magicAttack += toValue;

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} raised its {statEnumToChange.ToString()}!");

        // Update monster's UI health element
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats();
    }

    // Half health execute
    public void HalfHealthExecute(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Is target's health 50% or less of maximum?
        float currentHealth = monsterAttackManager.currentTargetedMonster.health;
        float maxHealth = monsterAttackManager.currentTargetedMonster.maxHealth;
        if (currentHealth / maxHealth <= 0.5f)
        {
            // Send execute message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} " +
                $"executed {monsterAttackManager.currentTargetedMonster.aiType} {monsterAttackManager.currentTargetedMonster.name}!");

            // remove monster
            monsterAttackManager.currentTargetedMonster.health = 0;
        }
    }

    // Speed buff allies
    public void SpeedBuffAllies(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        combatManagerScript = monsterAttackManager.combatManagerScript;

        if (monsterReferenceGameObject == null)
        {
            return;
        }

        if (monsterReference != null)
        {
            // Calculate speed buff (% of current speed)
            if (monsterReference.aiType == Monster.AIType.Ally)
            {
                foreach (GameObject monsterObj in combatManagerScript.ListOfAllys.ToArray()) //ToArray avoids missing lists
                {
                    if (monsterObj == null)
                    {
                        continue;
                    }

                    Monster monster = monsterObj.GetComponent<CreateMonster>().monsterReference;

                    if (monster.health <= 0) // fixed dual combat log calls
                    {
                        continue;
                    }

                    float fromValue = monster.speed;
                    float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
                    if (toValue <= 1)
                    {
                        toValue = 1; // prevent buffs of 0
                    }
                    monster.speed += toValue;
                    monsterObj.GetComponent<CreateMonster>().monsterSpeed += (int)toValue;

                    // Send speed buff message to combat log
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} raised its {statEnumToChange.ToString()}!");

                    // Update monster's speed element
                    monsterObj.GetComponent<CreateMonster>().UpdateStats();

                    // Does this actually work?
                    combatManagerScript.SortMonsterBattleSequence();
                }
            }
            else if (monsterReference.aiType == Monster.AIType.Enemy)
            {
                foreach (GameObject monsterObj in combatManagerScript.ListOfEnemies) //ToArray avoids missing lists
                {
                    if (monsterObj == null)
                    {
                        continue;
                    }

                    Monster monster = monsterObj.GetComponent<CreateMonster>().monsterReference;

                    float fromValue = monster.speed;
                    float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
                    if (toValue <= 1)
                    {
                        toValue = 1; // prevent buffs of 0
                    }
                    monster.speed += toValue;
                    monsterObj.GetComponent<CreateMonster>().monsterSpeed += (int)toValue;

                    // Send speed buff message to combat log
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} raised its {statEnumToChange.ToString()}!");

                    // Update monster's speed element
                    monsterObj.GetComponent<CreateMonster>().UpdateStats();

                    // Does this actually work?
                    combatManagerScript.SortMonsterBattleSequence();
                }
            }
        }
    }
}
