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
        DamageBonusIfTargetStatusEffect, InflictStunned, AffectTargetByAnotherStat, InflictStatusEffect, AffectTargetStat
    }

    public TypeOfEffect typeOfEffect;

    public enum StatChangeType { Buff, Debuff, None }
    public StatChangeType statChangeType;

    public Modifier.StatusEffectType attackEffectStatus;

    [EnableIf("typeOfEffect", TypeOfEffect.AffectTargetStat)]
    public enum StatToChange { Health, Mana, PhysicalAttack, MagicAttack, PhysicalDefense, MagicDefense, Speed, Evasion, CritChance, Debuffs, StatChanges, Damage, BothOffensiveStats, CritDamage, MaxHealth, Accuracy, Various, AddOffensiveAttackEffect, HighestAttackStat, Buffs }
    public StatToChange statToChange;

    public enum EffectTime { PreAttack, DuringAttack, PostAttack, OnKill, OnDeath, GameStart, RoundStart, RoundEnd, OnStatChange, OnDamageTaken, PreOtherAttack, OnDamageDealt }
    public EffectTime effectTime;

    public enum MonsterTargetType { Target, Self }
    public MonsterTargetType monsterTargetType;

    public GameObject currentMonsterTargetGameObject;
    public Monster currentMonsterTarget;

    [EnableIf("typeOfEffect", TypeOfEffect.AffectTargetByAnotherStat)]
    public StatToChange scaleFromWhatStat;
    public enum ScaleFromWhatTarget { monsterUsingAttack, targetMonster }

    [EnableIf("typeOfEffect", TypeOfEffect.AffectTargetByAnotherStat)]
    public ScaleFromWhatTarget scaleFromWhatTarget;

    public enum AttackEffectDuration { Permanent, Temporary }
    public AttackEffectDuration attackEffectDuration;

    [Title("Modifier Adjustments")]
    public bool inflictSelf = false;
    public bool modifierCalledOnce = false;
    public bool flatValueChange = false;

    [PropertyRange(0, 10)]
    public int modifierDuration;

    [PropertyRange(0, 400)]
    public float amountToChange;

    [PropertyRange(0, 100)]
    public float effectTriggerChance;

    [DisplayWithoutEdit] public CombatManagerScript combatManagerScript;

    public AttackEffect(StatToChange statEnumToChange, StatChangeType statChangeType, EffectTime effectTime, 
        Modifier.StatusEffectType attackEffectStatus, bool inflictSelf, bool modifierCalledOnce, bool flatValueChange, int modifierDuration, float amountToChange, 
        float effectTriggerChance, CombatManagerScript combatManagerScript)
    {
        this.statToChange = statEnumToChange;
        this.statChangeType = statChangeType;
        this.effectTime = effectTime;
        this.attackEffectStatus = attackEffectStatus;
        this.inflictSelf = inflictSelf;
        this.modifierCalledOnce = modifierCalledOnce;
        this.flatValueChange = flatValueChange;
        this.modifierDuration = modifierDuration;
        this.amountToChange = amountToChange;
        this.effectTriggerChance = effectTriggerChance;
        this.combatManagerScript = combatManagerScript;
    }

    // Initial function that is called by monsterAttackManager that enacts attack after effects
    public void TriggerEffects(MonsterAttackManager monsterAttackManager, string effectTrigger, MonsterAttack attackTrigger)
    {
        switch (typeOfEffect)
        {
            case TypeOfEffect.MinorDrain:
                Monster targetMonster = monsterAttackManager.currentMonsterTurn; // should still be the monster who used the move, NOT the one next in Queue
                GameObject targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                //MinorDrain(targetMonster, monsterAttackManager, targetMonsterGameObject, monsterAttackManager.cachedDamage);
                break;

            case TypeOfEffect.MagicalAttackBuffSelf:
                targetMonster = monsterAttackManager.currentMonsterTurn; // should still be the monster who used the move, NOT the one next in Queue
                targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                //MagicalAttackBuffSelf(targetMonster, monsterAttackManager, targetMonsterGameObject);
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
                    //SpeedBuffTarget(targetMonster, monsterAttackManager, targetMonsterGameObject);
                }
                break;

            case TypeOfEffect.EvasionBuffTarget:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = monsterAttackManager.combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
                    targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                    //EvasionBuffTarget(targetMonster, monsterAttackManager, targetMonsterGameObject);
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
                    //HealthCut(targetMonster, monsterAttackManager, targetMonsterGameObject);
                }
                break;

            case TypeOfEffect.BuffTarget:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = monsterAttackManager.currentTargetedMonster;
                    targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                    BuffTargetStat(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                }
                break;

            case TypeOfEffect.DebuffTarget:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = monsterAttackManager.currentTargetedMonster;
                    targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                    DebuffTargetStat(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                }
                break;

            case TypeOfEffect.GrantImmunity:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    targetMonster = monsterAttackManager.currentTargetedMonster;
                    targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                    GrantTargetImmunity(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                }
                break;

            //case TypeOfEffect.InflictBurning:
            //    if (monsterAttackManager.currentMonsterTurn != null)
            //    {
            //        if (!inflictSelf)
            //        {
            //            targetMonster = monsterAttackManager.currentTargetedMonster;
            //            targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
            //            InflictBurning(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
            //        }
            //        else
            //        {
            //            targetMonster = monsterAttackManager.currentMonsterTurn; // monsterattackmanager instead of combatmanager reference to fix a fueguy self daze bug after killing all enemies
            //            targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject; // monsterattackmanager instead of combatmanager reference to fix a fueguy self daze bug after killing all enemies
            //            InflictBurning(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
            //        }
            //    }
            //    break;

            //case TypeOfEffect.InflictPoisoned:
            //    if (monsterAttackManager.currentMonsterTurn != null)
            //    {
            //        if (!inflictSelf)
            //        {
            //            targetMonster = monsterAttackManager.currentTargetedMonster;
            //            targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
            //            InflictPoisoned(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
            //        }
            //        else
            //        {
            //            targetMonster = monsterAttackManager.currentMonsterTurn; // monsterattackmanager instead of combatmanager reference to fix a fueguy self daze bug after killing all enemies
            //            targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject; // monsterattackmanager instead of combatmanager reference to fix a fueguy self daze bug after killing all enemies
            //            InflictPoisoned(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
            //        }
            //    }
            //    break;

            //case TypeOfEffect.InflictDazed:
            //    if (monsterAttackManager.currentMonsterTurn != null)
            //    {
            //        if (!inflictSelf)
            //        {
            //            targetMonster = monsterAttackManager.currentTargetedMonster;
            //            targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
            //            InflictDazed(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
            //        }
            //        else
            //        {
            //            targetMonster = monsterAttackManager.currentMonsterTurn; // monsterattackmanager instead of combatmanager reference to fix a fueguy self daze bug after killing all enemies
            //            targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject; // monsterattackmanager instead of combatmanager reference to fix a fueguy self daze bug after killing all enemies
            //            InflictDazed(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
            //        }
            //    }
            //    break;

            //case TypeOfEffect.InflictStunned:
            //    if (monsterAttackManager.currentMonsterTurn != null)
            //    {
            //        if (!inflictSelf)
            //        {
            //            targetMonster = monsterAttackManager.currentTargetedMonster;
            //            targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
            //            InflictStunned(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
            //        }
            //        else
            //        {
            //            targetMonster = monsterAttackManager.currentMonsterTurn; // monsterattackmanager instead of combatmanager reference to fix a fueguy self daze bug after killing all enemies
            //            targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject; // monsterattackmanager instead of combatmanager reference to fix a fueguy self daze bug after killing all enemies
            //            InflictStunned(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
            //        }
            //    }
            //    break;

            case TypeOfEffect.InflictStatusEffect:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                        targetMonster = monsterAttackManager.currentTargetedMonster;
                        targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                        InflictStatusEffect(targetMonster, targetMonsterGameObject, monsterAttackManager, effectTrigger);
                }
                break;

            case TypeOfEffect.DamageBonusIfTargetStatusEffect:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    monsterAttackManager.currentTargetedMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster; // fix fan the flames bug?
                    targetMonster = monsterAttackManager.currentMonsterTurn; // fixed fan the flames bug incorrect monster getting bonus
                    targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
                    BonusDamageIfTargetStatusEffect(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                }
                break;

            case TypeOfEffect.AffectTargetByAnotherStat:
                if (monsterAttackManager.currentMonsterTurn != null)
                {
                    if (monsterTargetType == MonsterTargetType.Target)
                    {
                        targetMonster = monsterAttackManager.currentMonsterTurn;
                        targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentMonsterTurn;
                        _AffectTargetStatByAnotherStat(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                    }
                    else
                    {
                        targetMonster = monsterAttackManager.currentTargetedMonster;
                        targetMonsterGameObject = monsterAttackManager.combatManagerScript.CurrentTargetedMonster;
                        _AffectTargetStatByAnotherStat(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                    }
                }
                break;

            default:
                Debug.Log("Missing effect or attack reference?", this);
                break;
        }
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

    // Double power of monster attack
    public void DoublePowerIfStatBoost(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Check if the monster had recieved a stat boost this round
        if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound)
        {
            monsterAttackManager.currentMonsterAttack.monsterAttackDamage *= 2f;

            // Send buff message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Gained Bonus Damage!");
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} had it's power doubled!");
        }
    }

    // Double power of monster attack
    public void BonusDamageIfTargetStatusEffect(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, string effectTriggerName)
    {
        CreateMonster monsterComponent = monsterAttackManager.combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>();

        if (monsterComponent == null)
            return;

        if (monsterComponent.listofCurrentStatusEffects.Contains(attackEffectStatus))
        {
            // apply bonus damage
            monsterAttackManager.recievedDamagePercentBonus = true;
            float bonusDamagePercent = (amountToChange / 100f) * 10f;
            monsterAttackManager.cachedBonusDamagePercent = bonusDamagePercent;

            // Send buff message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Gained Bonus Damage!");
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} gained bonus damage!");
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
                    if (CheckTargetIsImmune(monster, monsterAttackManager, monsterObj, this))
                    {
                        return;
                    }

                    // Add modifiers
                    //AddModifiers(toValue, false, monster, monsterObj);

                    // Send speed buff message to combat log
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} raised its {statToChange.ToString()}!");
                    monsterObj.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatToChange.Speed, true);

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
                    if (CheckTargetIsImmune(monster, monsterAttackManager, monsterObj, this))
                    {
                        return;
                    }

                    // Add modifiers
                    //AddModifiers(toValue, false, monster, monsterObj);

                    // Send speed buff message to combat log
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} raised its {statToChange.ToString()}!");
                    monsterObj.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatToChange.Speed, true);

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
                    if (CheckTargetIsImmune(monster, monsterAttackManager, monsterObj, this))
                    {
                        return;
                    }

                    // Add modifiers
                    //AddModifiers(toValue, true, monster, monsterObj);

                    // Send speed buff message to combat log
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name}'s {statToChange.ToString()} was lowered!");
                    monsterObj.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatToChange.Speed, false);

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
                    if (CheckTargetIsImmune(monster, monsterAttackManager, monsterObj, this))
                    {

                        return;
                    }

                    // Add modifiers
                    //AddModifiers(toValue, true, monster, monsterObj);

                    // Send speed buff message to combat log
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name}'s {statToChange.ToString()} was lowered!");
                    monsterObj.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatToChange.Speed, false);

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
                    if (CheckTargetIsImmune(monsterReference, monsterAttackManager, monsterReferenceGameObject, this))
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
        float bonusAmountSource = GetBonusDamageSource(statToChange, monsterReference);

        // calc bonus
        monsterAttackManager.currentMonsterAttack.monsterAttackFlatDamageBonus = Mathf.RoundToInt(bonusAmountSource * (amountToChange / 100));

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Gained Bonus Damage!");
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} gained {monsterAttackManager.currentMonsterAttack.monsterAttackFlatDamageBonus} additional damage!");
    }

    // Bonus damage test (for Galeforce)
    public void AddBonusDamage(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Get bonus damage amount source
        float bonusAmountSource = GetBonusDamageSource(statToChange, monsterReference);

        // calc bonus
        monsterAttackManager.recievedDamagePercentBonus = true;
        monsterAttackManager.cachedBonusDamagePercent = Mathf.RoundToInt((bonusAmountSource * (amountToChange / 100)) / 2);

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} gained {monsterAttackManager.cachedBonusDamagePercent}% damage bonus!");
    }

    // Get bonus damage source
    float GetBonusDamageSource(StatToChange statEnumToChange, Monster monsterRef)
    {
        switch (statEnumToChange)
        {
            case (StatToChange.Speed):
                return monsterRef.speed;

            case (StatToChange.PhysicalAttack):
                return monsterRef.physicalAttack;

            case (StatToChange.PhysicalDefense):
                return monsterRef.physicalDefense;

            case (StatToChange.MagicAttack):
                return monsterRef.magicAttack;

            case (StatToChange.MagicDefense):
                return monsterRef.magicDefense;

            case (StatToChange.Evasion):
                return monsterRef.evasion;

            case (StatToChange.CritChance):
                return monsterRef.critChance;

            case (StatToChange.CritDamage):
                return monsterRef.critDamage;

            case (StatToChange.MaxHealth):
                return monsterRef.maxHealth;

            case (StatToChange.Health):
                return monsterRef.maxHealth;

            case (StatToChange.Accuracy):
                return monsterRef.bonusAccuracy;

            case (StatToChange.HighestAttackStat):
                if (MonsterAttackManager.ReturnMonsterHighestAttackStat(monsterRef) == MonsterAttack.MonsterAttackDamageType.Magical)
                {
                    statToChange = StatToChange.MagicAttack;
                    return monsterRef.magicAttack;
                }
                else
                {
                    statToChange = StatToChange.PhysicalAttack;
                    return monsterRef.physicalAttack;
                }

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
            switch (statToChange)
            {
                case (StatToChange.PhysicalAttack):

                    // Calculate buff
                    float fromValue = monsterReference.physicalAttack;
                    float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
                    if (toValue <= 1)
                    {
                        toValue = 1; // prevent buffs of 0
                    }

                    // Check if immune to skip modifiers
                    if (CheckTargetIsImmune(monsterReference, monsterAttackManager, monsterReferenceGameObject, this))
                    {
                        return;
                    }

                    // Add modifiers
                    //AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject);

                    // Send buff message to combat log
                    combatManagerScript = monsterAttackManager.combatManagerScript;
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} raised its {statToChange.ToString()}!");
                    monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatToChange.PhysicalAttack, true);

                    // Update monster's UI health element
                    monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
                    monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

                    break;

                case (StatToChange.MagicAttack):

                    // Calculate buff
                    fromValue = monsterReference.physicalAttack;
                    toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
                    if (toValue <= 1)
                    {
                        toValue = 1; // prevent buffs of 0
                    }

                    // Check if immune to skip modifiers
                    if (CheckTargetIsImmune(monsterReference, monsterAttackManager, monsterReferenceGameObject, this))
                    {
                        return;
                    }

                    // Add modifiers
                    //AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject);

                    // Send buff message to combat log
                    combatManagerScript = monsterAttackManager.combatManagerScript;
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} raised its {statToChange.ToString()}!");
                    monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatToChange.MagicAttack, true);

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
        // Send message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;

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
        mod.statChangeType = StatChangeType.Debuff;
        mod.modifierDurationType = Modifier.ModifierDurationType.Permanent;
        mod.statModified = StatToChange.PhysicalAttack;

        // Check stat change reaches lower cap
        if (fromValue <= 1)
        {
            // Send execute message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s " +
                $"{mod.statModified.ToString()} couldn't go any lower!", monsterReference.aiType);
            monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup($"No Effect on {mod.statModified.ToString()}!");
        }
        else
        {
            monsterReference.ListOfModifiers.Add(mod);
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {mod.statModified.ToString()} was decreased by {mod.modifierName}!", monsterReference.aiType);
            monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(mod.statModified, false);
            monsterReferenceGameObject.GetComponent<CreateMonster>().ModifyStats(mod.statModified, mod);
        };

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
        mod2.statChangeType = StatChangeType.Debuff;
        mod2.modifierDurationType = Modifier.ModifierDurationType.Permanent;
        mod2.statModified = StatToChange.MagicAttack;

        // Check stat change reaches lower cap
        if (fromValue <= 1)
        {
            // Send execute message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s " +
                $"{mod2.statModified.ToString()} couldn't go any lower!", monsterReference.aiType);
            monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup($"No Effect on {mod2.statModified.ToString()}!");
        }
        else
        {
            monsterReference.ListOfModifiers.Add(mod2);
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {mod2.statModified.ToString()} was decreased by {mod2.modifierName}!", monsterReference.aiType);
            monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(mod2.statModified, false);
            monsterReferenceGameObject.GetComponent<CreateMonster>().ModifyStats(mod2.statModified, mod2);
        }

        // Update monster's stats
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;
    }

    // Increase Offensive Stats
    public void IncreaseOffensiveStats(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Send message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;

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
        mod.statModified = StatToChange.PhysicalAttack;

        // Check stat change reaches lower cap
        if (fromValue <= 1)
        {
            // Send execute message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s " +
                $"{mod.statModified.ToString()} couldn't go any lower!", monsterReference.aiType);
            monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup($"No Effect on {mod.statModified.ToString()}!");
        }
        else
        {
            monsterReference.ListOfModifiers.Add(mod);
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {mod.statModified.ToString()} was increased by {mod.modifierName}!", monsterReference.aiType);
            monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(mod.statModified, true);
            monsterReferenceGameObject.GetComponent<CreateMonster>().ModifyStats(mod.statModified, mod);
        }

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
        mod2.statModified = StatToChange.MagicAttack;

        // Check stat change reaches lower cap
        if (fromValue <= 1)
        {
            // Send execute message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s " +
                $"{mod2.statModified.ToString()} couldn't go any lower!", monsterReference.aiType);
            monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup($"No Effect on {mod2.statModified.ToString()}!");
        }
        else
        {
            monsterReference.ListOfModifiers.Add(mod2);
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {mod2.statModified.ToString()} was increased by {mod2.modifierName}!", monsterReference.aiType);
            monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(mod2.statModified, true);
            monsterReferenceGameObject.GetComponent<CreateMonster>().ModifyStats(mod2.statModified, mod2);
        }

        // Update monster's stats
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;
    }

    public void BuffTargetStat(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, string effectTriggerName)
    {
        // Grab new refs?
        //monsterReference = monsterAttackManager.currentTargetedMonster;
        //monsterReferenceGameObject = monsterAttackManager.currentTargetedMonsterGameObject;

        // If the effect applies to user
        if (inflictSelf)
        {
            monsterReference = monsterAttackManager.currentMonsterTurn;
            monsterReferenceGameObject = monsterAttackManager.currentMonsterTurnGameObject;
        }

        CreateMonster monsterComponent = monsterReferenceGameObject.GetComponent<CreateMonster>();

        // Calculate buff
        float fromValue = GetBonusDamageSource(statToChange, monsterReference);
        float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
        if (toValue <= 1)
        {
            toValue = 1; // prevent buffs of 0
        }

        if (flatValueChange)
        {
            toValue = amountToChange;
        }

        // Check if immune to skip modifiers
        if (CheckTargetIsImmune(monsterReference, monsterAttackManager, monsterReferenceGameObject, this))
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
        if (statToChange == StatToChange.CritDamage && fromValue >= 2.5)
        {
            // Send execute message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s " +
                $"{statToChange.ToString()} couldn't go any higher!", monsterReference.aiType);
            monsterComponent.CreateStatusEffectPopup($"No Effect on {statToChange.ToString()}!");
            return;
        }

        // Check if certain stat change reaches upper cap
        if (statToChange == StatToChange.Health && monsterReference.health == monsterReference.maxHealth)
        {
            // Send execute message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s " +
                $"{statToChange.ToString()} couldn't go any higher!", monsterReference.aiType);
            monsterComponent.CreateStatusEffectPopup($"No Effect on {statToChange.ToString()}!");
            return;
        }

        // Add modifiers
        CreateAndAddModifiers(toValue, false, monsterReference, monsterReferenceGameObject, modifierDuration, monsterAttackManager.currentMonsterTurnGameObject);

        // Send speed buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        //combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statEnumToChange.ToString()} was increased by {effectTriggerName}!", monsterReference.aiType);
        //monsterComponent.CreateStatusEffectPopup(statEnumToChange, true, toValue);

        // Update monster's stats
        if (statToChange == StatToChange.Health)
        {
            monsterComponent.UpdateStats(true, null, false); // if health changed, check health
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statToChange.ToString()} was increased by {effectTriggerName} (+{toValue})!", monsterReference.aiType);
            monsterComponent.ShowDamageOrStatusEffectPopup(toValue, "Heal");
        }
        else
        {
            monsterComponent.UpdateStats(false, null, false);
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statToChange.ToString()} was increased by {effectTriggerName}!", monsterReference.aiType);
            monsterComponent.CreateStatusEffectPopup(statToChange, true);
        }

        monsterComponent.monsterRecievedStatBoostThisRound = true;

        // Update combat order if speed was adjusted
        if (statToChange == StatToChange.Speed)
        {
            combatManagerScript.SortMonsterBattleSequence();
        }

        // Trigger buff animation
        monsterComponent.GetComponent<Animator>().SetBool("buffAnimationPlaying", true);
        monsterAttackManager.soundEffectManager.PlaySoundEffect(monsterAttackManager.buffSound);
    }

    public void DebuffTargetStat(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, string effectTriggerName)
    {
        // Grab new refs?
        monsterReference = monsterAttackManager.currentTargetedMonster;
        monsterReferenceGameObject = monsterAttackManager.currentTargetedMonsterGameObject;

        // If the effect applies to user
        if (inflictSelf)
        {
            monsterReference = monsterAttackManager.currentMonsterTurn;
            monsterReferenceGameObject = monsterAttackManager.currentMonsterTurnGameObject;
        }

        CreateMonster monsterComponent = monsterReferenceGameObject.GetComponent<CreateMonster>();

        // Calculate debuff
        float fromValue = GetBonusDamageSource(statToChange, monsterReference);
        float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);
        if (toValue <= 1)
        {
            toValue = 1; // prevent buffs of 0
        }

        // Check if flat buff
        if (flatValueChange)
        {
            toValue = amountToChange;
        }

        // Check if health debuff to x (toValue) percentage of max health
        if (statToChange == StatToChange.MaxHealth)
        {
            statToChange = StatToChange.Health;
        }

        // health -= .10 * maxHealth

        // Check if immune to skip modifiers
        if (CheckTargetIsImmune(monsterReference, monsterAttackManager, monsterReferenceGameObject, this))
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
                $"{statToChange.ToString()} couldn't go any lower!", monsterReference.aiType);
            monsterComponent.CreateStatusEffectPopup($"No Effect on {statToChange.ToString()}!");
            return;
        }

        // Add modifiers
        CreateAndAddModifiers(toValue, true, monsterReference, monsterReferenceGameObject, modifierDuration, monsterAttackManager.currentMonsterTurnGameObject);

        // Send speed buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        //combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statEnumToChange.ToString()} was decreased by {effectTriggerName}!", monsterReference.aiType);
        //monsterComponent.CreateStatusEffectPopup(statEnumToChange, false);

        // Update monster's stats
        if (statToChange == StatToChange.Health)
        {
            monsterComponent.UpdateStats(true, null, false); // if health changed, check health
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statToChange.ToString()} was decreased by {effectTriggerName} (-{toValue})!", monsterReference.aiType);
            monsterComponent.ShowDamageOrStatusEffectPopup(toValue, "Damage");
        }
        else
        {
            monsterComponent.UpdateStats(false, null, false);
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statToChange.ToString()} was decreased by {effectTriggerName}!", monsterReference.aiType);
            monsterComponent.CreateStatusEffectPopup(statToChange, false);
        }

        monsterComponent.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

        // Update combat order if speed was adjusted
        if (statToChange == StatToChange.Speed)
        {
            combatManagerScript.SortMonsterBattleSequence();
        }

        // Trigger debuff animation
        monsterComponent.GetComponent<Animator>().SetBool("debuffAnimationPlaying", true);
        monsterAttackManager.soundEffectManager.PlaySoundEffect(monsterAttackManager.debuffSound);
    }

    public void AffectTargetStat(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, string effectTriggerName)
    {
        if (!CheckAttackEffectTargeting(targetMonster, targetMonsterGameObject, monsterAttackManager))
            return;

        CreateMonster monsterComponent = targetMonsterGameObject.GetComponent<CreateMonster>();

        float statChangeAmount = CalculateStatChange(targetMonster, this);

        // Check upper and lower boundaries

        // Apply stat change
    }

    public void InflictStatusEffect(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, string effectTriggerName)
    {
        if (!CheckAttackEffectTargeting(targetMonster, targetMonsterGameObject, monsterAttackManager))
            return;

        CreateMonster monsterComponent = targetMonsterGameObject.GetComponent<CreateMonster>();

        if (monsterComponent.listofCurrentStatusEffects.Contains(attackEffectStatus))
        {
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is already {attackEffectStatus.ToString()}!", targetMonster.aiType);
            return;
        }

        float modifierAmount = (amountToChange / 100);

        if (flatValueChange)
            modifierAmount = amountToChange;

        CreateAndAddModifiers(modifierAmount, targetMonster, targetMonsterGameObject, monsterAttackManager.currentMonsterTurnGameObject, this);

        monsterComponent.InflictStatus(attackEffectStatus);

        monsterComponent.UpdateStats(false, null, false);

        monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is {attackEffectStatus.ToString()} by {effectTriggerName}!");

        // Update combat order if speed was adjusted
        if (statToChange == StatToChange.Speed)
        {
            combatManagerScript.SortMonsterBattleSequence();
        }
    }

    public bool CheckAttackEffectTargeting(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager)
    {
        if (monsterTargetType == MonsterTargetType.Self)
        {
            targetMonster = monsterAttackManager.currentMonsterTurn;
            targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
        }

        if (targetMonster == null || targetMonsterGameObject == null)
        {
            return false;
        }

        if (CheckTargetIsImmune(targetMonster, monsterAttackManager, targetMonsterGameObject, this))
            return false;

        if (!CheckAttackEffectHit())
            return false;

        return true;
    }

    public float CalculateStatChange(Monster targetMonster, AttackEffect attackEffect)
    {
        // Grab the original stat that is being changed
        float fromValue = GetBonusDamageSource(statToChange, targetMonster);

        // Calculate the stat change
        float toValue = Mathf.RoundToInt(fromValue * attackEffect.amountToChange / 100);

        // The stat change should atleast be tangible
        if (toValue <= 1)
        {
            toValue = 1; 
        }

        // If the stat change is a flat amount, set it
        if (attackEffect.flatValueChange)
        {
            toValue = attackEffect.amountToChange;
        }

        return toValue;
    }

    public bool CheckAttackEffectHit()
    {
        float hitChance = (effectTriggerChance / 100);
        float randValue = UnityEngine.Random.value;

        if (randValue < hitChance)
        {
            return true;
        }

        return false;
    }

    #region Old Status Effect Code
    /*
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
        if (statToChange == StatToChange.Speed)
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
        if (statToChange == StatToChange.Speed)
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
        if (statToChange == StatToChange.Speed)
        {
            combatManagerScript.SortMonsterBattleSequence();
        }
    }

    // Delegate Debuff Function Test
    public void InflictStunned(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, string effectTriggerName)
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
        if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsStunned == true)
        {
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is already Stunned!", monsterReference.aiType);
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
        CreateAndAddModifiers(toValue, false, monsterReference, monsterReferenceGameObject, modifierDuration, Modifier.StatusEffectType.Stunned, monsterAttackManager.currentMonsterTurnGameObject);

        // Send message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} was Stunned by {effectTriggerName}!");

        // Update monster's stats
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
        monsterReferenceGameObject.GetComponent<CreateMonster>().InflictStatus(Modifier.StatusEffectType.Stunned);

        // Update combat order if speed was adjusted
        if (statToChange == StatToChange.Speed)
        {
            combatManagerScript.SortMonsterBattleSequence();
        }
    }
    */
    #endregion

    // Delegate Buff Function Test
    public void GrantTargetImmunity(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, string effectTriggerName)
    {
        // Check if immune to skip modifiers
        if (CheckTargetIsImmune(monsterReference, monsterAttackManager, monsterReferenceGameObject, this))
        {
            return;
        }

        float toValue = 0;

        // First check if already immune
        switch (statToChange)
        {
            case (StatToChange.Debuffs):
                if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToDebuffs)
                {
                    // Send message to combat log
                    combatManagerScript = monsterAttackManager.combatManagerScript;
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is already immune to status effects and debuffs!");
                    return;
                }
                break;

            //case (StatEnumToChange.Buffs):
            //    if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToBuffs)
            //    {
            //        // Send message to combat log
            //        combatManagerScript = monsterAttackManager.combatManagerScript;
            //        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is already immune to buffs!");
            //        return;
            //    }
            //    break;

            case (StatToChange.Damage):
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
        //CreateModifier(monsterReference, monsterAttackManager, monsterReferenceGameObject, 0, modifierDuration, false);

        // Add modifiers
        //AddModifiers(toValue, false, monsterReference, monsterReferenceGameObject, modifierDuration);
        CreateAndAddModifiers(toValue, false, monsterReference, monsterReferenceGameObject, modifierDuration, monsterAttackManager.currentMonsterTurnGameObject, statToChange);

        // Send message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} gained immunity to status effects and debuffs!");
        //monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Debuff and Status Immunity!");

        // Update monster's stats
        monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);
        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

        // Update combat order if speed was adjusted
        if (statToChange == StatToChange.Speed)
        {
            combatManagerScript.SortMonsterBattleSequence();
        }
    }

    // Check Immunities
    public static bool CheckTargetIsImmune(Monster targetMonster, MonsterAttackManager monsterAttackManager, GameObject targetMonsterGameObject, AttackEffect attackEffect)
    {
        CreateMonster monsterComponent = targetMonsterGameObject.GetComponent<CreateMonster>();

        if (monsterComponent == null)
            return false;

        if (attackEffect.statChangeType == StatChangeType.Buff)
        {
            if (monsterComponent.listofCurrentStatusEffects.Contains(Modifier.StatusEffectType.Crippled))
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Crippled and Immune to Buffs!");
                monsterComponent.CreateStatusEffectPopup("Immune!");
                return true;
            }
        }

        if (attackEffect.statChangeType == StatChangeType.Debuff)
        {
            if (monsterComponent.monsterImmuneToDebuffs)
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Immune to Debuffs!");
                monsterComponent.CreateStatusEffectPopup("Immune!");
                return true;
            }

            if (monsterComponent.listOfElementImmunities.Contains(monsterAttackManager.currentMonsterAttack.monsterElementClass))
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Immune to {monsterAttackManager.currentMonsterAttack.monsterElementClass.ToString()} Element Attacks!");
                monsterComponent.CreateStatusEffectPopup("Immune!");
                return true;
            }

            if (monsterComponent.listOfStatImmunities.Contains(attackEffect.statToChange))
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Immune to {attackEffect.statToChange.ToString()} Debuffs!");
                monsterComponent.CreateStatusEffectPopup("Immune!");
                return true;
            }

            if (monsterComponent.listOfStatusImmunities.Contains(attackEffect.attackEffectStatus) && attackEffect.typeOfEffect == TypeOfEffect.InflictStatusEffect)
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Immune to {attackEffect.attackEffectStatus.ToString()} Status!");
                monsterComponent.CreateStatusEffectPopup("Immune!");
                return true;
            }
        }

        return false;
    }

    // Check Immunities
    public static bool CheckTargetIsImmune(Monster targetMonster, MonsterAttackManager monsterAttackManager, GameObject targetMonsterGameObject, Modifier modifier)
    {
        CreateMonster monsterComponent = targetMonsterGameObject.GetComponent<CreateMonster>();

        if (monsterComponent == null)
            return false;

        if (modifier.statChangeType == StatChangeType.Buff)
        {
            if (monsterComponent.listofCurrentStatusEffects.Contains(Modifier.StatusEffectType.Crippled))
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Crippled and Immune to Buffs!");
                monsterComponent.CreateStatusEffectPopup("Immune!");
                return true;
            }
        }

        if (modifier.statChangeType == StatChangeType.Debuff)
        {
            if (monsterComponent.monsterImmuneToDebuffs)
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Immune to Debuffs!");
                monsterComponent.CreateStatusEffectPopup("Immune!");
                return true;
            }

            if (monsterComponent.listOfStatImmunities.Contains(modifier.statModified))
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Immune to {modifier.statModified.ToString()} Debuffs!");
                monsterComponent.CreateStatusEffectPopup("Immune!");
                return true;
            }

            if (monsterComponent.listOfStatusImmunities.Contains(modifier.statusEffectType) && modifier.isStatusEffect == true)
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Immune to {modifier.statusEffectType.ToString()} Status!");
                monsterComponent.CreateStatusEffectPopup("Immune!");
                return true;
            }
        }

        return false;
    }

    // Create and Add modifiers
    public void CreateAndAddModifiers(float toValue, bool statDecrease, Monster monster, GameObject monsterObj, int duration, GameObject monsterOwnerGameObject)
    {
        // Create and Apply modifier
        Modifier mod = CreateInstance<Modifier>();

        // First check if not buff
        if (statDecrease)
        {
            toValue *= -1;
            mod.statChangeType = StatChangeType.Debuff;
        }
        else
        {
            mod.statChangeType = StatChangeType.Buff;
        }

        // Create and Apply modifier
        mod.modifierSource = name;
        mod.statModified = statToChange;
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
        monsterObj.GetComponent<CreateMonster>().ModifyStats(statToChange, mod);
        //monsterObj.GetComponent<CreateMonster>().AddStatusIcon(mod, statEnumToChange, duration);
    }

    // Create and Add modifiers - New Stat
    public void CreateAndAddModifiers(float toValue, bool statDecrease, Monster monster, GameObject monsterObj, int duration, GameObject monsterOwnerGameObject, StatToChange statEnumToChange)
    {
        // Create and Apply modifier
        Modifier mod = CreateInstance<Modifier>();

        // First check if not buff
        if (statDecrease)
        {
            toValue *= -1;
            mod.statChangeType = StatChangeType.Debuff;
        }
        else
        {
            mod.statChangeType = StatChangeType.Buff;
        }
 
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
        //monsterObj.GetComponent<CreateMonster>().AddStatusIcon(mod, statEnumToChange, duration);
    }

    public void CreateAndAddModifiers(float modifierValue, Monster targetMonster, GameObject targetMonsterGameObject, GameObject attackEffectSourceGameObject, AttackEffect attackEffect)
    {
        // Create Modifier
        Modifier mod = CreateInstance<Modifier>();
        mod.modifierSource = attackEffect.name;
        mod.statModified = attackEffect.statToChange;
        mod.modifierAmount = modifierValue;
        mod.statusEffectType = attackEffect.attackEffectStatus;

        mod.modifierDuration = attackEffect.modifierDuration;
        mod.modifierCurrentDuration = mod.modifierDuration;

        mod.modifierOwnerGameObject = attackEffectSourceGameObject;
        mod.modifierOwner = attackEffectSourceGameObject.GetComponent<CreateMonster>().monsterReference;

        mod.modifierDurationType = mod.modifierDuration == 0 ? Modifier.ModifierDurationType.Permanent : Modifier.ModifierDurationType.Temporary;

        if (mod.statusEffectType != Modifier.StatusEffectType.None)
            mod.isStatusEffect = true;

        targetMonster.ListOfModifiers.Add(mod);
        targetMonsterGameObject.GetComponent<CreateMonster>().ModifyStats(statToChange, mod);
    }

    // 
    public void _AffectTargetStatByAnotherStat(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, string effectTriggerName)
    {
        // Initial float
        float fromValue = 0;

        // Grab new refs?
        //monsterReference = monsterAttackManager.currentTargetedMonster;
        //monsterReferenceGameObject = monsterAttackManager.currentTargetedMonsterGameObject;

        // If the effect applies to user
        if (inflictSelf)
        {
            monsterReference = monsterAttackManager.currentMonsterTurn;
            monsterReferenceGameObject = monsterAttackManager.currentMonsterTurnGameObject;
        }

        // Create reference to monsterComponent
        CreateMonster monsterComponent = monsterReferenceGameObject.GetComponent<CreateMonster>();

        // Calculate stat change
        // What target's stat is being used to calculate change?
        if (scaleFromWhatTarget == ScaleFromWhatTarget.monsterUsingAttack)
        {
            fromValue = GetBonusDamageSource(scaleFromWhatStat, monsterAttackManager.currentMonsterTurn);
        }
        else if (scaleFromWhatTarget == ScaleFromWhatTarget.targetMonster)
        {
            fromValue = GetBonusDamageSource(scaleFromWhatStat, monsterAttackManager.currentTargetedMonster);
        }

        // Change by how much of what stat? 
        float toValue = Mathf.RoundToInt(fromValue * amountToChange / 100);

        if (toValue <= 1)
        {
            toValue = 1; // prevent buffs of 0
        }

        if (flatValueChange)
        {
            toValue = amountToChange;
        }

        // Check if immune to skip modifiers
        if (CheckTargetIsImmune(monsterReference, monsterAttackManager, monsterReferenceGameObject, this))
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
        if (statToChange == StatToChange.CritDamage && fromValue >= 2.5)
        {
            // Send execute message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s " +
                $"{statToChange.ToString()} couldn't go any higher!", monsterReference.aiType);
            monsterComponent.CreateStatusEffectPopup($"No Effect on {statToChange.ToString()}!");
            return;
        }

        // Check if certain stat change reaches upper cap
        if (statToChange == StatToChange.Health && monsterReference.health == monsterReference.maxHealth && statChangeType == StatChangeType.Buff)
        {
            // Send execute message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s " +
                $"{statToChange.ToString()} couldn't go any higher!", monsterReference.aiType);
            monsterComponent.CreateStatusEffectPopup($"No Effect on {statToChange.ToString()}!");
            return;
        }

        // Send message to log
        if (statChangeType == StatChangeType.Buff)
        {
            // Add modifiers
            combatManagerScript = monsterAttackManager.combatManagerScript;
            CreateAndAddModifiers(toValue, false, monsterReference, monsterReferenceGameObject, modifierDuration, monsterAttackManager.currentMonsterTurnGameObject);

            // Update monster's stats
            if (statToChange == StatToChange.Health)
            {
                monsterComponent.UpdateStats(true, null, false); // if health changed, check health
                combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statToChange.ToString()} was increased by {effectTriggerName} +-{toValue})!", monsterReference.aiType);
                monsterComponent.ShowDamageOrStatusEffectPopup(toValue, "Damage");
            }
            else
            {
                monsterComponent.UpdateStats(false, null, false);
                combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statToChange.ToString()} was increased by {effectTriggerName}!", monsterReference.aiType);
                monsterComponent.CreateStatusEffectPopup(statToChange, true);
            }

            // Trigger buff / debuff animation
            monsterComponent.GetComponent<Animator>().SetBool("buffAnimationPlaying", true);
            monsterAttackManager.soundEffectManager.PlaySoundEffect(monsterAttackManager.buffSound);
        }
        else
        {
            // Add modifiers
            combatManagerScript = monsterAttackManager.combatManagerScript;
            CreateAndAddModifiers(toValue, true, monsterReference, monsterReferenceGameObject, modifierDuration, monsterAttackManager.currentMonsterTurnGameObject);

            // Update monster's stats
            if (statToChange == StatToChange.Health)
            {
                monsterComponent.UpdateStats(true, null, false); // if health changed, check health
                combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statToChange.ToString()} was decreased by {effectTriggerName} (-{toValue})!", monsterReference.aiType);
                monsterComponent.ShowDamageOrStatusEffectPopup(toValue, "Damage");
            }
            else
            {
                monsterComponent.UpdateStats(false, null, false);
                combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statToChange.ToString()} was decreased by {effectTriggerName}!", monsterReference.aiType);
                monsterComponent.CreateStatusEffectPopup(statToChange, false);
            }

            // Trigger buff / debuff animation
            monsterComponent.GetComponent<Animator>().SetBool("debuffAnimationPlaying", true);
            monsterAttackManager.soundEffectManager.PlaySoundEffect(monsterAttackManager.debuffSound);
        }

        //// Update monster's stats
        //if (statEnumToChange == StatEnumToChange.Health)
        //{
        //    monsterComponent.UpdateStats(true, null, false); // if health changed, check health
        //}
        //else
        //{
        //    monsterComponent.UpdateStats(false, null, false);
        //}

        monsterComponent.monsterRecievedStatBoostThisRound = true;

        // Update combat order if speed was adjusted
        if (statToChange == StatToChange.Speed)
        {
            combatManagerScript.SortMonsterBattleSequence();
        }
    }
}
