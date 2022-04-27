using System; // allows serialization of custom classes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
[CreateAssetMenu(fileName = "New Monster Attack Effect", menuName = "Monster Attack Effects")]
public class AttackEffect : ScriptableObject
{
    public enum TypeOfEffect { MinorDrain, MagicalAttackBuffSelf, HalfHealthExecute, SpeedBuffAllies, Counter75, DoublePowerIfStatBoost, OnCriticalStrikeBuff, DamageAllEnemies, CripplingFearEffect }
    public TypeOfEffect typeOfEffect;

    public enum StatEnumToChange { Health, Mana, PhysicalAttack, MagicAttack, PhysicalDefense, MagicDefense, Speed, Evasion, CritChance }
    public StatEnumToChange statEnumToChange;

    public enum EffectTime { PreAttack, PostAttack }
    public EffectTime effectTime;

    [PropertyRange(0, 100)]
    public float amountToChange;

    [DisplayWithoutEdit] public CombatManagerScript combatManagerScript;

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
                    targetMonster = Instantiate(monsterAttackManager.currentMonsterTurn); // should still be the monster who used the move, NOT the one next in Queue // WHY THE FUCK IS THERE INSTANTIATION GOING ON
                    targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                    SpeedBuffAllies(targetMonster, monsterAttackManager, targetMonsterGameObject);
                }
                break;

            case TypeOfEffect.DoublePowerIfStatBoost:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = Instantiate(monsterAttackManager.currentMonsterTurn); // should still be the monster who used the move, NOT the one next in Queue // WHY THE FUCK IS THERE INSTANTIATION GOING ON
                    targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                    DoublePowerIfStatBoost(targetMonster, monsterAttackManager, targetMonsterGameObject);
                }
                break;

            case TypeOfEffect.Counter75:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = Instantiate(monsterAttackManager.currentMonsterTurn); // should still be the monster who used the move, NOT the one next in Queue // WHY THE FUCK IS THERE INSTANTIATION GOING ON
                    targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                    Counter75(targetMonster, monsterAttackManager, targetMonsterGameObject);
                }
                break;

            case TypeOfEffect.OnCriticalStrikeBuff:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = monsterAttackManager.currentMonsterTurn; // should still be the monster who used the move, NOT the one next in Queue
                    targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                    OnCriticalStrikeBuff(targetMonster, monsterAttackManager, targetMonsterGameObject);
                }
                break;

            case TypeOfEffect.DamageAllEnemies:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = monsterAttackManager.currentMonsterTurn; // should still be the monster who used the move, NOT the one next in Queue
                    targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                    DamageAllEnemies(targetMonster, monsterAttackManager, targetMonsterGameObject);
                }
                break;

            case TypeOfEffect.CripplingFearEffect:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = monsterAttackManager.currentMonsterTurn; // should still be the monster who used the move, NOT the one next in Queue
                    targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                    CripplingFearEffect(targetMonster, monsterAttackManager, targetMonsterGameObject);
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
        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;
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

    // Half health execute
    public void DamageAllEnemies(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        combatManagerScript = monsterAttackManager.combatManagerScript;

        if (monsterReference.aiType == Monster.AIType.Ally)
        {
            // Get all other enemies that weren't the main target
            foreach (GameObject monster in combatManagerScript.ListOfEnemies.ToArray())
            {
                if (monster == combatManagerScript.CurrentTargetedMonster || monster == null)
                {
                    continue;
                }

                // First set the monster targeted then get damage calced
                //monsterAttackManager.currentTargetedMonster = monster.GetComponent<CreateMonster>().monsterReference;
                //monsterAttackManager.currentTargetedMonsterGameObject = monster;

                monsterAttackManager.DealDamageOthers(monster);
            }
        }
        else if (monsterReference.aiType == Monster.AIType.Enemy)
        {
            // Get all other enemies that weren't the main target
            foreach (GameObject monster in combatManagerScript.ListOfAllys.ToArray())
            {
                if (monster == monsterAttackManager.currentMonsterTurnGameObject || monster == null)
                {
                    continue;
                }

                // First set the monster targeted then get damage calced
                monsterAttackManager.DealDamageOthers(monster);
            }
        }
    }

    // Double power of monster attack
    public void DoublePowerIfStatBoost(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Check if the monster had recieved a stat boost this round
        if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound)
        {
            monsterAttackManager.currentMonsterAttack.monsterAttackDamage *= 2f;

            // Send buff message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} had it's power doubled!");
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
                    monsterObj.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

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
                    monsterObj.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

                    // Does this actually work?
                    combatManagerScript.SortMonsterBattleSequence();
                }
            }
        }
    }

    // Nerf all enemies speed if user's speed is the slowest in the battle. Otherwise, lower user's attack stats
    public void CripplingFearEffect(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        combatManagerScript = monsterAttackManager.combatManagerScript;

        if (monsterReferenceGameObject == null)
        {
            return;
        }

        if (monsterReference != null)
        {
            // If user's position in the battle order is the last index, Count - 1;
            if (combatManagerScript.BattleSequence.IndexOf(monsterReferenceGameObject) == combatManagerScript.BattleSequence.Count - 1)
            {
                if (monsterReference.aiType == Monster.AIType.Ally)
                {
                    foreach (GameObject monsterObj in combatManagerScript.ListOfEnemies.ToArray()) //ToArray avoids missing lists
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

                        // Calculate speed nerf (% of current speed)
                        float fromValue = monster.speed;
                        float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
                        if (toValue <= 1)
                        {
                            toValue = 1; // prevent buffs of 0
                        }
                        monster.speed -= toValue;
                        monsterObj.GetComponent<CreateMonster>().monsterSpeed -= (int)toValue;

                        // Send speed buff message to combat log
                        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name}'s {statEnumToChange.ToString()} was lowered!");

                        // Update monster's speed element
                        monsterObj.GetComponent<CreateMonster>().UpdateStats();
                        monsterObj.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

                        // Does this actually work?
                        combatManagerScript.SortMonsterBattleSequence();
                    }
                }
                else 
                if (monsterReference.aiType == Monster.AIType.Enemy)
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

                        // Calculate speed nerf (% of current speed)
                        float fromValue = monster.speed;
                        float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
                        if (toValue <= 1)
                        {
                            toValue = 1; // prevent buffs of 0
                        }
                        monster.speed -= toValue;
                        monsterObj.GetComponent<CreateMonster>().monsterSpeed -= (int)toValue;

                        // Send speed buff message to combat log
                        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name}'s {statEnumToChange.ToString()} was lowered!");

                        // Update monster's speed element
                        monsterObj.GetComponent<CreateMonster>().UpdateStats();
                        monsterObj.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

                        // Does this actually work?
                        combatManagerScript.SortMonsterBattleSequence();
                    }
                }
            }
            else // the user's not the slowest, so reduce their attack stats
            {
                LowerOffensiveStats(monsterReference, monsterAttackManager, monsterReferenceGameObject);
            }
        }
    }

    // 75% Counter
    public void Counter75(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Check the amount of damage taken this round
        monsterAttackManager.currentMonsterAttack.monsterAttackFlatDamageBonus = Mathf.RoundToInt(monsterReferenceGameObject.GetComponent<CreateMonster>().monsterDamageTakenThisRound * .75f);

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} gained {monsterAttackManager.currentMonsterAttack.monsterAttackFlatDamageBonus} additional damage!");
    }

    // Critical Strike Buff
    public void OnCriticalStrikeBuff(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Check if the monster critically struck with attack this round
        if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterCriticallyStrikedThisRound)
        {
            switch (statEnumToChange) {
                case (StatEnumToChange.PhysicalAttack):

                    // Calculate buff
                    float fromValue = monsterReference.physicalAttack;
                    float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
                    if (toValue <= 1)
                    {
                        toValue = 1; // prevent buffs of 0
                    }
                    monsterReference.physicalAttack += toValue;
                    Debug.Log($"{fromValue} + {toValue}");

                    // Send buff message to combat log
                    combatManagerScript = monsterAttackManager.combatManagerScript;
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} raised its {statEnumToChange.ToString()}!");

                    // Update monster's UI health element
                    monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats();
                    monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

                    break;

                case (StatEnumToChange.MagicAttack):

                    // Calculate buff
                    fromValue = monsterReference.physicalAttack;
                    toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
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
                    monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

                    break;

                default:
                    Debug.Log("Missing enum type?", this);
                    break;
        }
        }


    }

    // Lower Offensive Stats
    public void LowerOffensiveStats(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Physical Attack nerf (20%)
        float fromValue = monsterReference.physicalAttack;
        float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
        if (toValue <= 1)
        {
            toValue = 1; // prevent buffs of 0
        }
        monsterReference.physicalAttack -= toValue;

        // Magic Attack nerf (20%)
        fromValue = monsterReference.magicAttack;
        toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
        if (toValue <= 1)
        {
            toValue = 1; // prevent buffs of 0
        }
        monsterReference.magicAttack -= toValue;

        // Send speed buff message to combat log
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} lowered its Physical and Magic Attack!");

        // Update monster's stats
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats();
        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;
    }
}
