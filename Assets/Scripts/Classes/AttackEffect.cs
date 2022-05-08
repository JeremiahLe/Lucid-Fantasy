using System; // allows serialization of custom classes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
[CreateAssetMenu(fileName = "New Monster Attack Effect", menuName = "Monster Attack Effects")]
public class AttackEffect : ScriptableObject
{
    public enum TypeOfEffect { MinorDrain, MagicalAttackBuffSelf, HalfHealthExecute, SpeedBuffAllies, Counter,
        DoublePowerIfStatBoost, OnCriticalStrikeBuff, DamageAllEnemies, CripplingFearEffect, NonDamagingMove,
        SpeedBuffTarget, EvasionBuffTarget, AddBonusDamage, AddBonusDamageFlat, IncreaseOffensiveStats, HealthCut }

    public TypeOfEffect typeOfEffect;

    public enum StatEnumToChange { Health, Mana, PhysicalAttack, MagicAttack, PhysicalDefense, MagicDefense, Speed, Evasion, CritChance }
    public StatEnumToChange statEnumToChange;

    public enum EffectTime { PreAttack, PostAttack }
    public EffectTime effectTime;

    public bool modifierCalledOnce = false;

    [PropertyRange(0, 400)]
    public float amountToChange;

    [DisplayWithoutEdit] public CombatManagerScript combatManagerScript;

    public List<Modifier> ListOfModifiers;

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
                MagicalAttackBuffSelf(targetMonster, monsterAttackManager, targetMonsterGameObject);
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

            case TypeOfEffect.Counter:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = Instantiate(monsterAttackManager.currentMonsterTurn); // should still be the monster who used the move, NOT the one next in Queue // WHY THE FUCK IS THERE INSTANTIATION GOING ON
                    targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                    Counter(targetMonster, monsterAttackManager, targetMonsterGameObject);
                }
                break;

            case TypeOfEffect.AddBonusDamage:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = Instantiate(monsterAttackManager.currentMonsterTurn); // should still be the monster who used the move, NOT the one next in Queue // WHY THE FUCK IS THERE INSTANTIATION GOING ON
                    targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                    AddBonusDamage(targetMonster, monsterAttackManager, targetMonsterGameObject);
                }
                break;

            case TypeOfEffect.AddBonusDamageFlat:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = Instantiate(monsterAttackManager.currentMonsterTurn); // should still be the monster who used the move, NOT the one next in Queue // WHY THE FUCK IS THERE INSTANTIATION GOING ON
                    targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                    AddBonusDamageFlat(targetMonster, monsterAttackManager, targetMonsterGameObject);
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

            case TypeOfEffect.CripplingFearEffect:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = monsterAttackManager.currentMonsterTurn; // should still be the monster who used the move, NOT the one next in Queue
                    targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                    CripplingFearEffect(targetMonster, monsterAttackManager, targetMonsterGameObject);
                }
                break;

            case TypeOfEffect.SpeedBuffTarget:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = monsterAttackManager.combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
                    targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                    SpeedBuffTarget(targetMonster, monsterAttackManager, targetMonsterGameObject);
                }
                break;

            case TypeOfEffect.EvasionBuffTarget:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = monsterAttackManager.combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
                    targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                    EvasionBuffTarget(targetMonster, monsterAttackManager, targetMonsterGameObject);
                }
                break;

            case TypeOfEffect.IncreaseOffensiveStats:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = monsterAttackManager.combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
                    targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                    IncreaseOffensiveStats(targetMonster, monsterAttackManager, targetMonsterGameObject);
                }
                break;

            case TypeOfEffect.HealthCut:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = monsterAttackManager.combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
                    targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                    HealthCut(targetMonster, monsterAttackManager, targetMonsterGameObject);
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

    // MagicalAttackBuffSelf
    public void MagicalAttackBuffSelf(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Calculate buff magical attack (25% percent of current magic attack)
        float fromValue = monsterReference.magicAttack;
        float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
        if (toValue <= 1)
        {
            toValue = 1; // prevent buffs of 0
        }

        // Add modifiers
        AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject);

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} raised its {statEnumToChange.ToString()}!");

        // Update monster's UI health element
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats();
        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;
    }

    // Speed buff target
    public void SpeedBuffTarget(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Calculate buff
        float fromValue = monsterReference.speed;
        float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
        if (toValue <= 1)
        {
            toValue = 1; // prevent buffs of 0
        }

        // Add modifiers
        AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject);

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} raised its {statEnumToChange.ToString()}!");

        // Update UI, stats, and combat manager
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats();
        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;
        combatManagerScript.SortMonsterBattleSequence();
    }

    // Evasion buff target
    public void EvasionBuffTarget(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Calculate buff
        float fromValue = monsterReference.evasion;
        float toValue = Mathf.RoundToInt(amountToChange);
        if (toValue <= 1)
        {
            toValue = 1; // prevent buffs of 0
        }

        // Add modifiers
        AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject);

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} raised its {statEnumToChange.ToString()}!");

        // Update UI, stats, and combat manager
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

    // Health Cut
    public void HealthCut(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Is target's health 50% or less of maximum?
        float currentHealth = monsterAttackManager.currentTargetedMonster.health;
        float maxHealth = monsterAttackManager.currentTargetedMonster.maxHealth;

        currentHealth = Mathf.RoundToInt((amountToChange / 100f) * maxHealth);

        if (monsterAttackManager.currentTargetedMonster.health <= currentHealth)
        {
            // Send execute message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s " +
                $"health couldn't go any lower!");

            return;
        }

        // Send execute message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s " +
            $"health was lowered!");

        // reduce health
        monsterAttackManager.currentTargetedMonster.health = currentHealth;
    }

    // Damage All enemies
    /*public void DamageAllEnemies(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
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
                if (monster == combatManagerScript.CurrentTargetedMonster || monster == null)
                {
                    continue;
                }

                // First set the monster targeted then get damage calced
                monsterAttackManager.DealDamageOthers(monster);
            }
        }
    }*/

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

                    // Add modifiers
                    AddModifiers(toValue, false, monster, monsterObj);

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

                    // Add modifiers
                    AddModifiers(toValue, false, monster, monsterObj);

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

    // Lower Target Speed 
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
                    GameObject monsterObj = monsterAttackManager.currentTargetedMonsterGameObject;
                    Monster monster = monsterObj.GetComponent<CreateMonster>().monsterReference;

                    if (monster.health <= 0) // fixed dual combat log calls
                    {
                        return;

                    }

                    // Calculate speed nerf (% of current speed)
                    float fromValue = monster.speed;
                    float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
                    if (toValue <= 1)
                    {
                        toValue = 1; // prevent buffs of 0
                    }

                    // Add modifiers
                    AddModifiers(toValue, true, monster, monsterObj);

                    // Send speed buff message to combat log
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name}'s {statEnumToChange.ToString()} was lowered!");

                    // Update monster's speed element
                    monsterObj.GetComponent<CreateMonster>().UpdateStats();
                    monsterObj.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

                    // Does this actually work?
                    combatManagerScript.SortMonsterBattleSequence();
                }
                else
                if (monsterReference.aiType == Monster.AIType.Enemy)
                {
                    GameObject monsterObj = monsterAttackManager.currentTargetedMonsterGameObject;
                    Monster monster = monsterObj.GetComponent<CreateMonster>().monsterReference;

                    if (monster.health <= 0) // fixed dual combat log calls
                    {
                        return;
                    }

                    // Calculate speed nerf (% of current speed)
                    float fromValue = monster.speed;
                    float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
                    if (toValue <= 1)
                    {
                        toValue = 1; // prevent buffs of 0
                    }

                    // Add modifiers
                    AddModifiers(toValue, true, monster, monsterObj);

                    // Send speed buff message to combat log
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name}'s {statEnumToChange.ToString()} was lowered!");

                    // Update monster's speed element
                    monsterObj.GetComponent<CreateMonster>().UpdateStats();
                    monsterObj.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

                    // Does this actually work?
                    combatManagerScript.SortMonsterBattleSequence();
                }
            }
            else // the user's not the slowest, so reduce their attack stats
            {
                if (modifierCalledOnce == false)
                {
                    modifierCalledOnce = true;
                    LowerOffensiveStats(monsterReference, monsterAttackManager, monsterReferenceGameObject);
                }
            }
        }
    }

    // Counter
    public void Counter(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Check the amount of damage taken this round
        monsterAttackManager.currentMonsterAttack.monsterAttackFlatDamageBonus = Mathf.RoundToInt(monsterReferenceGameObject.GetComponent<CreateMonster>().monsterDamageTakenThisRound * (amountToChange / 100));

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} gained {monsterAttackManager.currentMonsterAttack.monsterAttackFlatDamageBonus} additional damage!");
    }

    // Bonus damage test (for Galeforce)
    public void AddBonusDamageFlat(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Get bonus damage amount source
        float bonusAmountSource = GetBonusDamageSource(statEnumToChange, monsterAttackManager);

        // calc bonus
        monsterAttackManager.currentMonsterAttack.monsterAttackFlatDamageBonus = Mathf.RoundToInt(bonusAmountSource * (amountToChange / 100));

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} gained {monsterAttackManager.currentMonsterAttack.monsterAttackFlatDamageBonus} additional damage!");
    }

    // Bonus damage test (for Galeforce)
    public void AddBonusDamage(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Get bonus damage amount source
        float bonusAmountSource = GetBonusDamageSource(statEnumToChange, monsterAttackManager);

        // calc bonus
        monsterAttackManager.recievedDamagePercentBonus = true;
        monsterAttackManager.cachedBonusDamagePercent = Mathf.RoundToInt(bonusAmountSource * (amountToChange / 100));

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} gained {monsterAttackManager.cachedBonusDamagePercent}% damage bonus!");
    }

    // Get bonus damage source
    float GetBonusDamageSource(StatEnumToChange statEnumToChange, MonsterAttackManager monsterAttackManager)
    {
        switch (statEnumToChange)
        {
            case (StatEnumToChange.Speed):
                return monsterAttackManager.currentMonsterTurn.speed;

            default:
                Debug.Log("Missing stat or monster reference?", this);
                return 0;
        }
        
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

                    // Add modifiers
                    AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject);

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

                    // Add modifiers
                    AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject);

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
        toValue *= -1;

        // Alt modifier call
        Modifier mod = CreateInstance<Modifier>();
        mod.modifierSource = name;
        mod.modifierAmount = toValue;
        mod.modifierDurationType = Modifier.ModifierDurationType.Permanent;
        mod.statModified = StatEnumToChange.PhysicalAttack;
        monsterReference.ListOfModifiers.Add(mod);
        monsterReferenceGameObject.GetComponent<CreateMonster>().ModifyStats(mod.statModified, mod);

        // Magic Attack nerf (20%)
        fromValue = monsterReference.magicAttack;
        toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
        if (toValue <= 1)
        {
            toValue = 1; // prevent buffs of 0
        }
        toValue *= -1;

        // Alt modifier call
        Modifier mod2 = CreateInstance<Modifier>();
        mod2.modifierSource = name;
        mod2.modifierAmount = toValue;
        mod2.modifierDurationType = Modifier.ModifierDurationType.Permanent;
        mod2.statModified = StatEnumToChange.MagicAttack;
        monsterReference.ListOfModifiers.Add(mod2);
        monsterReferenceGameObject.GetComponent<CreateMonster>().ModifyStats(mod2.statModified, mod2);

        // Send speed buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} lowered its Physical and Magic Attack!");

        // Update monster's stats
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats();
        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;
    }

    // Increase Offensive Stats
    public void IncreaseOffensiveStats(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Physical Attack buff
        float fromValue = monsterReference.physicalAttack;
        float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
        if (toValue <= 1)
        {
            toValue = 1; // prevent buffs of 0
        }

        // Alt modifier call
        Modifier mod = CreateInstance<Modifier>();
        mod.modifierSource = name;
        mod.modifierAmount = toValue;
        mod.modifierDurationType = Modifier.ModifierDurationType.Permanent;
        mod.statModified = StatEnumToChange.PhysicalAttack;
        monsterReference.ListOfModifiers.Add(mod);
        monsterReferenceGameObject.GetComponent<CreateMonster>().ModifyStats(mod.statModified, mod);

        // Magic Attack buff
        fromValue = monsterReference.magicAttack;
        toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
        if (toValue <= 1)
        {
            toValue = 1; // prevent buffs of 0
        }

        // Alt modifier call
        Modifier mod2 = CreateInstance<Modifier>();
        mod2.modifierSource = name;
        mod2.modifierAmount = toValue;
        mod2.modifierDurationType = Modifier.ModifierDurationType.Permanent;
        mod2.statModified = StatEnumToChange.MagicAttack;
        monsterReference.ListOfModifiers.Add(mod2);
        monsterReferenceGameObject.GetComponent<CreateMonster>().ModifyStats(mod2.statModified, mod2);

        // Send speed buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} increased its Physical and Magic Attack!");

        // Update monster's stats
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats();
        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;
    }

    // Add modifiers
    public void AddModifiers(float toValue, bool statDecrease, Monster monster, GameObject monsterObj)
    {
        // First check if not buff
        if (statDecrease)
        {
            toValue *= -1;
        }

        // Apply modifiers
        foreach (Modifier modifier in ListOfModifiers)
        {
            Modifier mod = Instantiate(modifier);
            mod.modifierSource = name;
            mod.modifierAmount = toValue;
            monster.ListOfModifiers.Add(mod);
            monsterObj.GetComponent<CreateMonster>().ModifyStats(statEnumToChange, mod);
        }
    }

    // Add modifiers override
    public void AddModifiers(float toValue, bool statDecrease, Monster monster, GameObject monsterObj, StatEnumToChange statToChange)
    {
        // First check if not buff
        if (statDecrease)
        {
            toValue *= -1;
        }

        // Apply modifiers
        foreach (Modifier modifier in ListOfModifiers)
        {
            Modifier mod = Instantiate(modifier);
            mod.modifierSource = name;
            mod.modifierAmount = toValue;
            monster.ListOfModifiers.Add(mod);
            monsterObj.GetComponent<CreateMonster>().ModifyStats(statToChange, mod);
        }
    }
}
