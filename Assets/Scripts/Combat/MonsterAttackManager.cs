using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class MonsterAttackManager : MonoBehaviour
{
    [Title("Current Components")]
    public MonsterAttack currentMonsterAttack;
    public Monster currentMonsterTurn;
    public GameObject currentMonsterTurnGameObject;

    public GameObject currentTargetedMonsterGameObject;
    public Monster currentTargetedMonster;
    public List<GameObject> ListOfTargetedMonsters;

    [Title("Required Components")]
    [Required] public ButtonManagerScript buttonManagerScript;
    [Required] public CombatManagerScript combatManagerScript;
    [Required] public HUDAnimationManager HUDanimationManager;
    [Required] public EnemyAIManager enemyAIManager;
    [Required] public UIManager uiManager;
    [Required] public MessageManager CombatLog;
    [Required] public SoundEffectManager soundEffectManager;

    [Title("UI Elements")]
    public TextMeshProUGUI currentMonsterAttackDescription;
    public Image TextBackImage;
    public Image TextBackImageBorder; // temporary

    public GameObject monsterAttackMissText;
    public GameObject monsterCritText;

    public Sprite poisonedUISprite;
    public Sprite noUISprite;
    public Sprite burningUISprite;
    public Sprite dazedUISprite;

    [Title("Combat Variables")]
    public Vector3 cachedTransform;
    public float cachedDamage;

    public float calculatedDamage = 0;
    public float bonusDamagePercent = 0;
    public float cachedBonusDamagePercent = 0;
    public bool recievedDamagePercentBonus = false;

    public bool dontDealDamage = false;

    [Title("Combat Audio Elements")]
    public AudioClip CritSound;
    public AudioClip HitSound;
    public AudioClip MissSound;
    public AudioClip LevelUpSound;

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
    }

    // This functions initializes the gameObject's components
    public void InitializeComponents()
    {
        buttonManagerScript = GetComponent<ButtonManagerScript>();
        combatManagerScript = GetComponent<CombatManagerScript>();
        HUDanimationManager = GetComponent<HUDAnimationManager>();
        enemyAIManager = GetComponent<EnemyAIManager>();
        CombatLog = GetComponent<MessageManager>();
        uiManager = GetComponent<UIManager>();
        soundEffectManager = GetComponent<SoundEffectManager>();

        currentMonsterAttackDescription.gameObject.SetActive(false);
        TextBackImage.enabled = false;
        TextBackImageBorder.enabled = false;
    }

    // This function assigns the monster attack that is connected to the pressed button
    public void AssignCurrentButtonAttack(string buttonNumber)
    {
        currentMonsterAttack = buttonManagerScript.ListOfMonsterAttacks[int.Parse(buttonNumber)];
        currentMonsterTurn = combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference; // unrelated to UI popup (missing reassignment calls?)
        combatManagerScript.CurrentMonsterAttack = currentMonsterAttack;

        if (currentMonsterAttack.attackOnCooldown)
        {
            uiManager.EditCombatMessage($"{currentMonsterAttack.monsterAttackName} is on cooldown!");
            return;
        }

        combatManagerScript.TargetingEnemyMonsters(true);
        UpdateCurrentTargetText();

        buttonManagerScript.HideAllButtons("AttacksHUDButtons");
        buttonManagerScript.ShowButton("ConfirmButton");

        SetMonsterAttackDescriptionText(currentMonsterAttack.monsterAttackType);
    }

    // This function sets the monster attack descrption box text
    public void SetMonsterAttackDescriptionText(MonsterAttack.MonsterAttackType attackType)
    {
        switch (attackType) {

            case (MonsterAttack.MonsterAttackType.Other):
                // Fallthrough

            case (MonsterAttack.MonsterAttackType.Buff):
                currentMonsterAttackDescription.gameObject.SetActive(true);
                currentMonsterAttackDescription.text = ($"{currentMonsterAttack.monsterAttackName}" +
                    $"\n{currentMonsterAttack.monsterAttackDescription}" +
                    $"\nBuff Type: ({currentMonsterAttack.monsterAttackType.ToString()}) | Accuracy: {currentMonsterAttack.monsterAttackAccuracy}%" +
                    $"\nElement: {currentMonsterAttack.monsterAttackElement.ToString()}");
                TextBackImage.enabled = true;
                TextBackImageBorder.enabled = true;
                break;

            default:
                currentMonsterAttackDescription.gameObject.SetActive(true);
                currentMonsterAttackDescription.text = ($"{currentMonsterAttack.monsterAttackName}" +
                    $"\n{currentMonsterAttack.monsterAttackDescription}" +
                    $"\nBase Power: {currentMonsterAttack.monsterAttackDamage} ({currentMonsterAttack.monsterAttackDamageType.ToString()}) | Accuracy: {currentMonsterAttack.monsterAttackAccuracy}%" +
                    $"\nElement: {currentMonsterAttack.monsterAttackElement.ToString()}");
                TextBackImage.enabled = true;
                TextBackImageBorder.enabled = true;
                break;
        }
    }

    // This function updates the targeted enemy text on screen
    public void UpdateCurrentTargetText()
    {
        // Fixes a weird targeting bug
        if (combatManagerScript.CurrentTargetedMonster == null)
        {
            return;
        }

        // Is the monster targeting itself?
        if (combatManagerScript.CurrentMonsterTurn == combatManagerScript.CurrentTargetedMonster)
        {
            //Debug.Log("I got called!");
            HUDanimationManager.MonsterCurrentTurnText.text = ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name} " +
                $"will use {ReturnCurrentButtonAttack().monsterAttackName} on self?");
        }
        else
        {
            //Debug.Log("I got called!");
            HUDanimationManager.MonsterCurrentTurnText.text = ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name} " +
                $"will use {ReturnCurrentButtonAttack().monsterAttackName} " +
                $"on {combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.aiType} " +
                $"{combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.name}?");
        }
    }

    // This function assigns the monster attack that is connected to the pressed button
    public MonsterAttack ReturnCurrentButtonAttack()
    {
        return currentMonsterAttack;
    }

    // This function clears the assigned monster attask
    public void ClearCurrentButtonAttack()
    {
        currentMonsterAttack = null;
    }

    // This function serves as a reset to visual elements on screen (monster attack description)
    public void ResetHUD()
    {
        currentMonsterAttackDescription.text = "";
        TextBackImage.enabled = false;
        TextBackImageBorder.enabled = false;

        currentMonsterAttackDescription.gameObject.SetActive(false);
    }

    // This coroutine serves as a delay after the attack button is selected in order to check dazed/target re-direction etc.
    IEnumerator TargetMonsterCheck()
    {
        // Enemy AI manager handles this
        if (combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Enemy)
        {
            yield return new WaitForSeconds(1.3f);
            Invoke("ConfirmMonsterAttack", 0.1f);
            yield break;
        }

        // fix missing target bug?
        currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
        currentTargetedMonster = currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterReference; // wtf?

        // Check if current monster is dazed
        if (currentMonsterTurnGameObject.GetComponent<CreateMonster>().monsterIsDazed)
        {
            // 50% chance to select a different move
            float selectAnotherMove = Random.value;

            // Not selecting another move, continue
            if (selectAnotherMove < .5f)
            {
                uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} is will use {currentMonsterAttack.monsterAttackName} on {currentTargetedMonster.aiType} {currentTargetedMonster.name}!");
                yield return new WaitForSeconds(1.3f);
                Invoke("ConfirmMonsterAttack", 0.1f);
                yield break;
            }

            // Selecting another move since Dazed
            MonsterAttack dazedAttackChoice = combatManagerScript.GetRandomMove();

            // Check selected move target type
            if (dazedAttackChoice.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.SelfTarget)
            {
                combatManagerScript.CurrentTargetedMonster = combatManagerScript.CurrentMonsterTurn; // Update the combat manager, since this script references it later

                // Update targeting
                currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
                currentTargetedMonster = currentMonsterTurnGameObject.GetComponent<CreateMonster>().monsterReference; // wtf?

                uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} is Dazed and will use {currentMonsterAttack.monsterAttackName} on itself!");
                yield return new WaitForSeconds(1.3f);
                Invoke("ConfirmMonsterAttack", 0.1f);
                yield break;
            }
            else // Not targeting self only, grab a random list, allys or enemies
            {
                combatManagerScript.CurrentTargetedMonster = enemyAIManager.GetRandomTarget(enemyAIManager.GetRandomList()); // this is quite the line of code

                // Update targeting
                currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
                currentTargetedMonster = currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterReference; // wtf?

                uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} is Dazed and will use {currentMonsterAttack.monsterAttackName} on {currentTargetedMonster.aiType} {currentTargetedMonster.name}!");
                yield return new WaitForSeconds(1.3f);
                Invoke("ConfirmMonsterAttack", 0.1f);
                yield break;
            }
        }

        // Not dazed, continue
        if (currentTargetedMonsterGameObject == currentMonsterTurnGameObject)
        {
            uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} will use {currentMonsterAttack.monsterAttackName} on itself!");
        }
        else
        {
            uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} will use {currentMonsterAttack.monsterAttackName} on {currentTargetedMonster.aiType} {currentTargetedMonster.name}!");
        }

        yield return new WaitForSeconds(1.3f);
        Invoke("ConfirmMonsterAttack", 0.1f);
        yield break;
    }

    // This function uses the selected monster attack on the selected monster
    public void UseMonsterAttack()
    {
        // Are you trying to target the wrong target?
        if (currentMonsterAttack.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.SelfTarget && combatManagerScript.CurrentTargetedMonster != combatManagerScript.CurrentMonsterTurn)
        {
            uiManager.EditCombatMessage($"{currentMonsterAttack.monsterAttackName} can only be used on self!");
            return;
        }

        // Hide buttons and Attack UI and display targeting text
        combatManagerScript.targeting = false;
        HideButtonsAndAttackUI();

        // Idk what this does
        dontDealDamage = false;

        // Assign this scripts current monster turn obj and monster ref from combat manager
        currentMonsterTurnGameObject = combatManagerScript.CurrentMonsterTurn;
        currentMonsterTurn = combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference; // unrelated to UI popup (missing reassignment calls?)

        // Check if dazed/target-redirect/etc.
        StartCoroutine(TargetMonsterCheck());
    }

    // This function extends from UseMonsterAttack to work with the new coroutine
    public void ConfirmMonsterAttack()
    {
        // take away action
        currentMonsterTurnGameObject.GetComponent<CreateMonster>().monsterActionAvailable = false;

        // does attack have a cooldown? if so, activate it
        if (currentMonsterAttack.attackHasCooldown)
        {
            currentMonsterAttack.attackOnCooldown = true;
            currentMonsterAttack.attackCurrentCooldown = currentMonsterAttack.attackBaseCooldown;
        }

        // First check if the move should deal damage or not
        if (currentMonsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Buff)
        {
            currentTargetedMonster = combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
            currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;

            UseMonsterBuff();
            return;
        }

        // Not using a buff
        // Get the list of targeted monsters
        if (combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Enemy)
        {
            ListOfTargetedMonsters = combatManagerScript.ListOfEnemies;
        }
        else
        {
            ListOfTargetedMonsters = combatManagerScript.ListOfAllys;
        }

        // Trigger animations and sounds
        combatManagerScript.CurrentMonsterTurnAnimator.SetBool("attackAnimationPlaying", true);
        QueueAttackSound();
        UpdateHUDElements();
    }

    // This function is called when the monster attack does not deal damage
    public void UseMonsterBuff()
    {
        // Are you trying to target the wrong target?
        if (currentMonsterAttack.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.SelfTarget && currentTargetedMonsterGameObject != currentMonsterTurnGameObject)
        {
            uiManager.EditCombatMessage($"{currentMonsterAttack.monsterAttackName} can only be used on self!");
            return;
        }

        // Use buff animation play
        //combatManagerScript.CurrentMonsterTurnAnimator = combatManagerScript.CurrentMonsterTurn.GetComponent<Animator>();
        //combatManagerScript.CurrentMonsterTurnAnimator.SetBool("useBuffAnimationPlaying", true);

        // Check if the attack actually hits
        if (CheckAttackHit(true))
        {
            dontDealDamage = true;
            QueueAttackSound();
            soundEffectManager.BeginSoundEffectQueue();
            UpdateHUDElements();

            combatManagerScript.CurrentMonsterTurnAnimator = combatManagerScript.CurrentTargetedMonster.GetComponent<Animator>();
            combatManagerScript.CurrentMonsterTurnAnimator.SetBool("buffAnimationPlaying", true);

            // Send log message
            if (currentMonsterTurn == currentTargetedMonster) // targeting self?
            {
                CombatLog.SendMessageToCombatLog($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} used {currentMonsterAttack.monsterAttackName} " +
                    $"on itself!");
            }
            else // targeting ssomething else
            {
                CombatLog.SendMessageToCombatLog($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} used {currentMonsterAttack.monsterAttackName} on " +
                    $"{currentTargetedMonster.aiType} {currentTargetedMonster.name}!");
            }

            Monster monsterWhoUsedAttack = currentMonsterTurn;
            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);

            TriggerPostAttackEffects(monsterWhoUsedAttack);

            combatManagerScript.monsterTargeter.SetActive(false);
            combatManagerScript.targeting = false;

            combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
        }
        else // Miss!
        {
            dontDealDamage = true;

            QueueAttackSound();
            soundEffectManager.BeginSoundEffectQueue();
            UpdateHUDElements();
            AttackMissed();
        }
    }

    // Update HUD elements
    public void UpdateHUDElements()
    {
        buttonManagerScript.HideAllButtons("All");
        ResetHUD();
        uiManager.EditCombatMessage();
    }

    // Update HUD elements
    public void HideButtonsAndAttackUI()
    {
        buttonManagerScript.HideAllButtons("All");
        ResetHUD();
    }

    // Queue Attack Sound
    public void QueueAttackSound()
    {
        AudioClip monsterAttackSound = currentMonsterAttack.monsterAttackSoundEffect;
        soundEffectManager.AddSoundEffectToQueue(monsterAttackSound);
    }

    // trigger post attack effects
    public void TriggerPostAttackEffects(Monster monsterWhoUsedAttack)
    {
        // Trigger all attack after effects (buffs, debuffs etc.) - TODO - Implement other buffs/debuffs and durations
        if (currentMonsterTurnGameObject != null && monsterWhoUsedAttack.health > 0)
        {
            foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
            {
                if (effect.effectTime == AttackEffect.EffectTime.PostAttack)
                {
                    effect.TriggerEffects(this, currentMonsterAttack.monsterAttackName);
                }
            }
        }
    }

    // trigger during attack effects
    public void TriggerDuringAttackEffects(Monster monsterWhoUsedAttack)
    {
        // Trigger all attack after effects (buffs, debuffs etc.) - TODO - Implement other buffs/debuffs and durations
        if (currentMonsterTurnGameObject != null && monsterWhoUsedAttack.health > 0)
        {
            foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
            {
                if (effect.effectTime == AttackEffect.EffectTime.DuringAttack)
                {
                    effect.TriggerEffects(this, currentMonsterAttack.monsterAttackName);
                }
            }
        }
    }

    // pre attack function
    public void PreAttackEffectCheck()
    {
        // Pre Attack Effects Go Here
        foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
        {
            if (effect.effectTime == AttackEffect.EffectTime.PreAttack)
            {
                effect.TriggerEffects(this, currentMonsterAttack.monsterAttackName);
            }
        }
    }

    // This function uses the current move to deal damage to the target (should be called after attack animation ends)
    public void DealDamage()
    {
        // First check if the attack should deal damage to more than one target
        if (currentMonsterAttack.monsterAttackTargetCount != MonsterAttack.MonsterAttackTargetCount.SingleTarget)
        {
            DealDamageAll();
            return;
        }

        PreAttackEffectCheck();

        currentTargetedMonster = combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
        currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;

        if (currentTargetedMonsterGameObject != null)
        {
            cachedTransform = currentTargetedMonsterGameObject.transform.position;
            cachedTransform.y += 1;
        }

        // Check miss?
        if (CheckAttackHit(false))
        {
            float calculatedDamage = CalculatedDamage(combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference, currentMonsterAttack);
            currentTargetedMonster.health -= calculatedDamage;
            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterDamageTakenThisRound += calculatedDamage;

            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().ShowDamageOrStatusEffectPopup(calculatedDamage);
            currentTargetedMonsterGameObject.GetComponent<Animator>().SetBool("hitAnimationPlaying", true);

            soundEffectManager.AddSoundEffectToQueue(HitSound);
            soundEffectManager.BeginSoundEffectQueue();

            Monster monsterWhoUsedAttack = currentMonsterTurn;
            GameObject monsterWhoUsedAttackGameObject = currentMonsterTurnGameObject;

            monsterWhoUsedAttack.health = currentMonsterTurn.health;
            monsterWhoUsedAttack.cachedDamageDone += calculatedDamage;

            TriggerPostAttackEffects(monsterWhoUsedAttack);

            //currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().UpdateStats(true, null, false);

            // Add to killcount if applicable
            if (combatManagerScript.adventureMode && monsterWhoUsedAttack.aiType == Monster.AIType.Ally && monsterWhoUsedAttack != currentTargetedMonster)
            {
                if (currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterReference.health <= 0)
                {
                    monsterWhoUsedAttack.monsterKills += 1;
                    monsterWhoUsedAttackGameObject.GetComponent<CreateMonster>().GrantExp(15 * currentTargetedMonster.level);
                }
            }

            combatManagerScript.monsterTargeter.SetActive(false);
            combatManagerScript.targeting = false;

            combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
        }
        else
        {
            // Don't advance turn
            AttackMissed(true);

            // Main target miss, damage others clasue for multi target attacks
            foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
            {
                if (effect.effectTime == AttackEffect.EffectTime.PostAttack)
                {
                    if (effect.typeOfEffect == AttackEffect.TypeOfEffect.DamageAllEnemies)
                    {
                        effect.TriggerEffects(this, currentMonsterAttack.monsterAttackName);
                    }
                }
            }

            // Invoke after
            ClearCachedModifiers();
            combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
        }
    }

    // This function is called if the current move should deal damage to all targets
    public void DealDamageAll()
    {
        // Check pre attack effects, buffs etc.
        PreAttackEffectCheck();
        Monster monsterWhoUsedAttack = currentMonsterTurn;

        foreach (GameObject monsterObj in ListOfTargetedMonsters.ToArray())
        {
            // Get Monster Reference
            Monster monster = monsterObj.GetComponent<CreateMonster>().monsterReference;

            // Set target references
            currentTargetedMonster = monster;
            currentTargetedMonsterGameObject = monsterObj;

            // Set cached transform for popups
            cachedTransform = currentTargetedMonsterGameObject.transform.position;
            cachedTransform.y += 1;

            // Check attack hit or miss?
            if (CheckAttackHit(false))
            {
                // Get damage calc and deal damage
                float calculatedDamage = CalculatedDamage(combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference, currentMonsterAttack);
                currentTargetedMonster.health -= calculatedDamage;
                currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterDamageTakenThisRound += calculatedDamage;

                // Visual and Sound cues
                currentTargetedMonsterGameObject.GetComponent<CreateMonster>().ShowDamageOrStatusEffectPopup(calculatedDamage);
                currentTargetedMonsterGameObject.GetComponent<Animator>().SetBool("hitAnimationPlaying", true);
                soundEffectManager.AddSoundEffectToQueue(HitSound);
                soundEffectManager.BeginSoundEffectQueue();

                GameObject monsterWhoUsedAttackGameObject = currentMonsterTurnGameObject;
                monsterWhoUsedAttack = currentMonsterTurn;
                monsterWhoUsedAttack.health = currentMonsterTurn.health;
                monsterWhoUsedAttack.cachedDamageDone += calculatedDamage;

                // Trigger DURING attack effects
                TriggerDuringAttackEffects(monsterWhoUsedAttack);

                // End of turn stuff
                currentTargetedMonsterGameObject.GetComponent<CreateMonster>().UpdateStats(true, null, false);

                // Add to killcount if applicable
                if (combatManagerScript.adventureMode)
                {
                    if (currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterReference.health <= 0)
                    {
                        monsterWhoUsedAttack.monsterKills += 1;
                        monsterWhoUsedAttackGameObject.GetComponent<CreateMonster>().GrantExp(15 * currentTargetedMonster.level);
                    }
                }

                combatManagerScript.monsterTargeter.SetActive(false);
                combatManagerScript.targeting = false;
            }
            else
            {
                // Don't advance turn
                AttackMissed(true);
            }
        }

        // Trigger post attack effects
        monsterWhoUsedAttack = currentMonsterTurn;
        TriggerPostAttackEffects(monsterWhoUsedAttack);

        // Out of loop, next turn
        ClearCachedModifiers();
        combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
    }

    // This function is called when an attack misses
    public void AttackMissed()
    {
        CombatLog.SendMessageToCombatLog
                ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.aiType} " +
                $"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name}'s " +
                $"{currentMonsterAttack.monsterAttackName} missed {currentTargetedMonster.aiType} {currentTargetedMonster.name}!", currentMonsterTurn.aiType);

        soundEffectManager.AddSoundEffectToQueue(MissSound);
        soundEffectManager.BeginSoundEffectQueue();

        //monsterAttackMissText.SetActive(true);
        //monsterAttackMissText.transform.position = cachedTransform;

        currentTargetedMonsterGameObject.GetComponent<CreateMonster>().ShowDamageOrStatusEffectPopup("Miss");
        combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
    }

    // This override function is called when an AOE Attack misses
    public void AttackMissed(bool DontAdvanceTurn)
    {
        CombatLog.SendMessageToCombatLog
                ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.aiType} " +
                $"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name}'s " +
                $"{currentMonsterAttack.monsterAttackName} missed {currentTargetedMonster.aiType} {currentTargetedMonster.name}!", currentMonsterTurn.aiType);

        soundEffectManager.AddSoundEffectToQueue(MissSound);
        soundEffectManager.BeginSoundEffectQueue();

        //monsterAttackMissText.SetActive(true);
        //monsterAttackMissText.transform.position = cachedTransform;

        currentTargetedMonsterGameObject.GetComponent<CreateMonster>().ShowDamageOrStatusEffectPopup("Miss");
    }

    // This function is called to check accuracy and evasion
    public bool CheckAttackHit(bool selfTargeting)
    {
        // first check if never miss
        if (currentMonsterAttack.monsterAttackNeverMiss)
        {
            return true;
        }

        float hitChance = 0f;

        // don't use evasion as a stat when self-targeting or ally targeting
        if (!selfTargeting || currentMonsterTurn.aiType != currentTargetedMonster.aiType)
        {
            hitChance = (currentMonsterAttack.monsterAttackAccuracy - currentTargetedMonster.evasion) / 100;
            //Debug.Log($"Attack: {currentMonsterAttack.monsterAttackName}, Hit chance: {hitChance}% & not targeting self or ally!");
        }
        else
        {
            hitChance = (currentMonsterAttack.monsterAttackAccuracy / 100);
            //Debug.Log($"Attack: {currentMonsterAttack.monsterAttackName}, Hit chance: {hitChance}%");
        }

        float randValue = Random.value;

        if (randValue < hitChance)
        {
            return true;
        }

        return false;
    }

    // This function is called to check if an attack critically hits
    public bool CheckAttackCrit()
    {
        float critChance = (currentMonsterAttack.monsterAttackCritChance + combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.critChance) / 100;
        float randValue = Random.value;

        if (randValue < critChance)
        {
            return true;
        }

        return false;
    }

    // This function returns the current monster's highest attack stat for split or true damage
    MonsterAttack.MonsterAttackDamageType ReturnMonsterHighestAttackStat(Monster currentMonster)
    {
        if (currentMonster.physicalAttack > currentMonster.magicAttack)
        {
            return MonsterAttack.MonsterAttackDamageType.Physical;
        }
        else
        {
            return MonsterAttack.MonsterAttackDamageType.Magical;
        }
    }

    // This function returns a damage number based on attack, defense + other calcs
    public float CalculatedDamage(Monster currentMonster, MonsterAttack monsterAttack)
    {
        calculatedDamage = 0;
        bonusDamagePercent = 0;
        MonsterAttack.MonsterAttackDamageType highestStatType;

        if (recievedDamagePercentBonus)
        {
            bonusDamagePercent = cachedBonusDamagePercent;
            recievedDamagePercentBonus = false;
        }

        // Calculate resistances
        switch (monsterAttack.monsterAttackDamageType)
        {
            case (MonsterAttack.MonsterAttackDamageType.Magical):
                //calculatedDamage = Mathf.RoundToInt(currentMonster.magicAttack + (currentMonster.magicAttack * Mathf.RoundToInt(bonusDamagePercent + currentMonsterAttack.monsterAttackDamage * .1f)) * (1 / (currentTargetedMonster.magicDefense + 1)));
                calculatedDamage = Mathf.RoundToInt((100 * currentMonster.magicAttack * Mathf.RoundToInt(bonusDamagePercent + currentMonsterAttack.monsterAttackDamage * .1f)) / (125 + currentTargetedMonster.magicDefense));
                break;

            case (MonsterAttack.MonsterAttackDamageType.Physical):
                //calculatedDamage = Mathf.RoundToInt(currentMonster.physicalAttack + (currentMonster.physicalAttack * Mathf.RoundToInt(bonusDamagePercent + currentMonsterAttack.monsterAttackDamage * .1f)) * (1 / (currentTargetedMonster.physicalDefense + 1)));
                calculatedDamage = Mathf.RoundToInt((100 * currentMonster.physicalAttack * Mathf.RoundToInt(bonusDamagePercent + currentMonsterAttack.monsterAttackDamage * .1f)) / (125 + currentTargetedMonster.physicalDefense));
                break;

            case (MonsterAttack.MonsterAttackDamageType.True):
                highestStatType = ReturnMonsterHighestAttackStat(currentMonster);
                if (highestStatType == MonsterAttack.MonsterAttackDamageType.Magical)
                {
                    calculatedDamage = currentMonster.magicAttack + (currentMonster.magicAttack * Mathf.RoundToInt(bonusDamagePercent + currentMonsterAttack.monsterAttackDamage * .1f));
                }
                else
                {
                    calculatedDamage = currentMonster.physicalAttack + (currentMonster.physicalAttack * Mathf.RoundToInt(bonusDamagePercent + currentMonsterAttack.monsterAttackDamage * .1f));
                }
                break;

            case (MonsterAttack.MonsterAttackDamageType.Split):
                highestStatType = ReturnMonsterHighestAttackStat(currentMonster);
                if (highestStatType == MonsterAttack.MonsterAttackDamageType.Magical)
                {
                    calculatedDamage = Mathf.RoundToInt(currentMonster.magicAttack + (currentMonster.magicAttack * Mathf.RoundToInt(bonusDamagePercent + currentMonsterAttack.monsterAttackDamage * .1f)) * (1 / (currentTargetedMonster.magicDefense + 1)));
                }
                else
                {
                    calculatedDamage = Mathf.RoundToInt(currentMonster.physicalAttack + (currentMonster.physicalAttack * Mathf.RoundToInt(bonusDamagePercent + currentMonsterAttack.monsterAttackDamage * .1f)) * (1 / (currentTargetedMonster.physicalDefense + 1)));
                }
                break;

            default:
                Debug.Log("Missing attack or attack type reference?", this);
                break;
        }

        // Clear cache
        bonusDamagePercent = 0;
        cachedBonusDamagePercent = 0;

        // Check if resistances are TOO high or base damage TOO low; if so, make the base damage at minimum 1% of their max health
        //if (calculatedDamage <= Mathf.RoundToInt(currentTargetedMonster.maxHealth * 0.05f))
        //{
        //    Debug.Log("Stat check resistance buff!");
        //    calculatedDamage = Mathf.RoundToInt(currentTargetedMonster.maxHealth * 0.05f);
        //}

        // Check for additional flat bonus damage
        calculatedDamage += monsterAttack.monsterAttackFlatDamageBonus;

        // Check monster main element bonus
        if (monsterAttack.monsterAttackElement == currentMonster.monsterElement)
        {
            calculatedDamage += Mathf.RoundToInt(calculatedDamage * .25f);
        }

        // Check sub element bonus
        if (monsterAttack.monsterAttackElement == currentMonster.monsterSubElement)
        {
            calculatedDamage += Mathf.RoundToInt(calculatedDamage * .15f);
        }

        // Now check for critical hit
        if (CheckAttackCrit())
        {
            calculatedDamage = Mathf.RoundToInt(calculatedDamage * currentMonster.critDamage);
            CombatLog.SendMessageToCombatLog($"Critical Hit!!! {currentMonster.aiType} {currentMonster.name} used {currentMonsterAttack.monsterAttackName} " +
                $"on {currentTargetedMonster.aiType} {currentTargetedMonster.name} for {calculatedDamage} damage!", currentMonsterTurn.aiType);

            monsterCritText.SetActive(true);
            monsterCritText.transform.position = cachedTransform;

            cachedDamage = calculatedDamage;
            soundEffectManager.AddSoundEffectToQueue(CritSound);

            currentMonsterTurnGameObject.GetComponent<CreateMonster>().monsterCriticallyStrikedThisRound = true;

            return calculatedDamage;
        }

        // Send log message
        CombatLog.SendMessageToCombatLog($"{currentMonster.aiType} {currentMonster.name} used {currentMonsterAttack.monsterAttackName} " +
            $"on {currentTargetedMonster.aiType} {currentTargetedMonster.name} for {calculatedDamage} damage!", currentMonsterTurn.aiType);

        cachedDamage = calculatedDamage;
        return calculatedDamage;
    }

    // This function clears cached damage bonuses etc. to prevent unwanted buffs
    public void ClearCachedModifiers()
    {
        bonusDamagePercent = 0;
        cachedBonusDamagePercent = 0;
        recievedDamagePercentBonus = false;
        combatManagerScript.monsterTargeter.SetActive(false);
    }

    // This function uses the current move to deal damage to the other targets (should be called initial hit)
    /*public void DealDamageOthers(GameObject otherSpecificTarget)
    {
        //yield return new WaitForSeconds(0);

        currentTargetedMonster = otherSpecificTarget.GetComponent<CreateMonster>().monsterReference;
        Monster monster = otherSpecificTarget.GetComponent<CreateMonster>().monsterReference;

        // Pre Attack Effects Go Here 
        foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
        {
            if (effect.effectTime == AttackEffect.EffectTime.PreAttack)
            {
                effect.TriggerEffects(this);
            }
        }

        //currentTargetedMonster = combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
        //currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;

        if (otherSpecificTarget != null)
        {
            cachedTransform = otherSpecificTarget.transform.position;
            cachedTransform.y += 1;
        }

        if (CheckAttackHit(false))
        {
            float calculatedDamage = CalculatedDamage(combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference, currentMonsterAttack);
            monster.health -= calculatedDamage;
            otherSpecificTarget.GetComponent<CreateMonster>().monsterDamageTakenThisRound += calculatedDamage;

            otherSpecificTarget.GetComponent<CreateMonster>().UpdateStats();
            //Debug.Log("You've reached update stats!");

            otherSpecificTarget.GetComponent<Animator>().SetBool("hitAnimationPlaying", true);

            soundEffectManager.AddSoundEffectToQueue(HitSound);
            soundEffectManager.BeginSoundEffectQueue();

            Monster monsterWhoUsedAttack = currentMonsterTurn;
            monsterWhoUsedAttack.health = currentMonsterTurn.health;

            // Trigger all attack after effects (buffs, debuffs etc.) - TODO - Implement other buffs/debuffs and durations
            if (currentMonsterTurnGameObject != null && monsterWhoUsedAttack.health > 0)
            {
                foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
                {
                    // Don't forever trigger damage
                    if (effect.typeOfEffect == AttackEffect.TypeOfEffect.DamageAllEnemies)
                    {
                        continue;
                    }

                    if (effect.effectTime == AttackEffect.EffectTime.PostAttack)
                    {
                        effect.TriggerEffects(this);
                    }
                }
            }

            //currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
        }
        else
        {
            CombatLog.SendMessageToCombatLog
                ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.aiType} " +
                $"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name}'s " +
                $"{currentMonsterAttack.monsterAttackName} missed {currentTargetedMonster.aiType} {currentTargetedMonster.name}!");

            soundEffectManager.AddSoundEffectToQueue(MissSound);
            soundEffectManager.BeginSoundEffectQueue();

            monsterAttackMissText.SetActive(true);
            monsterAttackMissText.transform.position = cachedTransform;
        }
    }*/
}
