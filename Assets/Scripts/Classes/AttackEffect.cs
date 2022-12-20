using System; // allows serialization of custom classes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "New Monster Attack Effect", menuName = "Monster Attack Effects")]
public class AttackEffect : ScriptableObject
{
    [Title("Effect Initialization Enumerators")]
    public enum TypeOfEffect
    {
        HalfHealthExecute, SpeedBuffAllies, Counter,
        DoublePowerIfStatBoost, OnCriticalStrikeBuff, CripplingFearEffect,
        AddBonusDamage, AddBonusDamageFlat, IncreaseOffensiveStats, HealthCut, BuffTarget, DebuffTarget, GrantImmunity,
        DamageBonusIfTargetStatusEffect, AffectTargetByAnotherStat, InflictStatusEffect, AffectTargetStat, UniqueAttackEffectCondition
    }

    public TypeOfEffect typeOfEffect;

    public enum StatChangeType { Buff, Debuff, None }
    public StatChangeType statChangeType;

    [EnableIf("typeOfEffect", TypeOfEffect.InflictStatusEffect)]
    public Modifier.StatusEffectType attackEffectStatus;

    public enum StatToChange { Health, Mana, PhysicalAttack, MagicAttack, PhysicalDefense, MagicDefense, Speed, Evasion, CritChance, Debuffs, StatChanges, Damage, BothOffensiveStats, CritDamage, MaxHealth, Accuracy, Various, AddOffensiveAttackEffect, HighestAttackStat, Buffs, Immunity}
    public StatToChange statToChange;

    public enum EffectTime { PreAttack, DuringAttack, PostAttack, OnKill, OnDeath, GameStart, RoundStart, RoundEnd, OnStatChange, OnDamageTaken, PreOtherAttack, OnDamageDealt, OnDamageNullified }
    public EffectTime effectTime;

    public enum MonsterTargetType { Target, Self }
    public MonsterTargetType monsterTargetType;

    public bool fixedDamageAmount = false;
    public bool triggersEffects = false;

    [EnableIf("statToChange", StatToChange.Health)]
    public MonsterAttack.MonsterAttackDamageType effectDamageType;

    [EnableIf("statToChange", StatToChange.Health)]
    public ElementClass elementClass;

    [Header("Type Of Effect - Unique Attack Effect Condition")]
    [EnableIf("typeOfEffect", TypeOfEffect.UniqueAttackEffectCondition)]
    public IAbilityTrigger uniqueAttackEffectCondition;

    [Header("Type Of Effect - Affect Target Stat By Another Stat")]
    [EnableIf("typeOfEffect", TypeOfEffect.AffectTargetByAnotherStat)]
    public StatToChange scaleFromWhatStat;
    public enum ScaleFromWhatTarget { monsterUsingAttack, targetMonster, abilitySourceMonster }

    [EnableIf("typeOfEffect", TypeOfEffect.AffectTargetByAnotherStat)]
    public ScaleFromWhatTarget scaleFromWhatTarget;

    [Header("Type Of Effect - Grant Target Immunity")]
    [EnableIf("typeOfEffect", TypeOfEffect.GrantImmunity)]
    public ImmunityType immunityType;
    public enum ImmunityType { None, Element, Status, SpecificStatChange, Damage, Death, Debuffs }

    [EnableIf("immunityType", ImmunityType.Element)]
    public ElementClass elementImmunity;

    [EnableIf("immunityType", ImmunityType.Status)]
    public Modifier.StatusEffectType statusImmunity;

    [EnableIf("immunityType", ImmunityType.SpecificStatChange)]
    public StatToChange statImmunity;

    public enum AttackEffectDuration { Permanent, Temporary }
    [DisplayWithoutEdit]
    [Header("For AttackEffect Instantiation Only")]
    public AttackEffectDuration attackEffectDuration;

    [Title("Modifier Adjustments")]
    public bool flatValueChange = false;

    [PropertyRange(0, 10)]
    public int modifierDuration;

    [PropertyRange(0, 400)]
    public float amountToChange;

    [PropertyRange(0, 100)]
    public float effectTriggerChance;

    public CombatManagerScript combatManagerScript;
    public MonsterAttack monsterAttackTrigger;
    public Monster monsterSource;

    public AttackEffect(StatToChange statEnumToChange, StatChangeType statChangeType, EffectTime effectTime, 
        Modifier.StatusEffectType attackEffectStatus, bool flatValueChange, int modifierDuration, float amountToChange, 
        float effectTriggerChance, CombatManagerScript combatManagerScript)
    {
        this.statToChange = statEnumToChange;
        this.statChangeType = statChangeType;
        this.effectTime = effectTime;
        this.attackEffectStatus = attackEffectStatus;
        this.flatValueChange = flatValueChange;
        this.modifierDuration = modifierDuration;
        this.amountToChange = amountToChange;
        this.effectTriggerChance = effectTriggerChance;
        this.combatManagerScript = combatManagerScript;
    }

    public void ResetAttackEffect()
    {
        this.monsterSource = null;
        this.monsterAttackTrigger = null;
    }

    // Initial function that is called by monsterAttackManager that enacts attack after effects
    public async Task<int> TriggerEffects(MonsterAttackManager monsterAttackManager, string effectTrigger, MonsterAttack attackTrigger)
    {
        Monster targetMonster = monsterAttackManager.currentTargetedMonster;
        GameObject targetMonsterGameObject = monsterAttackManager.currentTargetedMonsterGameObject;

        if (monsterAttackManager.currentMonsterTurn == null)
        {
            Debug.Log("Current monster turn is null. Returning from TriggerEffects!", this);
            return 1;
        }

        if (monsterTargetType == MonsterTargetType.Self)
        {
            targetMonster = monsterAttackManager.currentMonsterTurn;
            targetMonsterGameObject = monsterAttackManager.currentMonsterTurnGameObject;
            Debug.Log($"Target changed to self! {targetMonster.name}\nCurrent Attack Effect: {name} ({attackTrigger.monsterAttackName})");
        }

        if (monsterAttackTrigger == null)
        {
            monsterAttackTrigger = monsterAttackManager.currentMonsterAttack;
        }

        switch (typeOfEffect)
        {
            case TypeOfEffect.AddBonusDamage:
                await AddBonusDamage(targetMonster, monsterAttackManager, targetMonsterGameObject);
                break;

            case TypeOfEffect.AddBonusDamageFlat:
                await AddBonusDamageFlat(targetMonster, monsterAttackManager, targetMonsterGameObject);
                break;

            case TypeOfEffect.GrantImmunity:
                GrantTargetImmunity(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                break;

            case TypeOfEffect.InflictStatusEffect:
                InflictStatusEffect(targetMonster, targetMonsterGameObject, monsterAttackManager, effectTrigger);
                break;

            case TypeOfEffect.DamageBonusIfTargetStatusEffect:
                BonusDamageIfTargetStatusEffect(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                break;

            case TypeOfEffect.AffectTargetStat:
                await AffectTargetStat(targetMonster, targetMonsterGameObject, monsterAttackManager, effectTrigger);
                break;

            case TypeOfEffect.AffectTargetByAnotherStat:
                await AffectTargetStatByAnotherStat(targetMonster, targetMonsterGameObject, monsterAttackManager, effectTrigger, attackTrigger);
                break;

            case TypeOfEffect.UniqueAttackEffectCondition:
                await UniqueAttackEffectCondition(targetMonster, targetMonsterGameObject, monsterAttackManager, effectTrigger);
                break;

            default:
                Debug.Log("Missing attack effect or monster attack reference?", this);
                break;
        }

        return 1;
    }

    public async Task<int> TriggerEffects(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, string effectTrigger, MonsterAttack attackTrigger)
    {
        if (targetMonster == null)
            return 1;

        if (targetMonsterGameObject == null)
            return 1;

        switch (typeOfEffect)
        {
            case TypeOfEffect.AddBonusDamage:
                await AddBonusDamage(targetMonster, monsterAttackManager, targetMonsterGameObject);
                break;

            case TypeOfEffect.AddBonusDamageFlat:
                await AddBonusDamageFlat(targetMonster, monsterAttackManager, targetMonsterGameObject);
                break;

            case TypeOfEffect.GrantImmunity:
                GrantTargetImmunity(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                break;

            case TypeOfEffect.InflictStatusEffect:
                InflictStatusEffect(targetMonster, targetMonsterGameObject, monsterAttackManager, effectTrigger);
                break;

            case TypeOfEffect.DamageBonusIfTargetStatusEffect:
                BonusDamageIfTargetStatusEffect(targetMonster, monsterAttackManager, targetMonsterGameObject, effectTrigger);
                break;

            case TypeOfEffect.AffectTargetStat:
                await AffectTargetStat(targetMonster, targetMonsterGameObject, monsterAttackManager, effectTrigger);
                break;

            case TypeOfEffect.AffectTargetByAnotherStat:
                await AffectTargetStatByAnotherStat(targetMonster, targetMonsterGameObject, monsterAttackManager, effectTrigger, attackTrigger);
                break;

            case TypeOfEffect.UniqueAttackEffectCondition:
                await UniqueAttackEffectCondition(targetMonster, targetMonsterGameObject, monsterAttackManager, effectTrigger);
                break;

            default:
                Debug.Log("Missing attack effect or monster attack reference?", this);
                break;
        }

        return 1;
    }

    private async Task<int> UniqueAttackEffectCondition(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, string effectTrigger)
    {
        await uniqueAttackEffectCondition.TriggerAttackEffect(targetMonster, targetMonsterGameObject, monsterAttackManager, this);

        return 1;
    }

    public void DoublePowerIfStatBoost(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Check if the monster had recieved a stat boost this round
        if (monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound)
        {
            monsterAttackManager.currentMonsterAttack.monsterAttackDamageScalar *= 2f;

            // Send buff message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Gained Bonus Damage!", StatChangeType.Buff);
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} had it's power doubled!");
        }
    }

    public void BonusDamageIfTargetStatusEffect(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject, string effectTriggerName)
    {
        CreateMonster monsterComponent = monsterAttackManager.combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>();

        if (monsterComponent == null)
            return;

        if (monsterComponent.listofCurrentStatusEffects.Contains(attackEffectStatus))
        {
            // calc bonus
            monsterAttackManager.currentMonsterAttack.monsterAttackFlatDamageBonus = amountToChange / 100;

            // Send buff message to combat log
            combatManagerScript = monsterAttackManager.combatManagerScript;
            monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Gained Bonus Damage!", StatChangeType.Buff);
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} gained bonus damage!");
        }
    }

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
                    monsterObj.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatToChange.Speed, statChangeType);

                    // Update monster's speed element
                    monsterObj.GetComponent<CreateMonster>().UpdateStats(false, null, false, 0);
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
                    monsterObj.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatToChange.Speed, statChangeType);

                    // Update monster's speed element
                    monsterObj.GetComponent<CreateMonster>().UpdateStats(false, null, false, 0);
                    monsterObj.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

                    // Does this actually work?
                    combatManagerScript.SortMonsterBattleSequence();
                }
            }
        }
    }

    public void Counter(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Check the amount of damage taken this round
        monsterAttackManager.currentMonsterAttack.monsterAttackFlatDamageBonus = Mathf.RoundToInt(monsterReferenceGameObject.GetComponent<CreateMonster>().monsterDamageTakenThisRound * (amountToChange / 100));

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} gained {monsterAttackManager.currentMonsterAttack.monsterAttackFlatDamageBonus} additional damage!");
    }

    public async Task<int> AddBonusDamageFlat(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Adjust targeting
        monsterReference = monsterAttackManager.currentMonsterTurn;
        monsterReferenceGameObject = monsterAttackManager.currentMonsterTurnGameObject;

        // Get bonus damage amount source
        float bonusAmountSource = GetBonusDamageSource(statToChange, monsterReference);

        // calc bonus
        monsterAttackManager.currentMonsterAttack.monsterAttackFlatDamageBonus = bonusAmountSource * (amountToChange / 100) / 100;

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} gained {monsterAttackManager.currentMonsterAttack.monsterAttackFlatDamageBonus * 100}% bonus damage!");

        await Task.Delay(10);
        return 1;
    }

    public async Task<int> AddBonusDamage(Monster monsterReference, MonsterAttackManager monsterAttackManager, GameObject monsterReferenceGameObject)
    {
        // Get bonus damage amount source
        float bonusAmountSource = GetBonusDamageSource(statToChange, monsterReference);

        // calc bonus
        monsterAttackManager.recievedDamagePercentBonus = true;
        monsterAttackManager.cachedBonusDamagePercent = Mathf.RoundToInt((bonusAmountSource * (amountToChange / 100)) / 2);

        // Send buff message to combat log
        combatManagerScript = monsterAttackManager.combatManagerScript;
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {monsterAttackManager.currentMonsterAttack.monsterAttackName} gained {monsterAttackManager.cachedBonusDamagePercent}% damage bonus!");

        await Task.Delay(10);
        return 1;
    }

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
                    monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatToChange.PhysicalAttack, statChangeType);

                    // Update monster's UI health element
                    monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false, 0);
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
                    monsterReferenceGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup(StatToChange.MagicAttack, statChangeType);

                    // Update monster's UI health element
                    monsterReferenceGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false, 0);
                    monsterReferenceGameObject.GetComponent<CreateMonster>().monsterRecievedStatBoostThisRound = true;

                    break;

                default:
                    Debug.Log("Missing enum type?", this);
                    break;
            }
        }


    }

    #region Generic Attack Effect Methods

    public async Task<int> AffectTargetStat(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, string effectTriggerName)
    {
        if (!CheckAttackEffectTargeting(targetMonster, targetMonsterGameObject, monsterAttackManager))
            return 1;

        Debug.Log($"Attack effect targeting has passed all checks. Calling Attack Effect! {effectTriggerName}");

        //CreateMonster monsterComponent = targetMonsterGameObject.GetComponent<CreateMonster>();

        float statChangeAmount = CalculateStatChange(targetMonster, this, statToChange);

        Debug.Log($"Stat Change Amount calculated: {statChangeAmount}");

        if (CheckIfStatChangeClamps(statChangeAmount, statToChange, targetMonster, targetMonsterGameObject, monsterAttackManager, this))
            return 1;

        Debug.Log("Stat does not clamp. Creating Modifier!");

        await CreateModifier(statChangeAmount, targetMonster, targetMonsterGameObject, monsterAttackManager.currentMonsterTurnGameObject, this, monsterAttackManager);

        return 1;
    }

    public async Task<int> AffectTargetStatByAnotherStat(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, string effectTriggerName, MonsterAttack attackTrigger)
    {
        if (!CheckAttackEffectTargeting(targetMonster, targetMonsterGameObject, monsterAttackManager))
            return 1;

        float fromValue = 1;

        //CreateMonster monsterComponent = targetMonsterGameObject.GetComponent<CreateMonster>();

        // What target is being used to calculate stat change?
        if (scaleFromWhatTarget == ScaleFromWhatTarget.monsterUsingAttack)
        {
            fromValue = GetBonusDamageSource(scaleFromWhatStat, attackTrigger.monsterAttackSource);
            Debug.Log($"Monster Attack Source: {attackTrigger.monsterAttackSource.name}. Value: {fromValue}", this);
        }
        else if (scaleFromWhatTarget == ScaleFromWhatTarget.targetMonster)
        {
            fromValue = GetBonusDamageSource(scaleFromWhatStat, monsterAttackManager.currentTargetedMonster);
            Debug.Log($"Monster Attack Source: {monsterAttackManager.currentTargetedMonster.name}. Value: {fromValue}", this);
        }
        else if (scaleFromWhatTarget == ScaleFromWhatTarget.abilitySourceMonster)
        {
            fromValue = GetBonusDamageSource(scaleFromWhatStat, monsterSource);
            Debug.Log($"Monster Attack Source: {monsterSource.name}. Value: {fromValue}", this);
        }

        float statChangeAmount = CalculateStatChange(targetMonster, this, statToChange, fromValue);

        Debug.Log($"Stat Change Amount: {statChangeAmount}");

        if (CheckIfStatChangeClamps(statChangeAmount, statToChange, targetMonster, targetMonsterGameObject, monsterAttackManager, this))
            return 1;

        monsterAttackTrigger = attackTrigger;

        await CreateModifier(statChangeAmount, targetMonster, targetMonsterGameObject, attackTrigger.monsterAttackSourceGameObject, this, monsterAttackManager);

        return 1;
    }

    public void InflictStatusEffect(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, string effectTriggerName)
    {
        if (!CheckAttackEffectTargeting(targetMonster, targetMonsterGameObject, monsterAttackManager))
            return;

        CreateMonster monsterComponent = targetMonsterGameObject.GetComponent<CreateMonster>();

        if (monsterComponent.listofCurrentStatusEffects.Contains(attackEffectStatus))
        {
            combatManagerScript = monsterAttackManager.combatManagerScript;
            monsterComponent.CreateStatusEffectPopup("No Effect!", StatChangeType.None);
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is already {attackEffectStatus}!", targetMonster.aiType);
            return;
        }

        if (attackEffectStatus == Modifier.StatusEffectType.Enraged)
            targetMonsterGameObject.GetComponent<CreateMonster>().monsterEnragedTarget = monsterAttackTrigger.monsterAttackSourceGameObject;

        float modifierAmount = (amountToChange / 100);

        if (flatValueChange)
            modifierAmount = amountToChange;

        CreateModifier(modifierAmount, targetMonster, targetMonsterGameObject, monsterAttackManager.currentMonsterTurnGameObject, this, monsterAttackManager);

        monsterComponent.InflictStatus(attackEffectStatus);

        monsterComponent.UpdateStats(false, null, false, 0);

        monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is {attackEffectStatus} by {effectTriggerName}!");
    }

    public void GrantTargetImmunity(Monster targetMonster, MonsterAttackManager monsterAttackManager, GameObject targetMonsterGameObject, string effectTriggerName)
    {
        if (!CheckAttackEffectTargeting(targetMonster, targetMonsterGameObject, monsterAttackManager))
            return;

        if (!CheckPreExistingImmunity(targetMonster, targetMonsterGameObject, monsterAttackManager, immunityType, this))
            return;

        CreateMonster monsterComponent = targetMonsterGameObject.GetComponent<CreateMonster>();

        switch (immunityType)
        {
            case ImmunityType.Element:
                monsterComponent.listOfElementImmunities.Add(elementImmunity);
                CreateModifier(amountToChange, targetMonster, targetMonsterGameObject, targetMonsterGameObject, this, monsterAttackManager);
                break;

            case ImmunityType.Status:
                monsterComponent.listOfStatusImmunities.Add(statusImmunity);
                CreateModifier(amountToChange, targetMonster, targetMonsterGameObject, targetMonsterGameObject, this, monsterAttackManager);
                break;

            case ImmunityType.SpecificStatChange:
                monsterComponent.listOfStatImmunities.Add(statImmunity);
                CreateModifier(amountToChange, targetMonster, targetMonsterGameObject, targetMonsterGameObject, this, monsterAttackManager);
                break;

            case ImmunityType.Debuffs:
                monsterComponent.monsterImmuneToDebuffs = true;
                CreateModifier(amountToChange, targetMonster, targetMonsterGameObject, targetMonsterGameObject, this, monsterAttackManager);
                break;

            case ImmunityType.Damage:
                throw new NotImplementedException();

            case ImmunityType.Death:
                throw new NotImplementedException();

            default:
                break;
        }
    }

    public bool CheckPreExistingImmunity(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, ImmunityType _immunityType, AttackEffect attackEffect)
    {
        CreateMonster monsterComponent = targetMonsterGameObject.GetComponent<CreateMonster>();

        switch (_immunityType)
        {
            case ImmunityType.Element:
                if (monsterComponent.listOfElementImmunities.Contains(attackEffect.elementImmunity))
                {
                    monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is already Immune to {attackEffect.elementImmunity.element.ToString()} Element Attacks!");
                    return false;
                }
                break;

            case ImmunityType.Status:
                if (monsterComponent.listofCurrentStatusEffects.Contains(attackEffect.statusImmunity))
                {
                    monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is already Immune to {attackEffect.statusImmunity} Status!");
                    return false;
                }
                break;

            case ImmunityType.SpecificStatChange:
                if (monsterComponent.listOfStatImmunities.Contains(attackEffect.statImmunity))
                {
                    monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is already Immune to {attackEffect.statImmunity} Debuffs!");
                    return false;
                }
                break;

            case ImmunityType.Damage:
                if (monsterComponent.monsterImmuneToDamage)
                {
                    monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is already Immune to Damage!");
                    return false;
                }
                break;

            case ImmunityType.Debuffs:
                if (monsterComponent.monsterImmuneToDebuffs)
                {
                    monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is already Immune to Debuffs!");
                    return false;
                }
                break;

            case ImmunityType.Death:

            default:
                Debug.Log("Missing ImmunityType reference?", this);
                return false;
        }

        return true;
    }

    #endregion Generic Attack Effect Methods

    #region Helper Functions

    public async void CallStatAdjustment(Monster targetMonster, CreateMonster monsterComponent, MonsterAttackManager monsterAttackManager, AttackEffect attackEffect, Modifier modifier)
    {
        //await monsterAttackManager.TriggerAbilityEffects(targetMonster, EffectTime.OnStatChange, monsterComponent.gameObject, modifier); 

        if (typeOfEffect == TypeOfEffect.GrantImmunity)
            return;

        if (attackEffect.monsterSource == null)
            attackEffect.monsterSource = modifier.modifierOwner;

        // Pass in the ability name if necessary
        //if (attackEffect.monsterAttackTrigger == null)
        //    monsterAttackTrigger = monsterAttackManager.currentMonsterAttack;

        if (statChangeType == StatChangeType.Buff)
        {
            monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name}'s {statToChange} was increased by " +
                $"{attackEffect.monsterSource.aiType} {attackEffect.monsterSource.name}'s " +
                $"{attackEffect.name} (+{modifier.modifierAmount})!", targetMonster.aiType);

            if (statToChange == StatToChange.Health)
            {
                monsterComponent.UpdateStats(true, null, false, modifier.modifierAmount, true);
                //monsterComponent.ShowDamageOrStatusEffectPopup(modifier.modifierAmount, "Heal");
                monsterComponent.CreateDamageEffectPopup(modifier.modifierAmount, "Heal");
            }
            else
            {
                monsterComponent.UpdateStats(false, null, false, 0);
                monsterComponent.CreateStatusEffectPopup(statToChange, statChangeType);
            }

            monsterComponent.GetComponent<Animator>().SetBool("buffAnimationPlaying", true);
            monsterAttackManager.soundEffectManager.PlaySoundEffect(monsterAttackManager.buffSound);
        }
        else // if (statChangeType == StatChangeType.Debuff)
        {
            if (modifier.modifierAmount == 0)
                return;

            monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name}'s {statToChange} was decreased by " +
                $"{attackEffect.monsterSource.aiType} {attackEffect.monsterSource.name}'s " +
                $"{attackEffect.name} ({modifier.modifierAmount})!", targetMonster.aiType);

            if (statToChange == StatToChange.Health)
            {
                monsterComponent.UpdateStats(true, null, false, modifier.modifierAmount, true);
                //monsterComponent.ShowDamageOrStatusEffectPopup(modifier.modifierAmount, "Damage");
                monsterComponent.CreateDamageEffectPopup(modifier.modifierAmount, "Damage");
            }
            else
            {
                monsterComponent.UpdateStats(false, null, false, 0);
                monsterComponent.CreateStatusEffectPopup(statToChange, statChangeType);
            }

            monsterComponent.GetComponent<Animator>().SetBool("debuffAnimationPlaying", true);
            monsterAttackManager.soundEffectManager.PlaySoundEffect(monsterAttackManager.debuffSound);
        }
    }

    private bool CheckIfStatChangeClamps(float statChangeAmount, StatToChange statToChange, Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, AttackEffect attackEffect)
    {
        CreateMonster monsterComponent = targetMonsterGameObject.GetComponent<CreateMonster>();
        Debug.Log("Checking if monsterComponent is null!");
        if (monsterComponent == null)
            return false;
        Debug.Log("monsterComponent is not null!");
        if (attackEffect.statChangeType == StatChangeType.Debuff && GetBonusDamageSource(statToChange, targetMonster) <= 1)
        {
            monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name}'s " +
                $"{statToChange} couldn't go any lower!", targetMonster.aiType);
            monsterComponent.CreateStatusEffectPopup($"No Effect on {statToChange}!", StatChangeType.Buff);
            return true;
        }

        if (attackEffect.statChangeType == StatChangeType.Buff)
        {
            switch (statToChange)
            {
                case (StatToChange.CritDamage):
                    if (targetMonster.critDamage >= 2.5f)
                    {
                        monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name}'s " +
                            $"{statToChange} couldn't go any higher!", targetMonster.aiType);
                        monsterComponent.CreateStatusEffectPopup($"No Effect on {statToChange}!", StatChangeType.Buff);
                        return true;
                    }
                    break;

                case (StatToChange.Evasion):
                    if (targetMonster.critDamage >= 99f)
                    {
                        monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name}'s " +
                            $"{statToChange} couldn't go any higher!", targetMonster.aiType);
                        monsterComponent.CreateStatusEffectPopup($"No Effect on {statToChange}!", StatChangeType.Buff);
                        return true;
                    }
                    break;

                case (StatToChange.Health):
                    if (targetMonster.health >= targetMonster.maxHealth)
                    {
                        monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name}'s " +
                            $"{statToChange} couldn't go any higher!", targetMonster.aiType);
                        monsterComponent.CreateStatusEffectPopup($"No Effect on {statToChange}!", StatChangeType.Buff);
                        return true;
                    }
                    break;

                default:
                    return false;
            }
        }

        return false;
    }

    public bool CheckAttackEffectTargeting(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager)
    {
        if (targetMonster == null || targetMonsterGameObject == null || targetMonster.health <= 0)
        {
            Debug.Log("Target is null or dead, returning!");
            return false;
        }

        if (CheckTargetIsImmune(targetMonster, monsterAttackManager, targetMonsterGameObject, this))
            return false;

        if (!CheckAttackEffectHit())
            return false;

        return true;
    }

    public float CalculateStatChange(Monster targetMonster, AttackEffect attackEffect, StatToChange _statToChange)
    {
        // Grab the original stat that is being changed
        float fromValue = GetBonusDamageSource(_statToChange, targetMonster);

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

        Debug.Log($"Stat Change Amount calculated: {toValue}");
        return toValue;
    }

    public float CalculateStatChange(Monster targetMonster, AttackEffect attackEffect, StatToChange _statToChange, float statSource)
    {
        // Grab the original stat that is being changed
        float fromValue = statSource;

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
            Debug.Log($"Attack Effect Hit!\n{name}");
            return true;
        }

        Debug.Log($"Attack Effect Missed!\n{name}");
        return false;
    }

    public static bool CheckTargetIsImmune(Monster targetMonster, MonsterAttackManager monsterAttackManager, GameObject targetMonsterGameObject, AttackEffect attackEffect)
    {
        CreateMonster monsterComponent = targetMonsterGameObject.GetComponent<CreateMonster>();

        Debug.Log("Checking if monsterComponent is null!");

        if (monsterComponent == null)
            return false;

        Debug.Log("MonsterComponent is not null!");

        if (attackEffect.statChangeType == StatChangeType.Buff)
        {
            if (monsterComponent.listofCurrentStatusEffects.Contains(Modifier.StatusEffectType.Crippled))
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Crippled and Immune to Buffs!");
                monsterComponent.CreateStatusEffectPopup("Immune!!", StatChangeType.Buff);
                return true;
            }
        }

        if (attackEffect.statChangeType == StatChangeType.Debuff)
        {
            if (monsterComponent.monsterImmuneToDebuffs)
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Immune to Debuffs!");
                monsterComponent.CreateStatusEffectPopup("Immune!!", StatChangeType.Buff);
                return true;
            }

            if (monsterComponent.listOfElementImmunities.Contains(attackEffect.elementClass))
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Immune to {attackEffect.elementClass.element} Element Attacks!");
                monsterComponent.CreateStatusEffectPopup("Immune!!", StatChangeType.Buff);
                return true;
            }

            if (monsterComponent.listOfStatImmunities.Contains(attackEffect.statToChange))
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Immune to {attackEffect.statToChange} Debuffs!");
                monsterComponent.CreateStatusEffectPopup("Immune!!", StatChangeType.Buff);
                return true;
            }

            if (monsterComponent.listOfStatusImmunities.Contains(attackEffect.attackEffectStatus) && attackEffect.typeOfEffect == TypeOfEffect.InflictStatusEffect)
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Immune to {attackEffect.attackEffectStatus} Status!");
                monsterComponent.CreateStatusEffectPopup("Immune!!", StatChangeType.Debuff);
                return true;
            }
        }

        return false;
    }

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
                monsterComponent.CreateStatusEffectPopup("Immune!", StatChangeType.Debuff);
                return true;
            }
        }

        if (modifier.statChangeType == StatChangeType.Debuff)
        {
            if (monsterComponent.monsterImmuneToDebuffs)
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Immune to Debuffs!");
                monsterComponent.CreateStatusEffectPopup("Immune!", StatChangeType.Buff);
                return true;
            }

            if (monsterComponent.listOfStatImmunities.Contains(modifier.statModified))
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Immune to {modifier.statModified} Debuffs!");
                monsterComponent.CreateStatusEffectPopup("Immune!", StatChangeType.Buff);
                return true;
            }

            if (monsterComponent.listOfStatusImmunities.Contains(modifier.statusEffectType) && modifier.isStatusEffect == true)
            {
                monsterAttackManager.combatManagerScript.CombatLog.SendMessageToCombatLog($"{targetMonster.aiType} {targetMonster.name} is Immune to {modifier.statusEffectType} Status!");
                monsterComponent.CreateStatusEffectPopup("Immune!", StatChangeType.Buff);
                return true;
            }
        }

        return false;
    }

    float GetBonusDamageSource(StatToChange statEnumToChange, Monster monsterRef)
    {
        Debug.Log($"Getting stat source...");

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

    #endregion Helper Functions

    public async Task<int> CreateModifier(float modifierValue, Monster targetMonster, GameObject targetMonsterGameObject, GameObject attackEffectSourceGameObject, AttackEffect attackEffect, MonsterAttackManager monsterAttackManager)
    {
        // Create Modifier
        Modifier mod = CreateInstance<Modifier>();
        mod.attackEffect = attackEffect;

        mod.modifierSource = attackEffect.name;
        mod.statModified = attackEffect.statToChange;
        mod.modifierAmount = modifierValue;
        mod.statusEffectType = attackEffect.attackEffectStatus;
        mod.statChangeType = attackEffect.statChangeType;

        mod.modifierDuration = attackEffect.modifierDuration;
        mod.modifierCurrentDuration = mod.modifierDuration;

        if (attackEffectSourceGameObject == null)
            attackEffectSourceGameObject = targetMonsterGameObject; // return 1

        mod.modifierOwnerGameObject = attackEffectSourceGameObject;
        mod.modifierOwner = attackEffectSourceGameObject.GetComponent<CreateMonster>().monsterReference;

        mod.modifierDurationType = mod.modifierDuration == 0 ? Modifier.ModifierDurationType.Permanent : Modifier.ModifierDurationType.Temporary;

        if (mod.statusEffectType == Modifier.StatusEffectType.None)
            mod.modifierAmount *= mod.statChangeType == StatChangeType.Buff ? 1 : -1;

        if (mod.statusEffectType != Modifier.StatusEffectType.None)
            mod.isStatusEffect = true;

        Debug.Log($"Modifier Created! \n" +
            $"AttackEffect Source: {mod.attackEffect.name} \n" +
            $"Stat Modified: {mod.statModified} \n" +
            $"Amount: {mod.modifierAmount} \n" +
            $"Status Effect Type: {mod.isStatusEffect}, {mod.statusEffectType} \n" +
            $"Stat Change Type: {mod.statChangeType} \n" +
            $"Duration: {mod.modifierCurrentDuration}/{mod.modifierDuration} \n" +
            $"Owner: {mod.modifierOwner.name}, {mod.modifierOwnerGameObject.name}");

        targetMonster.ListOfModifiers.Add(mod);
        await targetMonsterGameObject.GetComponent<CreateMonster>().ModifyStats(attackEffect, mod);

        return 1;

        //if (mod.statusEffectType == Modifier.StatusEffectType.None)
            //CallStatAdjustment(targetMonster, targetMonsterGameObject.GetComponent<CreateMonster>(), monsterAttackManager, attackEffect.name, mod);
    }
}
