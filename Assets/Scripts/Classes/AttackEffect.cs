using System; // allows serialization of custom classes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
[CreateAssetMenu(fileName = "New Monster Attack Effect", menuName = "Monster Attack Effects")]
public class AttackEffect : ScriptableObject
{
    [Title("Effect Initialization Enumerators")]
    public enum TypeOfEffect
    {
        MinorDrain, MagicalAttackBuffSelf, HalfHealthExecute, SpeedBuffAllies, Counter,
        DoublePowerIfStatBoost, OnCriticalStrikeBuff, DamageAllEnemies, CripplingFearEffect, NonDamagingMove,
        SpeedBuffTarget, EvasionBuffTarget, AddBonusDamage, AddBonusDamageFlat, IncreaseOffensiveStats, HealthCut, BuffTarget, DebuffTarget, GrantImmunity,
        InflictBurning, InflictPoisoned, InflictDazed,
        DamageBonusIfTargetStatusEffect
    }

    public TypeOfEffect typeOfEffect;

    public enum StatEnumToChange { Health, Mana, PhysicalAttack, MagicAttack, PhysicalDefense, MagicDefense, Speed, Evasion, CritChance, Debuffs, StatChanges, Damage, BothOffensiveStats, CritDamage, MaxHealth, Accuracy }
    public StatEnumToChange statEnumToChange;

    public enum StatChangeType { Buff, Debuff, None }
    public StatChangeType statChangeType;

    public enum EffectTime { PreAttack, DuringAttack, PostAttack }
    public EffectTime effectTime;

    public Modifier.StatusEffectType attackEffectStatus;

    [Title("Modifier Adjustments")]
    public bool inflictSelf = false;
    public bool modifierCalledOnce = false;
    public bool flatBuff = false;

    [PropertyRange(0, 10)]
    public int modifierDuration;

    [PropertyRange(0, 400)]
    public float amountToChange;

    [PropertyRange(0, 100)]
    public float effectTriggerChance;

    [DisplayWithoutEdit] public CombatManagerScript combatManagerScript;

    public List<Modifier> ListOfModifiers;

    // Initial function that is called by monsterAttackManager that enacts attack after effects
    public void TriggerEffects(MonsterAttackManager monsterAttackManager, string effectTrigger)
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

            case TypeOfEffect.BuffTarget:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = monsterAttackManager.combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
                    targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                    BuffTargetStat(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                }
                break;

            case TypeOfEffect.DebuffTarget:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = monsterAttackManager.combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
                    targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                    DebuffTargetStat(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                }
                break;

            case TypeOfEffect.GrantImmunity:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = monsterAttackManager.combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
                    targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                    GrantTargetImmunity(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                }
                break;

            case TypeOfEffect.InflictBurning:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    if (!inflictSelf)
                    {
                        targetMonster = monsterAttackManager.combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
                        targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                        InflictBurning(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                    }
                    else
                    {
                        targetMonster = monsterAttackManager.currentMonsterTurn; // monsterattackmanager instead of combatmanager reference to fix a fueguy self daze bug after killing all enemies
                        targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject; // monsterattackmanager instead of combatmanager reference to fix a fueguy self daze bug after killing all enemies
                        InflictBurning(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                    }
                }
                break;

            case TypeOfEffect.InflictPoisoned:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    if (!inflictSelf)
                    {
                        targetMonster = monsterAttackManager.combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
                        targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                        InflictPoisoned(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                    }
                    else
                    {
                        targetMonster = monsterAttackManager.currentMonsterTurn; // monsterattackmanager instead of combatmanager reference to fix a fueguy self daze bug after killing all enemies
                        targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject; // monsterattackmanager instead of combatmanager reference to fix a fueguy self daze bug after killing all enemies
                        InflictPoisoned(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                    }
                }
                break;

            case TypeOfEffect.InflictDazed:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    if (!inflictSelf)
                    {
                        targetMonster = monsterAttackManager.combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
                        targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                        InflictDazed(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                    }
                    else
                    {
                        targetMonster = monsterAttackManager.currentMonsterTurn; // monsterattackmanager instead of combatmanager reference to fix a fueguy self daze bug after killing all enemies
                        targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject; // monsterattackmanager instead of combatmanager reference to fix a fueguy self daze bug after killing all enemies
                        InflictDazed(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                    }
                }
                break;

            case TypeOfEffect.DamageBonusIfTargetStatusEffect:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    monsterAttackManager.currentTargetedMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster; // fix fan the flames bug?
                    targetMonster = monsterAttackManager.combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
                    targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                    BonusDamageIfTargetStatusEffect(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
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
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
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

        // Check if immune to skip modifiers
        if (CheckImmunities(monsterReference, monsterAttackManager, monsterReferenceGameObject))
        {
            return;
        }

        // Add modifiers
        AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject);

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} raised its {statEnumToChange.ToString()}!");
        monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(statEnumToChange, true);

        // Update monster's UI health element
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
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

        // Check if immune to skip modifiers
        if (CheckImmunities(monsterReference, monsterAttackManager, monsterReferenceGameObject))
            return;


        // Add modifiers
        AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject);

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} raised its {statEnumToChange.ToString()}!");

        // Update UI, stats, and combat manager
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
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

        // Check if immune to skip modifiers
        if (CheckImmunities(monsterReference, monsterAttackManager, monsterReferenceGameObject))
            return;


        // Add modifiers
        AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject);

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} raised its {statEnumToChange.ToString()}!");

        // Update UI, stats, and combat manager
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
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

        // Check if immune to skip modifiers
        if (CheckImmunities(monsterReference, monsterAttackManager, monsterReferenceGameObject))
        {
            return;
        }

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
        monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatEnumToChange.Health, false);

        // reduce health
        monsterAttackManager.currentTargetedMonster.health = currentHealth;

        // Update monster's stats
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(true, null, false);
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

    // Double power of monster attack
    public void BonusDamageIfTargetStatusEffect(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, string effectTriggerName)
    {
        // Check if the monster had recieved a stat boost this round
        switch (attackEffectStatus)
        {
            case (Modifier.StatusEffectType.Burning):
                if (monsterAttackManager.combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterIsBurning)
                {
                    // apply bonus damage
                    monsterAttackManager.recievedDamagePercentBonus = true;
                    float bonusDamagePercent = (amountToChange / 100f) * 10f;
                    monsterAttackManager.cachedBonusDamagePercent = bonusDamagePercent;

                    // Send buff message to combat log
                    combatManagerScript = monsterAttackManager.combatManagerScript;
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} gained bonus damage!");
                }
                break;

            default:
                break;
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

                    // Check if immune to skip modifiers
                    if (CheckImmunities(monster, monsterAttackManager, monsterObj))
                    {
                        return;
                    }

                    // Add modifiers
                    AddModifiers(toValue, false, monster, monsterObj);

                    // Send speed buff message to combat log
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} raised its {statEnumToChange.ToString()}!");
                    monsterObj.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatEnumToChange.Speed, true);

                    // Update monster's speed element
                    monsterObj.GetComponent<CreateMonster>().UpdateStats(false, null, false);
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

                    // Check if immune to skip modifiers
                    if (CheckImmunities(monster, monsterAttackManager, monsterObj))
                    {
                        return;
                    }

                    // Add modifiers
                    AddModifiers(toValue, false, monster, monsterObj);

                    // Send speed buff message to combat log
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} raised its {statEnumToChange.ToString()}!");
                    monsterObj.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatEnumToChange.Speed, true);

                    // Update monster's speed element
                    monsterObj.GetComponent<CreateMonster>().UpdateStats(false, null, false);
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

                    // Check if immune to skip modifiers
                    if (CheckImmunities(monster, monsterAttackManager, monsterObj))
                    {
                        return;
                    }

                    // Add modifiers
                    AddModifiers(toValue, true, monster, monsterObj);

                    // Send speed buff message to combat log
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name}'s {statEnumToChange.ToString()} was lowered!");
                    monsterObj.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatEnumToChange.Speed, false);

                    // Update monster's speed element
                    monsterObj.GetComponent<CreateMonster>().UpdateStats(false, null, false);
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

                    // Check if immune to skip modifiers
                    if (CheckImmunities(monster, monsterAttackManager, monsterObj))
                    {
                        return;
                    }

                    // Add modifiers
                    AddModifiers(toValue, true, monster, monsterObj);

                    // Send speed buff message to combat log
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name}'s {statEnumToChange.ToString()} was lowered!");
                    monsterObj.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatEnumToChange.Speed, false);

                    // Update monster's speed element
                    monsterObj.GetComponent<CreateMonster>().UpdateStats(false, null, false);
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

                    // Check if immune to skip modifiers
                    if (CheckImmunities(monsterReference, monsterAttackManager, monsterReferenceGameObject))
                    {
                        return;
                    }

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
        float bonusAmountSource = GetBonusDamageSource(statEnumToChange, monsterReference);

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
        float bonusAmountSource = GetBonusDamageSource(statEnumToChange, monsterReference);

        // calc bonus
        monsterAttackManager.recievedDamagePercentBonus = true;
        monsterAttackManager.cachedBonusDamagePercent = Mathf.RoundToInt((bonusAmountSource * (amountToChange / 100)) / 2);

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} gained {monsterAttackManager.cachedBonusDamagePercent}% damage bonus!");
    }

    // Get bonus damage source
    float GetBonusDamageSource(StatEnumToChange statEnumToChange, Monster monsterRef)
    {
        switch (statEnumToChange)
        {
            case (StatEnumToChange.Speed):
                return monsterRef.speed;

            case (StatEnumToChange.PhysicalAttack):
                return monsterRef.physicalAttack;

            case (StatEnumToChange.PhysicalDefense):
                return monsterRef.physicalDefense;

            case (StatEnumToChange.MagicAttack):
                return monsterRef.magicAttack;

            case (StatEnumToChange.MagicDefense):
                return monsterRef.magicDefense;

            case (StatEnumToChange.Evasion):
                return monsterRef.evasion;

            case (StatEnumToChange.CritChance):
                return monsterRef.critChance;

            case (StatEnumToChange.CritDamage):
                return monsterRef.critDamage;

            case (StatEnumToChange.MaxHealth):
                return monsterRef.maxHealth;

            case (StatEnumToChange.Health):
                return monsterRef.health;

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
            switch (statEnumToChange)
            {
                case (StatEnumToChange.PhysicalAttack):

                    // Calculate buff
                    float fromValue = monsterReference.physicalAttack;
                    float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
                    if (toValue <= 1)
                    {
                        toValue = 1; // prevent buffs of 0
                    }

                    // Check if immune to skip modifiers
                    if (CheckImmunities(monsterReference, monsterAttackManager, monsterReferenceGameObject))
                    {
                        return;
                    }

                    // Add modifiers
                    AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject);

                    // Send buff message to combat log
                    combatManagerScript = monsterAttackManager.combatManagerScript;
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} raised its {statEnumToChange.ToString()}!");
                    monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatEnumToChange.PhysicalAttack, true);

                    // Update monster's UI health element
                    monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
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

                    // Check if immune to skip modifiers
                    if (CheckImmunities(monsterReference, monsterAttackManager, monsterReferenceGameObject))
                    {
                        return;
                    }

                    // Add modifiers
                    AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject);

                    // Send buff message to combat log
                    combatManagerScript = monsterAttackManager.combatManagerScript;
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} raised its {statEnumToChange.ToString()}!");
                    monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatEnumToChange.MagicAttack, true);

                    // Update monster's UI health element
                    monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
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
        monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatEnumToChange.PhysicalAttack, false);
        monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatEnumToChange.MagicAttack, false);

        // Update monster's stats
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
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
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;
    }

    // Delegate Buff Function Test
    public void BuffTargetStat(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, string effectTriggerName)
    {
        // Grab new refs?
        monsterReference = monsterAttackManager.currentTargetedMonster;
        monsterReferenceGameObject = monsterAttackManager.currentTargetedMonsterGameObject;

        // Calculate buff
        float fromValue = GetBonusDamageSource(statEnumToChange, monsterReference);
        float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
        if (toValue <= 1)
        {
            toValue = 1; // prevent buffs of 0
        }

        if (flatBuff)
        {
            toValue = amountToChange;
        }

        // Check if immune to skip modifiers
        if (CheckImmunities(monsterReference, monsterAttackManager, monsterReferenceGameObject))
        {
            return;
        }

        // see if effect hits
        float hitChance = (effectTriggerChance / 100);
        float randValue = UnityEngine.Random.value;

        if (randValue > hitChance)
        {
            return;
        }

        // Check if certain stat change reaches upper cap
        if (statEnumToChange == StatEnumToChange.CritDamage && fromValue >= 2.5)
        {
            // Send execute message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s " +
                $"{statEnumToChange.ToString()} couldn't go any higher!", monsterReference.aiType);
            return;
        }

        // Add modifiers
        CreateAndAddModifiers(toValue, false, monsterReference, monsterReferenceGameObject, modifierDuration, monsterAttackManager.currentMonsterTurnGameObject);

        // Send speed buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statEnumToChange.ToString()} was increased by {effectTriggerName}!", monsterReference.aiType);
        monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(statEnumToChange, true);

        // Update monster's stats
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

        // Update combat order if speed was adjusted
        if (statEnumToChange == StatEnumToChange.Speed)
        {
            combatManagerScript.SortMonsterBattleSequence();
        }
    }

    // Delegate Debuff Function Test
    public void DebuffTargetStat(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, string effectTriggerName)
    {
        // Grab new refs?
        monsterReference = monsterAttackManager.currentTargetedMonster;
        monsterReferenceGameObject = monsterAttackManager.currentTargetedMonsterGameObject;

        // Calculate debuff
        float fromValue = GetBonusDamageSource(statEnumToChange, monsterReference);
        float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
        if (toValue <= 1)
        {
            toValue = 1; // prevent buffs of 0
        }

        // Check if flat buff
        if (flatBuff)
        {
            toValue = amountToChange;
        }

        // Check if health debuff to x (toValue) percentage of max health
        if (statEnumToChange == StatEnumToChange.MaxHealth)
        {
            statEnumToChange = StatEnumToChange.Health;
        }

        // health -= .10 * maxHealth

        // Check if immune to skip modifiers
        if (CheckImmunities(monsterReference, monsterAttackManager, monsterReferenceGameObject))
        {
            return;
        }

        // see if effect hits
        float hitChance = (effectTriggerChance / 100);
        float randValue = UnityEngine.Random.value;

        if (randValue > hitChance)
        {
            return;
        }

        // Check stat change reaches lower cap
        if (fromValue <= 1)
        {
            // Send execute message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s " +
                $"{statEnumToChange.ToString()} couldn't go any lower!", monsterReference.aiType);
            return;
        }

        // Add modifiers
        CreateAndAddModifiers(toValue, true, monsterReference, monsterReferenceGameObject, modifierDuration, monsterAttackManager.currentMonsterTurnGameObject);

        // Send speed buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statEnumToChange.ToString()} was decreased by {effectTriggerName}!", monsterReference.aiType);
        monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(statEnumToChange, false);

        // Update monster's stats
        if (statEnumToChange == StatEnumToChange.Health)
        {
            monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(true, null, false); // if health changed, check health
        }
        else
        {
            monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
        }

        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

        // Update combat order if speed was adjusted
        if (statEnumToChange == StatEnumToChange.Speed)
        {
            combatManagerScript.SortMonsterBattleSequence();
        }
    }

    // Delegate Debuff Function Test
    public void InflictBurning(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, string effectTriggerName)
    {
        // Grab new refs?
        monsterReference = monsterAttackManager.currentTargetedMonster;
        monsterReferenceGameObject = monsterAttackManager.currentTargetedMonsterGameObject;

        // Check if immune to skip modifiers
        if (CheckImmunities(monsterReference, monsterAttackManager, monsterReferenceGameObject))
        {
            return;
        }

        // Check if already burned
        if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsBurning == true)
        {
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is already Burning!", monsterReference.aiType);
            return;
        }

        // see if effect hits
        float hitChance = (effectTriggerChance / 100);
        float randValue = UnityEngine.Random.value;

        if (randValue > hitChance)
        {
            return;
        }

        // Get damage amount
        float toValue = (amountToChange / 100);

        // Add modifiers
        //AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject, modifierDuration);
        CreateAndAddModifiers(toValue, false, monsterReference, monsterReferenceGameObject, modifierDuration, Modifier.StatusEffectType.Burning, monsterAttackManager.currentMonsterTurnGameObject);

        // Send message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} was burned by {effectTriggerName}!");

        // Update monster's stats
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
        monsterReferenceGameObject.GetComponent<CreateMonster>().InflictStatus(Modifier.StatusEffectType.Burning);

        // Update combat order if speed was adjusted
        if (statEnumToChange == StatEnumToChange.Speed)
        {
            combatManagerScript.SortMonsterBattleSequence();
        }
    }

    // Delegate Debuff Function Test
    public void InflictPoisoned(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, string effectTriggerName)
    {
        // Grab new refs?
        monsterReference = monsterAttackManager.currentTargetedMonster;
        monsterReferenceGameObject = monsterAttackManager.currentTargetedMonsterGameObject;

        // Check if immune to skip modifiers
        if (CheckImmunities(monsterReference, monsterAttackManager, monsterReferenceGameObject))
        {
            return;
        }

        // Check if already poisened
        if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsPoisoned == true)
        {
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is already Poisoned!", monsterReference.aiType);
            return;
        }

        // see if effect hits
        float hitChance = (effectTriggerChance / 100);
        float randValue = UnityEngine.Random.value;

        if (randValue > hitChance)
        {
            return;
        }

        // Get damage amount
        float toValue = (amountToChange / 100);

        // Add modifiers
        //AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject, modifierDuration);
        CreateAndAddModifiers(toValue, false, monsterReference, monsterReferenceGameObject, modifierDuration, Modifier.StatusEffectType.Poisoned, monsterAttackManager.currentMonsterTurnGameObject);

        // Send message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} was poisoned by {effectTriggerName}!");

        // Update monster's stats
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
        monsterReferenceGameObject.GetComponent<CreateMonster>().InflictStatus(Modifier.StatusEffectType.Poisoned);

        // Update combat order if speed was adjusted
        if (statEnumToChange == StatEnumToChange.Speed)
        {
            combatManagerScript.SortMonsterBattleSequence();
        }
    }

    // Delegate Debuff Function Test
    public void InflictDazed(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, string effectTriggerName)
    {
        // Grab new refs?
        if (inflictSelf)
        {
            monsterReference = monsterAttackManager.currentMonsterTurn;
            monsterReferenceGameObject = monsterAttackManager.currentMonsterTurnGameObject;
        }
        else
        {
            monsterReference = monsterAttackManager.currentTargetedMonster;
            monsterReferenceGameObject = monsterAttackManager.currentTargetedMonsterGameObject;
        }

        // Check if immune to skip modifiers
        if (CheckImmunities(monsterReference, monsterAttackManager, monsterReferenceGameObject))
        {
            return;
        }

        // Check if already dazed
        if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsDazed == true)
        {
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is already Dazed!", monsterReference.aiType);
            return;
        }

        // see if effect hits
        float hitChance = (effectTriggerChance / 100);
        float randValue = UnityEngine.Random.value;

        if (randValue > hitChance)
        {
            return;
        }

        // Get damage amount
        float toValue = (amountToChange / 100);

        // Add modifiers
        //AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject, modifierDuration);
        CreateAndAddModifiers(toValue, false, monsterReference, monsterReferenceGameObject, modifierDuration, Modifier.StatusEffectType.Dazed, monsterAttackManager.currentMonsterTurnGameObject);

        // Send message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} was Dazed by {effectTriggerName}!");

        // Update monster's stats
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
        monsterReferenceGameObject.GetComponent<CreateMonster>().InflictStatus(Modifier.StatusEffectType.Dazed);

        // Update combat order if speed was adjusted
        if (statEnumToChange == StatEnumToChange.Speed)
        {
            combatManagerScript.SortMonsterBattleSequence();
        }
    }

    // Delegate Buff Function Test
    public void GrantTargetImmunity(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, string effectTriggerName)
    {
        // Check if immune to skip modifiers
        if (CheckImmunities(monsterReference, monsterAttackManager, monsterReferenceGameObject))
        {
            return;
        }

        float toValue = 0;

        // First check if already immune
        switch (statEnumToChange)
        {
            case (StatEnumToChange.Debuffs):
                if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToDebuffs)
                {
                    // Send message to combat log
                    combatManagerScript = monsterAttackManager.combatManagerScript;
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is already immune to status effects and debuffs!");
                    return;
                }
                break;

            case (StatEnumToChange.StatChanges):
                if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToStatChanges)
                {
                    // Send message to combat log
                    combatManagerScript = monsterAttackManager.combatManagerScript;
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is already immune to stat changes!");
                    return;
                }
                break;

            case (StatEnumToChange.Damage):
                if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToDamage)
                {
                    // Send message to combat log
                    combatManagerScript = monsterAttackManager.combatManagerScript;
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is already immune to damage!");
                    return;
                }
                break;

            default:
                break;
        }

        // Create Modifier
        CreateModifier(monsterReference, monsterAttackManager, monsterReferenceGameObject, 0, modifierDuration, false);

        // Add modifiers
        AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject, modifierDuration);

        // Send message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} gained immunity to status effects and debuffs!");
        //monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Debuff and Status Immunity!");

        // Update monster's stats
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

        // Update combat order if speed was adjusted
        if (statEnumToChange == StatEnumToChange.Speed)
        {
            combatManagerScript.SortMonsterBattleSequence();
        }
    }

    // Check Immunities
    public bool CheckImmunities(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToDebuffs || monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToStatChanges)
        {
            if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToDebuffs && statChangeType == StatChangeType.Debuff)
            {
                // Send immune message to combat log
                combatManagerScript = monsterAttackManager.combatManagerScript;
                combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is immune to status effects and debuffs!");
                monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Immune!");
                return true;
            }
            else if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToStatChanges && statChangeType != StatChangeType.None)
            {
                // Send immune message to combat log
                combatManagerScript = monsterAttackManager.combatManagerScript;
                combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is immune to stat changes! Its {statEnumToChange} cannot be effected!");
                monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Immune!");
                return true;
            }
        }

        return false;
    }

    // Delegate Create Modifier test
    public void CreateModifier(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, float toValue, int modifierDuration, bool statusEffect)
    {
        Modifier mod = CreateInstance<Modifier>();
        mod.modifierSource = name;
        mod.modifierAmount = toValue;
        mod.modifierDuration = modifierDuration;
        mod.modifierCurrentDuration = modifierDuration;
        if (modifierDuration > 0)
        {
            mod.modifierDurationType = Modifier.ModifierDurationType.Temporary;
        }
        mod.statModified = statEnumToChange;
        mod.statusEffect = statusEffect;
        monsterReference.ListOfModifiers.Add(mod);
        monsterReferenceGameObject.GetComponent<CreateMonster>().ModifyStats(mod.statModified, mod);
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

    // Add modifiers duration override
    public void AddModifiers(float toValue, bool statDecrease, Monster monster, GameObject monsterObj, int duration)
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
            mod.modifierDuration = duration;
            mod.modifierCurrentDuration = duration;
            if (duration > 0)
            {
                mod.modifierDurationType = Modifier.ModifierDurationType.Temporary;
            }
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

    // Create and Add modifiers
    public void CreateAndAddModifiers(float toValue, bool statDecrease, Monster monster, GameObject monsterObj, int duration, GameObject monsterOwnerGameObject)
    {
        // First check if not buff
        if (statDecrease)
        {
            toValue *= -1;
        }

        // Create and Apply modifier
        Modifier mod = CreateInstance<Modifier>();
        mod.modifierSource = name;
        mod.statModified = statEnumToChange;
        mod.modifierAmount = toValue;
        mod.modifierDuration = duration;
        mod.modifierCurrentDuration = duration;
        mod.modifierOwnerGameObject = monsterOwnerGameObject;
        mod.modifierOwner = monsterOwnerGameObject.GetComponent<CreateMonster>().monsterReference;
        if (duration > 0)
        {
            mod.modifierDurationType = Modifier.ModifierDurationType.Temporary;
        }
        else
        {
            mod.modifierDurationType = Modifier.ModifierDurationType.Permanent;
        }
        monster.ListOfModifiers.Add(mod);
        monsterObj.GetComponent<CreateMonster>().ModifyStats(statEnumToChange, mod);
    }

    // Create and Add modifiers - New Stat
    public void CreateAndAddModifiers(float toValue, bool statDecrease, Monster monster, GameObject monsterObj, int duration, GameObject monsterOwnerGameObject, StatEnumToChange statEnumToChange)
    {
        // First check if not buff
        if (statDecrease)
        {
            toValue *= -1;
        }

        // Create and Apply modifier
        Modifier mod = CreateInstance<Modifier>();
        mod.modifierSource = name;
        mod.statModified = statEnumToChange;
        mod.modifierAmount = toValue;
        mod.modifierDuration = duration;
        mod.modifierCurrentDuration = duration;
        mod.modifierOwnerGameObject = monsterOwnerGameObject;
        mod.modifierOwner = monsterOwnerGameObject.GetComponent<CreateMonster>().monsterReference;
        if (duration > 0)
        {
            mod.modifierDurationType = Modifier.ModifierDurationType.Temporary;
        }
        else
        {
            mod.modifierDurationType = Modifier.ModifierDurationType.Permanent;
        }
        monster.ListOfModifiers.Add(mod);
        monsterObj.GetComponent<CreateMonster>().ModifyStats(statEnumToChange, mod);
    }

    // Create and Add modifiers - Status Effect
    public void CreateAndAddModifiers(float toValue, bool statDecrease, Monster monster, GameObject monsterObj, int duration, Modifier.StatusEffectType statusEffect, GameObject monsterOwnerGameObject)
    {
        // First check if not buff
        if (statDecrease)
        {
            toValue *= -1;
        }

        // Create and Apply modifier
        Modifier mod = CreateInstance<Modifier>();
        mod.modifierSource = name;
        mod.statModified = statEnumToChange;
        mod.modifierAmount = toValue;
        mod.modifierDuration = duration;
        mod.modifierCurrentDuration = duration;
        mod.modifierOwnerGameObject = monsterOwnerGameObject;
        mod.modifierOwner = monsterOwnerGameObject.GetComponent<CreateMonster>().monsterReference;

        // status effect
        mod.statusEffect = true;
        mod.statusEffectType = statusEffect;

        if (duration > 0)
        {
            mod.modifierDurationType = Modifier.ModifierDurationType.Temporary;
        }
        else
        {
            mod.modifierDurationType = Modifier.ModifierDurationType.Permanent;
        }
        monster.ListOfModifiers.Add(mod);
        monsterObj.GetComponent<CreateMonster>().ModifyStats(statEnumToChange, mod);
    }
}
