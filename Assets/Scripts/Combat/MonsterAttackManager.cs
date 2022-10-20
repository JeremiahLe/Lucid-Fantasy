using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using System.Threading.Tasks;
using System.Linq;

public class MonsterAttackManager : MonoBehaviour
{
    [Title("Current Components")]
    public MonsterAttack currentMonsterAttack;
    public Monster currentMonsterTurn;
    public GameObject currentMonsterTurnGameObject;

    public GameObject currentTargetedMonsterGameObject;
    public Monster currentTargetedMonster;

    public List<GameObject> ListOfTargetMonsters;
    public List<GameObject> ListOfCurrentlyTargetedMonsters;

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

    public GameObject monsterCombatPredictionWindow;
    public TextMeshProUGUI monsterCombatPredictionText;

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

    public float elementCheckDamageBonus = 0;
    private float backRowDamagePercentBonus = 0.85f;
    private float frontRowDamagePercentBonus = 1.15f;

    public bool dontDealDamage = false;

    [Title("Combat Audio Elements")]
    public AudioClip CritSound;
    public AudioClip HitSound;
    public AudioClip MissSound;
    public AudioClip LevelUpSound;
    public AudioClip ResistSound;
    public AudioClip debuffSound;
    public AudioClip buffSound;


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

        combatManagerScript.TargetingEnemyMonsters(true, currentMonsterAttack);
        UpdateCurrentTargetText();

        buttonManagerScript.HideAllButtons("AttacksHUDButtons");
        buttonManagerScript.ShowButton("ConfirmButton");
        buttonManagerScript.ShowButton("ReturnToAttacksButton");

        SetMonsterAttackDescriptionText(currentMonsterAttack.monsterAttackType);
    }

    // This function sets the monster attack descrption box text
    public void SetMonsterAttackDescriptionText(MonsterAttack.MonsterAttackType attackType)
    {
        switch (attackType) {

            case (MonsterAttack.MonsterAttackType.Other):
            // Fallthrough

            case (MonsterAttack.MonsterAttackType.Status):
                currentMonsterAttackDescription.gameObject.SetActive(true);
                currentMonsterAttackDescription.text = ($"<b>{currentMonsterAttack.monsterAttackName} | {currentMonsterAttack.monsterAttackElement.ToString()}</b>" +
                    $"\n{currentMonsterAttack.monsterAttackDescription}" +
                    $"\n<b>Buff/Debuff Type: {currentMonsterAttack.monsterAttackTargetType.ToString()} " +
                    $"| Accuracy: {currentMonsterAttack.monsterAttackAccuracy}% " +
                    $"({ReturnProperAccuracyBasedOnTarget()}%)</b>");
                //$"\nElement: {currentMonsterAttack.monsterAttackElement.ToString()}");
                TextBackImage.enabled = true;
                TextBackImageBorder.enabled = true;
                break;

            default:
                currentMonsterAttackDescription.gameObject.SetActive(true);
                currentMonsterAttackDescription.text = ($"<b>{currentMonsterAttack.monsterAttackName} | {currentMonsterAttack.monsterAttackElement.ToString()}</b>" +
                    $"\n{currentMonsterAttack.monsterAttackDescription}" +
                    $"\n<b>Base Power: {currentMonsterAttack.monsterAttackDamage} (x{CheckElementWeakness(currentMonsterAttack.monsterAttackElement, combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference, false)}) ({currentMonsterAttack.monsterAttackDamageType.ToString()}) " +
                    $"| Accuracy: {currentMonsterAttack.monsterAttackAccuracy}% " +
                    $"({ReturnProperAccuracyBasedOnTarget()}%)</b>");
                //$"\nElement: {currentMonsterAttack.monsterAttackElement.ToString()}");
                TextBackImage.enabled = true;
                TextBackImageBorder.enabled = true;
                break;
        }
    }

    // This function returns the proper accuracy number if targeting ally or enemy
    public float ReturnProperAccuracyBasedOnTarget()
    {
        float rowAccuracyBonus = 0f;

        // Check row accuracy bonus
        if (combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterRowPosition == CreateMonster.MonsterRowPosition.BackRow)
        {
            rowAccuracyBonus = 5f;
        }
        else if (combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterRowPosition == CreateMonster.MonsterRowPosition.FrontRow)
        {
            rowAccuracyBonus = -5f;
        }

        float targetingEnemyAccuracy = currentMonsterAttack.monsterAttackAccuracy + currentMonsterTurn.bonusAccuracy + rowAccuracyBonus - combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.evasion;
        float targetingAllyAccuracy = currentMonsterAttack.monsterAttackAccuracy + currentMonsterTurn.bonusAccuracy + rowAccuracyBonus;

        // Are you targeting an ally? Do not show their evasion in accuracy calculation
        if (combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Ally)
        {
            return targetingAllyAccuracy;
        }

        // Else, targeting enemy, show evasion in accuracy calculation
        return targetingEnemyAccuracy;
    }

    // This function updates the targeted enemy text on screen
    public void UpdateCurrentTargetText()
    {
        // Focus camera on currently targeted monster
        //combatManagerScript.FocusCamera(combatManagerScript.CurrentTargetedMonster);

        // Update accuracy check - TODO - Don't show allies evasion
        SetMonsterAttackDescriptionText(currentMonsterAttack.monsterAttackType);

        // Fixes a weird targeting bug
        if (combatManagerScript.CurrentTargetedMonster == null)
        {
            return;
        }

        // Check if the current attack is a multi-target attack
        if (currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.MultiTarget)
        {
            HUDanimationManager.MonsterCurrentTurnText.text = 
                ($"Select {currentMonsterAttack.monsterAttackTargetCountNumber - ListOfCurrentlyTargetedMonsters.Count} target(s)...");
            return;
        }

        // Check if the current attack targets all allies / enemies
        if (currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.AllTargets)
        {
            if (combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Enemy)
            {
                HUDanimationManager.MonsterCurrentTurnText.text = ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name} " +
                $"will use {ReturnCurrentButtonAttack().monsterAttackName} " +
                $"on all Enemies?");
            }
            else
            {
                HUDanimationManager.MonsterCurrentTurnText.text = ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name} " +
                $"will use {ReturnCurrentButtonAttack().monsterAttackName} " +
                $"on all Allies?");
            }
            return;
        }

        // Is the monster targeting itself?
        if (combatManagerScript.CurrentMonsterTurn == combatManagerScript.CurrentTargetedMonster)
        {
            HUDanimationManager.MonsterCurrentTurnText.text = ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name} " +
                $"will use {ReturnCurrentButtonAttack().monsterAttackName} on self?");
            return;
        }
        else if (combatManagerScript.CurrentMonsterTurn != combatManagerScript.CurrentTargetedMonster && ReturnCurrentButtonAttack().monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.SelfTarget)
        {
            HUDanimationManager.MonsterCurrentTurnText.text = ($"Invalid Target!");
            return;
        }

        // If not, display default combat targeting message
        HUDanimationManager.MonsterCurrentTurnText.text = ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name} " +
            $"will use {ReturnCurrentButtonAttack().monsterAttackName} " +
            $"on {combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.aiType} " +
            $"{combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.name}?");
    }

    // This function assigns the monster attack that is connected to the pressed button
    public MonsterAttack ReturnCurrentButtonAttack()
    {
        return currentMonsterAttack;
    }

    // This function clears the assigned monster attask
    public void ClearCurrentButtonAttack()
    {
        combatManagerScript.TargetingEnemyMonsters(false, currentMonsterAttack);
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
        // Reset camera to neutral position
        //combatManagerScript.ResetCamera();

        // Enemy AI manager handles this
        //if (combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Enemy)
        //{
        //    yield return new WaitForSeconds(1.3f);
        //    Invoke("ConfirmMonsterAttack", 0.1f);
        //    yield break;
        //}

        // fix missing target bug?
        currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
        currentTargetedMonster = currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterReference; // wtf?

        // Check if current monster is dazed
        if (currentMonsterTurnGameObject.GetComponent<CreateMonster>().listofCurrentStatusEffects.Contains(Modifier.StatusEffectType.Dazed))
        {
            // 50% chance to select a different move
            float selectAnotherMove = Random.value;

            // Not selecting another move, continue
            if (selectAnotherMove < 0.5f)
            {
                //uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} is Dazed and will use {currentMonsterAttack.monsterAttackName} on {currentTargetedMonster.aiType} {currentTargetedMonster.name}!");
                yield return new WaitForSeconds(0.1f);
                UpdateCombatText(true);
                yield break;
            }

            // Selecting another move since Dazed
            MonsterAttack dazedAttackChoice = combatManagerScript.GetRandomMove();
            currentMonsterAttack = dazedAttackChoice;

            // Check selected move target type
            if (dazedAttackChoice.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.SelfTarget)
            {
                combatManagerScript.CurrentTargetedMonster = combatManagerScript.CurrentMonsterTurn; // Update the combat manager, since this script references it later

                // Update targeting
                currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
                currentTargetedMonster = currentMonsterTurnGameObject.GetComponent<CreateMonster>().monsterReference; // wtf?
                combatManagerScript.UpdateTargeterPosition(currentMonsterTurnGameObject);

                //uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} is Dazed and will use {currentMonsterAttack.monsterAttackName} on itself!");

                yield return new WaitForSeconds(0.1f);
                UpdateCombatText(true);
                yield break;
            }
            else // Not targeting self only, grab a random list, allys or enemies
            {
                combatManagerScript.CurrentTargetedMonster = enemyAIManager.GetRandomTarget(enemyAIManager.GetRandomList()); // this is quite the line of code

                // Update targeting
                currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
                currentTargetedMonster = currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterReference; // wtf?
                combatManagerScript.UpdateTargeterPosition(currentMonsterTurnGameObject);

                //// Check if targeting self
                //if (currentTargetedMonsterGameObject == currentMonsterTurnGameObject)
                //{
                //    uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} is Dazed and will use {currentMonsterAttack.monsterAttackName} on itself!");
                //}
                //else
                //{
                //    uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} is Dazed and will use {currentMonsterAttack.monsterAttackName} on {currentTargetedMonster.aiType} {currentTargetedMonster.name}!");
                //}

                yield return new WaitForSeconds(0.1f);
                UpdateCombatText(true);
                yield break;
            }
        }

        //// Not dazed, continue
        //// Else single target attack
        //if (currentTargetedMonsterGameObject == currentMonsterTurnGameObject)
        //{
        //    uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} will use {currentMonsterAttack.monsterAttackName} on itself!");
        //}
        //else
        //{
        //    uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} will use {currentMonsterAttack.monsterAttackName} on {currentTargetedMonster.aiType} {currentTargetedMonster.name}!");
        //}

        //// If the current attack is targets everything
        //if (currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.AllTargets)
        //{
        //    if (currentTargetedMonster.aiType == Monster.AIType.Ally)
        //    {
        //        uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} will use {currentMonsterAttack.monsterAttackName} on all Allies!");
        //    }
        //    else
        //    {
        //        uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} will use {currentMonsterAttack.monsterAttackName} on all Enemies!");
        //    }
        //}

        //// Add to list of currently targeted monsters
        //if (currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.SingleTarget)
        //{
        //    ListOfCurrentlyTargetedMonsters.Add(combatManagerScript.CurrentTargetedMonster);
        //}
        //else if (currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.AllTargets)
        //{
        //    if (combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Enemy)
        //    {
        //        foreach (GameObject monster in combatManagerScript.ListOfEnemies.OrderByDescending(monster => monster.GetComponent<CreateMonster>().monsterReference.speed).ToList())
        //            ListOfCurrentlyTargetedMonsters.Add(monster);
        //    }
        //    else
        //    {
        //        foreach (GameObject monster in combatManagerScript.ListOfAllys.OrderByDescending(monster => monster.GetComponent<CreateMonster>().monsterReference.speed).ToList())
        //            ListOfCurrentlyTargetedMonsters.Add(monster);
        //    }
        //}

        yield return new WaitForSeconds(0.1f);
        UpdateCombatText(false);
        yield break;
    }

    // This function is called by the TargetMonsterCheck Ienumerator after it assigns targets
    public void UpdateCombatText(bool monsterIsDazed)
    {
        if (monsterIsDazed)
        {
            // Dazed, continue
            // Else single target attack
            if (currentTargetedMonsterGameObject == currentMonsterTurnGameObject)
            {
                uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} is Dazed and will use {currentMonsterAttack.monsterAttackName} on itself!");
            }
            else
            {
                uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} is Dazed and will use {currentMonsterAttack.monsterAttackName} on {currentTargetedMonster.aiType} {currentTargetedMonster.name}!");
            }

            // If the current attack is targets everything
            if (currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.AllTargets)
            {
                if (currentTargetedMonster.aiType == Monster.AIType.Ally)
                {
                    uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} is Dazed and will use {currentMonsterAttack.monsterAttackName} on all Allies!");
                }
                else
                {
                    uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} is Dazed and will use {currentMonsterAttack.monsterAttackName} on all Enemies!");
                }
            }
        }
        else
        {
            // Not dazed, continue
            // Else single target attack
            if (currentTargetedMonsterGameObject == currentMonsterTurnGameObject)
            {
                uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} will use {currentMonsterAttack.monsterAttackName} on itself!");
            }
            else
            {
                uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} will use {currentMonsterAttack.monsterAttackName} on {currentTargetedMonster.aiType} {currentTargetedMonster.name}!");
            }

            //
            if (currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.MultiTarget)
            {
                uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} will use {currentMonsterAttack.monsterAttackName} on ");

                foreach (GameObject monsterGameObject in ListOfCurrentlyTargetedMonsters)
                {
                    CreateMonster monsterComponent = monsterGameObject.GetComponent<CreateMonster>();
                    Monster monster = monsterComponent.monsterReference;

                    HUDanimationManager.MonsterCurrentTurnText.text += 
                        ($"{monster.aiType} {monster.name}");

                    if (ListOfCurrentlyTargetedMonsters.IndexOf(monsterGameObject) == ListOfCurrentlyTargetedMonsters.Count - 1)
                    {
                        HUDanimationManager.MonsterCurrentTurnText.text += ("!");
                    }
                    else
                    {
                        HUDanimationManager.MonsterCurrentTurnText.text += (" and ");
                    }
                }
            }

            // If the current attack is targets everything
            if (currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.AllTargets)
            {
                if (currentTargetedMonster.aiType == Monster.AIType.Ally)
                {
                    uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} will use {currentMonsterAttack.monsterAttackName} on all Allies!");
                }
                else
                {
                    uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} will use {currentMonsterAttack.monsterAttackName} on all Enemies!");
                }
            }
        }

        // Add to list of currently targeted monsters
        if (currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.SingleTarget)
        {
            ListOfCurrentlyTargetedMonsters.Add(combatManagerScript.CurrentTargetedMonster);
        }
        else if (currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.AllTargets)
        {
            if (combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Enemy)
            {
                foreach (GameObject monster in combatManagerScript.ListOfEnemies.OrderByDescending(monster => monster.GetComponent<CreateMonster>().monsterReference.speed).ToList())
                    ListOfCurrentlyTargetedMonsters.Add(monster);
            }
            else
            {
                foreach (GameObject monster in combatManagerScript.ListOfAllys.OrderByDescending(monster => monster.GetComponent<CreateMonster>().monsterReference.speed).ToList())
                    ListOfCurrentlyTargetedMonsters.Add(monster);
            }
        }

        // Targeter position fix for
        //combatManagerScript.UpdateTargeterPosition(ListOfCurrentlyTargetedMonsters[0]);

        Invoke("ConfirmMonsterAttack", 1.3f);
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
    public async void ConfirmMonsterAttack()
    {
        // take away action
        currentMonsterTurnGameObject.GetComponent<CreateMonster>().monsterActionAvailable = false;

        // does attack have a cooldown? if so, activate it
        if (currentMonsterAttack.attackHasCooldown)
        {
            currentMonsterAttack.attackOnCooldown = true;
            currentMonsterAttack.attackCurrentCooldown = currentMonsterAttack.attackBaseCooldown;
        }

        //// First check if the move should deal damage or not
        //if (currentMonsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Buff)
        //{
        //    currentTargetedMonster = combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
        //    currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;

        //    UseMonsterBuff();
        //    return;
        //}

        // Not using a buff
        // Get the list of targeted monsters
        if (currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.AllTargets)
        {
            if (combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Enemy)
            {
                combatManagerScript.CurrentTargetedMonster = combatManagerScript.ListOfEnemies[0];
            }
            else
            {
                combatManagerScript.CurrentTargetedMonster = combatManagerScript.ListOfAllys[0];
            }
        }

        // Trigger animations and sounds
        await TriggerPreAttackEffects();

        combatManagerScript.CurrentMonsterTurnAnimator.SetBool("attackAnimationPlaying", true);
        QueueAttackSound();
        UpdateHUDElements();
    }

    // This function spawns a monster attack effect at the target's position
    public void SpawnAttackEffect()
    {
        // Spawn Effect on Target
        if (currentMonsterAttack.AttackVFX != null)
        {
            GameObject attackEffect = Instantiate(currentMonsterAttack.AttackVFX, currentTargetedMonsterGameObject.transform.position + (transform.forward * -2f), transform.rotation);
            attackEffect.GetComponent<Renderer>().sortingOrder = 20;
        }
    }

    // This function is called when the monster attack does not deal damage
    /*
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
            // Second, check specific monster element immunities
            if (currentTargetedMonsterGameObject.GetComponent<CreateMonster>().listOfElementImmunities.Contains(currentMonsterAttack.monsterElementClass))
            {
                CombatLog.SendMessageToCombatLog($"{currentTargetedMonster.aiType} {currentTargetedMonster.name} is immune to {currentMonsterAttack.monsterElementClass.element.ToString()} damage!");
                currentTargetedMonsterGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Immune!");
                combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
                return;
            }

            dontDealDamage = true;
            QueueAttackSound();
            soundEffectManager.BeginSoundEffectQueue();
            UpdateHUDElements();

            //combatManagerScript.CurrentMonsterTurnAnimator = combatManagerScript.CurrentTargetedMonster.GetComponent<Animator>();
            //combatManagerScript.CurrentMonsterTurnAnimator.SetBool("buffAnimationPlaying", true);

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
            GameObject monsterWhoUsedAttackGameObject = currentMonsterTurnGameObject;
            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().UpdateStats(false, null, false);

            StartCoroutine(TriggerPostAttackEffects(monsterWhoUsedAttack, monsterWhoUsedAttackGameObject));

            combatManagerScript.monsterTargeter.SetActive(false);
            combatManagerScript.targeting = false;

            //combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
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
    */

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
    public async Task<int> TriggerPostAttackEffects(Monster monsterWhoUsedAttack, GameObject monsterWhoUsedAttackGameObject)
    {
        // Trigger all attack after effects (buffs, debuffs etc.) - TODO - Implement other buffs/debuffs and durations
        if (currentMonsterTurnGameObject != null && monsterWhoUsedAttack.health > 0 && currentTargetedMonster.health > 0)
        {
            foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
            {
                if (effect.effectTime == AttackEffect.EffectTime.PostAttack && effect.monsterTargetType == AttackEffect.MonsterTargetType.Target)
                {
                    Debug.Log("Triggering Effect!");
                    effect.TriggerEffects(this, currentMonsterAttack.monsterAttackName, currentMonsterAttack);
                    await Task.Delay(300);
                }
            }
        }

        // Afterward, trigger self effects if still alive
        if (currentMonsterTurnGameObject != null && monsterWhoUsedAttack.health > 0)
        {
            foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
            {
                if (effect.effectTime == AttackEffect.EffectTime.PostAttack && effect.monsterTargetType == AttackEffect.MonsterTargetType.Self)
                {
                    effect.TriggerEffects(this, currentMonsterAttack.monsterAttackName, currentMonsterAttack);
                    await Task.Delay(300);
                }
            }
        }

        return 1;
    }

    // trigger during attack effects
    public async Task<int> TriggerDuringAttackEffects(Monster monsterWhoUsedAttack, GameObject monsterWhoUsedAttackGameObject)
    {
        // Trigger all attack after effects (buffs, debuffs etc.) - TODO - Implement other buffs/debuffs and durations
        if (currentMonsterTurnGameObject != null && monsterWhoUsedAttack.health > 0 && currentTargetedMonster.health > 0)
        {
            foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
            {
                if (effect.effectTime == AttackEffect.EffectTime.DuringAttack)
                {
                    effect.TriggerEffects(this, currentMonsterAttack.monsterAttackName, currentMonsterAttack);
                    await Task.Delay(300);
                }
            }
        }

        return 1;
    }

    // pre attack function
    public async Task<int> TriggerPreAttackEffects()
    {
        // Pre Attack Effects Go Here
        foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
        {
            if (effect.effectTime == AttackEffect.EffectTime.PreAttack)
            {
                effect.TriggerEffects(this, currentMonsterAttack.monsterAttackName, currentMonsterAttack);
                await Task.Delay(300);
            }
        }

        return 1;
    }

    // This function handles damaging one targeted monster
    public async void DealDamage()
    {
        if (currentMonsterAttack.monsterAttackTargetCount != MonsterAttack.MonsterAttackTargetCount.SingleTarget)
        {
            DealDamageToMultipleTargets();
            return;
        }

        // Update current target reference
        currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
        currentTargetedMonster = currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterReference;
        CreateMonster currentTargetedMonsterComponent = currentTargetedMonsterGameObject.GetComponent<CreateMonster>();

        // Cache the monster who used the attack
        Monster monsterWhoUsedAttack = currentMonsterTurn;
        GameObject monsterWhoUsedAttackGameObject = currentMonsterTurnGameObject;

        // Trigger any pre-attack effects the current selected MonsterAttack may have (This is now called in 
        //await TriggerPreAttackEffects();

        // Grab the targeted monster's transform for status effect popups
        cachedTransform = currentTargetedMonsterGameObject.transform.position;
        cachedTransform.y += 1;

        // Hide the targeter
        currentTargetedMonsterComponent.monsterTargeterUIGameObject.SetActive(false);

        // Check if the attack hits or misses
        if (!CheckAttackHit(false))
        {
            AttackMissed();
            ClearCachedModifiers();
            return;
        }

        // We didn't miss, continue function calls
        SpawnAttackEffect();

        // Calculate damage dealt and deal damage to targeted monster
        float calculatedDamage = CalculatedDamage(combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference, currentMonsterAttack);
        currentTargetedMonster.health -= calculatedDamage;

        // Cache damage dealt and taken for both target and user
        currentTargetedMonsterComponent.monsterDamageTakenThisRound += calculatedDamage;
        monsterWhoUsedAttack.cachedDamageDone += calculatedDamage;

        // If the attack was a damaging attack and the target wasn't immune, show damage done
        if (currentMonsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Attack && calculatedDamage != 0)
        {
            // Show the damage number and damaged animation of target
            currentTargetedMonsterGameObject.GetComponent<Animator>().SetBool("hitAnimationPlaying", true);
            currentTargetedMonsterComponent.ShowDamageOrStatusEffectPopup(calculatedDamage, "Damage");

            // Play damage sound
            soundEffectManager.AddSoundEffectToQueue(HitSound);
            soundEffectManager.BeginSoundEffectQueue();

            // Trigger any on damage ability effects if necesary
            await TriggerAbilityEffects(currentMonsterTurn, AttackEffect.EffectTime.OnDamageDealt, currentMonsterTurnGameObject);

            // Trigger any damage taken effects on target
            await TriggerAbilityEffects(currentTargetedMonster, AttackEffect.EffectTime.OnDamageTaken, currentTargetedMonsterGameObject);
        }

        // Finally, update the stats of the targeted monster, aka CheckHealth()
        currentTargetedMonsterComponent.UpdateStats(true, currentMonsterTurnGameObject, false);

        // Check if the damage killed the targeted monster
        if (CheckIfDamagedKilledMonster(monsterWhoUsedAttack, monsterWhoUsedAttackGameObject))
        {
            // Damage killed monster
            // Break out before continuing further in case self died from OnDamageTaken or OnDeath Abilities, or some other external factor
            if (currentMonsterTurnGameObject == null || currentMonsterTurn.health <= 0)
            {
                // Self is dead, continue combat loop
                Debug.Log("Self is dead, continue combat loop!", this);
                combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
                return;
            }

            // Self is not dead, continue function calls
            // Grant Exp for Kill and if self leveled up
            if (currentMonsterTurnGameObject.GetComponent<CreateMonster>().GrantExpAndCheckLevelup(CalculateExp()))
            {
                // Leveled up, do not trigger post ability effects, post attack effects, and next monster turn until after the level up window is closed
                //
                return;
            }
            else
            {
                // Did not level up, continue function calls
                // Trigger any On-Kill ability effects if necessary
                await TriggerAbilityEffects(currentMonsterTurn, AttackEffect.EffectTime.OnKill, currentMonsterTurnGameObject);

                // Trigger any post attack effects **(should only be self) (Null checks are handled in this function)
                await TriggerPostAttackEffects(monsterWhoUsedAttack, monsterWhoUsedAttackGameObject);

                // Trigger any post attack ability effects **(should only be self) (Null checks are handled in this function)
                await TriggerAbilityEffects(monsterWhoUsedAttack, AttackEffect.EffectTime.PostAttack, monsterWhoUsedAttackGameObject);
            }

        }
        else
        {
            // Damage did not kill monster, continue function calls
            // If the damage was not immune and the current attack was a status move
            if (calculatedDamage != 0 || currentMonsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Status)
            {
                // Trigger any post attack effects on self or target (Null checks are handled in this function)
                await TriggerPostAttackEffects(monsterWhoUsedAttack, monsterWhoUsedAttackGameObject);

                // Trigger any post attack ability effects **(should only be self) (Null checks are handled in this function)
                await TriggerAbilityEffects(monsterWhoUsedAttack, AttackEffect.EffectTime.PostAttack, monsterWhoUsedAttackGameObject);
            }
        }

        // Clear Target List
        ListOfCurrentlyTargetedMonsters.Clear();

        // Finally, call next monster turn
        combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
    }

    // This function handles damaging multiple targeted monsters
    public async void DealDamageToMultipleTargets()
    {
        // Cache the monster who used the attack
        Monster monsterWhoUsedAttack = currentMonsterTurn;
        GameObject monsterWhoUsedAttackGameObject = currentMonsterTurnGameObject;
        int totalExpGained = 0;

        foreach (GameObject targetedMonster in ListOfCurrentlyTargetedMonsters)
        {
            if (currentMonsterTurnGameObject == null)
            {
                break;
            }

            if (targetedMonster == null)
            {
                continue;
            }

            // Update current target reference
            combatManagerScript.CurrentTargetedMonster = targetedMonster;
            currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
            currentTargetedMonster = currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterReference;
            CreateMonster currentTargetedMonsterComponent = currentTargetedMonsterGameObject.GetComponent<CreateMonster>();

            // Trigger any pre-attack effects the current selected MonsterAttack may have (This is now called in 
            //await TriggerPreAttackEffects();

            // Grab the targeted monster's transform for status effect popups
            cachedTransform = currentTargetedMonsterGameObject.transform.position;
            cachedTransform.y += 1;

            // Hide the targeter
            currentTargetedMonsterComponent.monsterTargeterUIGameObject.SetActive(false);

            // Check if the attack hits or misses
            if (!CheckAttackHit(false))
            {
                AttackMissed(true);
                ClearCachedModifiers();
                await Task.Delay(500);
                continue;
            }

            // We didn't miss, continue function calls
            SpawnAttackEffect();

            // Calculate damage dealt and deal damage to targeted monster
            float calculatedDamage = CalculatedDamage(combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference, currentMonsterAttack);
            currentTargetedMonster.health -= calculatedDamage;

            // Cache damage dealt and taken for both target and user
            currentTargetedMonsterComponent.monsterDamageTakenThisRound += calculatedDamage;
            monsterWhoUsedAttack.cachedDamageDone += calculatedDamage;

            // If the attack was a damaging attack and the target wasn't immune, show damage done
            if (currentMonsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Attack && calculatedDamage != 0)
            {
                // Show the damage number and damaged animation of target
                currentTargetedMonsterGameObject.GetComponent<Animator>().SetBool("hitAnimationPlaying", true);
                currentTargetedMonsterComponent.ShowDamageOrStatusEffectPopup(calculatedDamage, "Damage");

                // Play damage sound
                soundEffectManager.AddSoundEffectToQueue(HitSound);
                soundEffectManager.BeginSoundEffectQueue();

                // Trigger any on damage ability effects if necesary
                await TriggerAbilityEffects(currentMonsterTurn, AttackEffect.EffectTime.OnDamageDealt, currentMonsterTurnGameObject);

                // Trigger any damage taken effects on self or target
                await TriggerAbilityEffects(currentTargetedMonster, AttackEffect.EffectTime.OnDamageTaken, currentTargetedMonsterGameObject);
            }

            // Finally, update the stats of the targeted monster, aka CheckHealth()
            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().UpdateStats(true, currentMonsterTurnGameObject, false);

            // Check if the damage killed the targeted monster
            if (CheckIfDamagedKilledMonster(monsterWhoUsedAttack, monsterWhoUsedAttackGameObject))
            {
                // Damage killed monster
                // Break out before continuing further in case self died from OnDamageTaken or OnDeath Abilities, or some other external factor
                if (currentMonsterTurn == null || currentMonsterTurn.health <= 0)
                {
                    // Self is dead, continue combat loop
                    Debug.Log("Self is dead, continue combat loop!", this);
                    combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
                    return;
                }

                totalExpGained += CalculateExp();

                // Self is not dead, continue function calls
                await TriggerAbilityEffects(currentMonsterTurn, AttackEffect.EffectTime.OnKill, currentMonsterTurnGameObject);
            }
            else
            {
                // Break out before continuing further in case self died from OnDamageTaken or OnDeath Abilities, or some other external factor
                if (currentMonsterTurn == null || currentMonsterTurn.health <= 0)
                {
                    // Self is dead, continue combat loop
                    Debug.Log("Self is dead, continue combat loop!", this);
                    combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
                    return;
                }

                // Damage did not kill monster, continue function calls
                // If the damage was not zero or the current attack was a status move
                if (calculatedDamage != 0 || currentMonsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Status)
                {
                    // Trigger any post attack effects on self or target (Null checks are handled in this function)
                    await TriggerDuringAttackEffects(monsterWhoUsedAttack, monsterWhoUsedAttackGameObject);
                }
            }

            await Task.Delay(1000);
        }

        // Trigger any post attack effects **(should only be self) (Null checks are handled in this function)
        await TriggerPostAttackEffects(monsterWhoUsedAttack, monsterWhoUsedAttackGameObject);

        // Trigger any post attack ability effects **(should only be self) (Null checks are handled in this function)
        await TriggerAbilityEffects(monsterWhoUsedAttack, AttackEffect.EffectTime.PostAttack, monsterWhoUsedAttackGameObject);

        // Clear Target List
        ListOfCurrentlyTargetedMonsters.Clear();

        // Finally, grant all exp earned
        if (currentMonsterTurnGameObject.GetComponent<CreateMonster>().GrantExpAndCheckLevelup(totalExpGained))
        {
            // Leveled up, do not trigger post ability effects, post attack effects, and next monster turn until after the level up window is closed
            //
            return;
        }
        else
        {
            // Did not level up, continue function calls
            combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
        }
    }

    // This function triggers any monster's ability effects based on what effect time is passed in
    public async Task<int> TriggerAbilityEffects(Monster monster, AttackEffect.EffectTime abilityEffectTime, GameObject monsterGameObject)
    {
        foreach (AttackEffect abilityEffect in monster.monsterAbility.listOfAbilityEffects)
        {
            if (abilityEffect.effectTime == abilityEffectTime)
            {
                abilityEffect.TriggerEffects(this, monster.monsterAbility.abilityName, currentMonsterAttack);
                await Task.Delay(300);
            }
        }

        return 1;
    }

    // This function uses the current move to deal damage to the target (should be called after attack animation ends)
    /*
    public IEnumerator DealDamage()
    {
        //// First check if the attack should deal damage to more than one target
        //if (currentMonsterAttack.monsterAttackTargetCount != MonsterAttack.MonsterAttackTargetCount.SingleTarget)
        //{
        //    //StartCoroutine(DealDamageAll());
        //    return;
        //}

        // Cache monster who used attack information
        Monster monsterWhoUsedAttack = currentMonsterTurn;
        GameObject monsterWhoUsedAttackGameObject = currentMonsterTurnGameObject;

        // Check any pre-attack effects before attacking
        TriggerPreAttackEffects();

        // Iterate through the entire list of targeted monsters
        foreach (GameObject targetMonsterGameObject in ListOfCurrentlyTargetedMonsters)
        {
            // Update the current targeted monster and gameObject if necessary
            currentTargetedMonster = targetMonsterGameObject.GetComponent<CreateMonster>().monsterReference;
            currentTargetedMonsterGameObject = targetMonsterGameObject;

            // Make sure the targeter is at the right position
            if (currentTargetedMonsterGameObject != null)
            {
                cachedTransform = currentTargetedMonsterGameObject.transform.position;
                cachedTransform.y += 1;
            }

            // Check if the attack misses
            if (!CheckAttackHit(false))
            {
                // If there is only one targeted monster, call the next turn. Otherwise, wait for each other targeted monster to check hit or miss
                if (ListOfCurrentlyTargetedMonsters.Count == 1)
                {
                    AttackMissed();
                    ClearCachedModifiers();
                    combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
                }
                else
                {
                    AttackMissed(true);
                }

                yield return new WaitForSeconds(1f);
            }

            // Play attack animations and spawn VFX
            SpawnAttackEffect();

            // Calculate damage dealt and deal damage to targeted monster
            float calculatedDamage = CalculatedDamage(combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference, currentMonsterAttack);
            currentTargetedMonster.health += calculatedDamage;
            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterDamageTakenThisRound += Mathf.Abs(calculatedDamage);

            // If the attack was a damaging attack and wasn't immune, show damage done
            if (currentMonsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Attack && calculatedDamage != 0)
            {
                currentTargetedMonsterGameObject.GetComponent<Animator>().SetBool("hitAnimationPlaying", true);
                currentTargetedMonsterGameObject.GetComponent<CreateMonster>().ShowDamageOrStatusEffectPopup(calculatedDamage);

                // Play damage sound
                soundEffectManager.AddSoundEffectToQueue(HitSound);
                soundEffectManager.BeginSoundEffectQueue();
            }

            // Cache monster who used attack health and track damage dealt
            monsterWhoUsedAttack.health = currentMonsterTurn.health;
            monsterWhoUsedAttack.cachedDamageDone += calculatedDamage;

            //currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().UpdateStats(true, null, false);

            // Remove targeter from above targeted monster's position
            combatManagerScript.monsterTargeter.SetActive(false);
            combatManagerScript.targeting = false;

            // Trigger any during attack effects if the attack is a multi-target attack
            StartCoroutine(TriggerDuringAttackEffects(monsterWhoUsedAttack));
        }

        // Clear list of targets after attack is finished
        ListOfCurrentlyTargetedMonsters.Clear();

        // Trigger any post attack effects only if calculated damage is not 0 (immune)
        if (calculatedDamage != 0 || !currentTargetedMonsterGameObject.GetComponent<CreateMonster>().listOfElementImmunities.Contains(currentMonsterAttack.monsterElementClass))
        {
            StartCoroutine(TriggerPostAttackEffects(monsterWhoUsedAttack, monsterWhoUsedAttackGameObject));
        }
        else
        {
            combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
        }
    }
    */

    //// This function is called if the current move should deal damage to all targets
    //public IEnumerator DealDamageAll()
    //{
    //    // Check pre attack effects, buffs etc.
    //    PreAttackEffectCheck();
    //    Monster monsterWhoUsedAttack = currentMonsterTurn;

    //    foreach (GameObject monsterObj in ListOfTargetedMonsters.ToArray())
    //    {
    //        // Get Monster Reference
    //        Monster monster = monsterObj.GetComponent<CreateMonster>().monsterReference;

    //        // Set target references
    //        currentTargetedMonster = monster;
    //        currentTargetedMonsterGameObject = monsterObj;

    //        // Set cached transform for popups
    //        cachedTransform = currentTargetedMonsterGameObject.transform.position;
    //        cachedTransform.y += 1;

    //        // Check if attack missed
    //        if (!CheckAttackHit(false))
    //        {
    //            // Don't advance turn
    //            AttackMissed(true);

    //            yield return new WaitForSeconds(1f);
    //            continue;
    //        }
    //        else
    //        {
    //            // Get damage calc and deal damage
    //            float calculatedDamage = CalculatedDamage(combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference, currentMonsterAttack);
    //            currentTargetedMonster.health -= calculatedDamage;
    //            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterDamageTakenThisRound += calculatedDamage;

    //            // Visual and Sound cues
    //            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().ShowDamageOrStatusEffectPopup(calculatedDamage);
    //            currentTargetedMonsterGameObject.GetComponent<Animator>().SetBool("hitAnimationPlaying", true);
    //            soundEffectManager.AddSoundEffectToQueue(HitSound);
    //            soundEffectManager.BeginSoundEffectQueue();
    //            SpawnAttackEffect();

    //            GameObject monsterWhoUsedAttackGameObject = currentMonsterTurnGameObject;
    //            monsterWhoUsedAttack = currentMonsterTurn;
    //            monsterWhoUsedAttack.health = currentMonsterTurn.health;
    //            monsterWhoUsedAttack.cachedDamageDone += calculatedDamage;

    //            // End of turn stuff
    //            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().UpdateStats(true, null, false);

    //            //// Trigger DURING attack effects only if damage dealt was not 0 (Immune)
    //            //if (calculatedDamage > 0 && currentTargetedMonster.health > 0)
    //            //{
    //            //    StartCoroutine(TriggerDuringAttackEffects(monsterWhoUsedAttack));
    //            //}

    //            // Check if damage dealt killed current targeted monster
    //            CheckIfDamagedKilledMonster(monsterWhoUsedAttack, monsterWhoUsedAttackGameObject);

    //            combatManagerScript.monsterTargeter.SetActive(false);
    //            combatManagerScript.targeting = false;

    //            // Trigger DURING attack effects only if damage dealt was not 0 (Immune)
    //            if (calculatedDamage > 0 && currentTargetedMonster.health > 0)
    //            {
    //                yield return StartCoroutine(TriggerDuringAttackEffects(monsterWhoUsedAttack));
    //            }
    //            else
    //            {
    //                yield return new WaitForSeconds(1f);
    //            }
    //        }
    //    }

    //    // Trigger post attack effects
    //    monsterWhoUsedAttack = currentMonsterTurn;
    //    StartCoroutine(TriggerPostAttackEffects(monsterWhoUsedAttack));

    //    // Out of loop, next turn
    //    ClearCachedModifiers();
    //    //combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
    //}

    // This function checks if the damage dealt killed the monster

    public bool CheckIfDamagedKilledMonster(Monster monsterWhoUsedAttack, GameObject monsterWhoUsedAttackGameObject)
    {
        // Add to killcount if applicable
        if (monsterWhoUsedAttack.aiType == Monster.AIType.Ally && monsterWhoUsedAttack != currentTargetedMonster && (combatManagerScript.adventureMode || combatManagerScript.testAdventureMode))
        {
            if (currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterReference.health <= 0)
            {
                monsterWhoUsedAttack.monsterKills += 1;

                Debug.Log("Damage killed monster. Call GrantExp and CheckLevelUp!", this);

                return true;
            }
        }

        Debug.Log("Damage did not kill monster. Call next monster turn", this);
        return false;
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
        float rowAccuracyBonus = 0f;

        // Check row accuracy bonus
        if (currentMonsterTurnGameObject.GetComponent<CreateMonster>().monsterRowPosition == CreateMonster.MonsterRowPosition.BackRow)
        {
            rowAccuracyBonus = 5f;
        }
        else if (currentMonsterTurnGameObject.GetComponent<CreateMonster>().monsterRowPosition == CreateMonster.MonsterRowPosition.FrontRow)
        {
            rowAccuracyBonus = -5f;
        }

        // don't use evasion as a stat when self-targeting or ally targeting
        if (!selfTargeting || currentMonsterTurn.aiType != currentTargetedMonster.aiType)
        {
            hitChance = (currentMonsterAttack.monsterAttackAccuracy + currentMonsterTurn.bonusAccuracy + rowAccuracyBonus - currentTargetedMonster.evasion) / 100;
            //Debug.Log($"Attack: {currentMonsterAttack.monsterAttackName}, Hit chance: {hitChance}% & not targeting self or ally!");
        }
        else
        {
            hitChance = (currentMonsterAttack.monsterAttackAccuracy + currentMonsterTurn.bonusAccuracy + rowAccuracyBonus / 100);
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
    public static MonsterAttack.MonsterAttackDamageType ReturnMonsterHighestAttackStat(Monster currentMonster)
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
        // First check if monster immune to damage
        if (currentMonsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Attack && currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterImmuneToDamage)
        {
            CombatLog.SendMessageToCombatLog($"{currentTargetedMonster.aiType} {currentTargetedMonster.name} is immune to damage!");
            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Immune!");
            return 0;
        }

        // Second, check specific monster element immunities
        if (currentTargetedMonsterGameObject.GetComponent<CreateMonster>().listOfElementImmunities.Contains(currentMonsterAttack.monsterElementClass))
        {
            CombatLog.SendMessageToCombatLog($"{currentTargetedMonster.aiType} {currentTargetedMonster.name} is immune to {currentMonsterAttack.monsterElementClass.element.ToString()} attacks!");
            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Immune!");
            return 0;
        }

        // Reset cached damage modifiers
        calculatedDamage = 0;
        bonusDamagePercent = 0;
        MonsterAttack.MonsterAttackDamageType highestStatType;

        if (recievedDamagePercentBonus)
        {
            bonusDamagePercent = cachedBonusDamagePercent;
            recievedDamagePercentBonus = false;
        }

        // Only calculate damage if the current attack is an attack not a buff / debuff
        if (currentMonsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Attack)
        {
            // Calculate resistances
            switch (monsterAttack.monsterAttackDamageType)
            {
                case (MonsterAttack.MonsterAttackDamageType.Magical):
                    //calculatedDamage = Mathf.RoundToInt(currentMonster.magicAttack + (currentMonster.magicAttack * Mathf.RoundToInt(bonusDamagePercent + currentMonsterAttack.monsterAttackDamage * .1f)) * (1 / (currentTargetedMonster.magicDefense + 1)));
                    calculatedDamage = Mathf.RoundToInt((95 * currentMonster.magicAttack * Mathf.RoundToInt(bonusDamagePercent + currentMonsterAttack.monsterAttackDamage * .1f)) / (145 + currentTargetedMonster.magicDefense));
                    break;

                case (MonsterAttack.MonsterAttackDamageType.Physical):
                    //calculatedDamage = Mathf.RoundToInt(currentMonster.physicalAttack + (currentMonster.physicalAttack * Mathf.RoundToInt(bonusDamagePercent + currentMonsterAttack.monsterAttackDamage * .1f)) * (1 / (currentTargetedMonster.physicalDefense + 1)));
                    calculatedDamage = Mathf.RoundToInt((95 * currentMonster.physicalAttack * Mathf.RoundToInt(bonusDamagePercent + currentMonsterAttack.monsterAttackDamage * .1f)) / (145 + currentTargetedMonster.physicalDefense));
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
                        calculatedDamage = Mathf.RoundToInt((95 * currentMonster.magicAttack * Mathf.RoundToInt(bonusDamagePercent + currentMonsterAttack.monsterAttackDamage * .1f)) / (145 + currentTargetedMonster.magicDefense));
                    }
                    else
                    {
                        calculatedDamage = Mathf.RoundToInt((95 * currentMonster.physicalAttack * Mathf.RoundToInt(bonusDamagePercent + currentMonsterAttack.monsterAttackDamage * .1f)) / (145 + currentTargetedMonster.physicalDefense));
                    }
                    break;

                default:
                    Debug.Log("Missing attack or attack type reference?", this);
                    break;
            }

            // Clear cached damage modifiers
            bonusDamagePercent = 0;
            cachedBonusDamagePercent = 0;

            // Check for additional flat bonus damage
            calculatedDamage += monsterAttack.monsterAttackFlatDamageBonus;

            // Check monster main element bonus/ /  fix ToStrings?
            if (monsterAttack.monsterAttackElement == currentMonster.monsterElement.element)
            {
                calculatedDamage += Mathf.RoundToInt(calculatedDamage * .15f);
                //Debug.Log("Main element bonus!");
            }

            // Check sub element bonus //  fix ToStrings?
            if (monsterAttack.monsterAttackElement == currentMonster.monsterSubElement.element)
            {
                calculatedDamage += Mathf.RoundToInt(calculatedDamage * .5f);
                //Debug.Log("Sub element bonus!");
            }

            // Check elemental weaknesses and resistances and apply bonus damage & Reset elemental damage bonus 
            elementCheckDamageBonus = CheckElementWeakness(currentMonsterAttack.monsterAttackElement, currentTargetedMonster, true);
            calculatedDamage = Mathf.RoundToInt(calculatedDamage * elementCheckDamageBonus);

            // Check for critical hit
            if (CheckAttackCrit())
            {
                calculatedDamage = Mathf.RoundToInt(calculatedDamage * currentMonster.critDamage);
                CombatLog.SendMessageToCombatLog($"Critical Hit!");

                soundEffectManager.AddSoundEffectToQueue(CritSound);
                Instantiate(monsterCritText, cachedTransform, Quaternion.identity);

                currentMonsterTurnGameObject.GetComponent<CreateMonster>().monsterCriticallyStrikedThisRound = true;
            }

            // Apply damage percent modifiers (row bonuses and etc.)
            // Check Current Monster current row position
            if (currentMonsterTurnGameObject.GetComponent<CreateMonster>().monsterRowPosition == CreateMonster.MonsterRowPosition.BackRow)
            {
                Debug.Log($"Damage before Current Monster Row Bonus: {calculatedDamage}");
                calculatedDamage = Mathf.RoundToInt(calculatedDamage * backRowDamagePercentBonus);
                Debug.Log($"Damage after Current Monster Row Bonus: {calculatedDamage}");
            }
            else
            if (currentMonsterTurnGameObject.GetComponent<CreateMonster>().monsterRowPosition == CreateMonster.MonsterRowPosition.FrontRow)
            {
                Debug.Log($"Damage before Current Monster Row Bonus: {calculatedDamage}");
                calculatedDamage = Mathf.RoundToInt(calculatedDamage * frontRowDamagePercentBonus);
                Debug.Log($"Damage after Current Monster Row Bonus: {calculatedDamage}");
            }

            // Check Target Monster current row position
            if (currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterRowPosition == CreateMonster.MonsterRowPosition.BackRow)
            {
                Debug.Log($"Damage before Target Monster Row Bonus: {calculatedDamage}");
                calculatedDamage = Mathf.RoundToInt(calculatedDamage * backRowDamagePercentBonus);
                Debug.Log($"Damage after Target Monster Row Bonus: {calculatedDamage}");
            }
            else
            if (currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterRowPosition == CreateMonster.MonsterRowPosition.FrontRow)
            {
                Debug.Log($"Damage before Target Monster Row Bonus: {calculatedDamage}");
                calculatedDamage = Mathf.RoundToInt(calculatedDamage * frontRowDamagePercentBonus);
                Debug.Log($"Damage after Target Monster Row Bonus: {calculatedDamage}");
            }

            // Final damage check
            if (calculatedDamage <= 0 && currentMonsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Attack)
            {
                Debug.Log("Called 1 damage check!", this);
                calculatedDamage = 1f;
            }

            // Display combat message
            // Targeting self?
            if (currentTargetedMonster == currentMonsterTurn)
            {
                CombatLog.SendMessageToCombatLog($"{currentMonster.aiType} {currentMonster.name} used {currentMonsterAttack.monsterAttackName} " +
                    $"on itself for {calculatedDamage} damage!", currentMonsterTurn.aiType);
            }
            else
            {
                CombatLog.SendMessageToCombatLog($"{currentMonster.aiType} {currentMonster.name} used {currentMonsterAttack.monsterAttackName} " +
                    $"on {currentTargetedMonster.aiType} {currentTargetedMonster.name} for {calculatedDamage} damage!", currentMonsterTurn.aiType);
            }
        }
        else
        {
            // Not an attack, don't show damage dealt.
            // Targeting self?
            if (currentTargetedMonster == currentMonsterTurn)
            {
                CombatLog.SendMessageToCombatLog($"{currentMonster.aiType} {currentMonster.name} used {currentMonsterAttack.monsterAttackName} " +
                    $"on itself!", currentMonsterTurn.aiType);
            }
            else
            {
                CombatLog.SendMessageToCombatLog($"{currentMonster.aiType} {currentMonster.name} used {currentMonsterAttack.monsterAttackName} " +
                $"on {currentTargetedMonster.aiType} {currentTargetedMonster.name}!", currentMonsterTurn.aiType);
            }
        }

        // Finally, return calculated damage
        cachedDamage = calculatedDamage;
        Debug.Log($"Calced Damage: {calculatedDamage}");
        return calculatedDamage;
    }

    // This function checks elemental weaknesses and resistances and returns a damage multipler
    public float CheckElementWeakness(ElementClass.MonsterElement attackElement, Monster targetedMonster, bool sendMessageToLog)
    {
        // Both main element and sub element are weak
        if (targetedMonster.monsterElement.listOfWeaknesses.Contains(attackElement) && targetedMonster.monsterSubElement.listOfWeaknesses.Contains(attackElement))
        {
            //Debug.Log("Two weaknesses!");
            if (sendMessageToLog)
            {
                CombatLog.SendMessageToCombatLog($"It was incredibly effective!");
                soundEffectManager.AddSoundEffectToQueue(CritSound);
                currentTargetedMonsterGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Super Weak!");
            }
            return 2f;
        }

        // One element is weak
        // First check if no resistances. Either one or the other is weak to attack element and neither are resistance
        if (!targetedMonster.monsterElement.listOfResistances.Contains(attackElement) && !targetedMonster.monsterSubElement.listOfResistances.Contains(attackElement))
        {
            if (targetedMonster.monsterElement.listOfWeaknesses.Contains(attackElement) && !targetedMonster.monsterSubElement.listOfWeaknesses.Contains(attackElement)
                || !targetedMonster.monsterElement.listOfWeaknesses.Contains(attackElement) && targetedMonster.monsterSubElement.listOfWeaknesses.Contains(attackElement))
            {
                //Debug.Log("One weakness!");
                if (sendMessageToLog)
                {
                    CombatLog.SendMessageToCombatLog($"It was highly effective!");
                    soundEffectManager.AddSoundEffectToQueue(CritSound);
                    currentTargetedMonsterGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Weak!");
                }
                return 1.5f;
            }
        }

        // Weakness/Resistance cancel out
        // First check if one is resistant. Either main or sub element is resistant, and the other is weak, they cancel it
        if (targetedMonster.monsterElement.listOfResistances.Contains(attackElement) || targetedMonster.monsterSubElement.listOfResistances.Contains(attackElement))
        {
            if (targetedMonster.monsterElement.listOfWeaknesses.Contains(attackElement) && targetedMonster.monsterSubElement.listOfResistances.Contains(attackElement)
                || targetedMonster.monsterElement.listOfResistances.Contains(attackElement) && targetedMonster.monsterSubElement.listOfWeaknesses.Contains(attackElement))
            {
                //Debug.Log("Canceled out!");
                return 1f;
            }
        }

        // Both elements resist (check this first)
        // First check if one is resistant. Either main or sub element is resistant, and the other is not weak or resistant
        if (targetedMonster.monsterElement.listOfResistances.Contains(attackElement) && targetedMonster.monsterSubElement.listOfResistances.Contains(attackElement))
        {
            //Debug.Log("Two Resistances!");
            if (sendMessageToLog)
            {
                CombatLog.SendMessageToCombatLog($"It was highly resisted!");
                soundEffectManager.AddSoundEffectToQueue(ResistSound);
                currentTargetedMonsterGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Super Resist!");
            }
            return 0.25f;
        }

        // One element resists
        // First check if one is resistant. Either main or sub element is resistant, and the other is not weak or resistant
        if (targetedMonster.monsterElement.listOfResistances.Contains(attackElement) || targetedMonster.monsterSubElement.listOfResistances.Contains(attackElement))
        {
            if (!targetedMonster.monsterElement.listOfWeaknesses.Contains(attackElement) && !targetedMonster.monsterSubElement.listOfWeaknesses.Contains(attackElement))
            {
                //Debug.Log("One Resistance!");
                if (sendMessageToLog)
                {
                    CombatLog.SendMessageToCombatLog($"It was resisted!");
                    soundEffectManager.AddSoundEffectToQueue(ResistSound);
                    currentTargetedMonsterGameObject.GetComponent<CreateMonster>().CreateStatusEffectPopup("Resist!");
                }
                return 0.5f;
            }
        }

        // default multiplier
        //Debug.Log("Neutral!");
        return 1f;
    }

    // This function clears cached damage bonuses etc. to prevent unwanted buffs
    public void ClearCachedModifiers()
    {
        bonusDamagePercent = 0;
        cachedBonusDamagePercent = 0;
        recievedDamagePercentBonus = false;
    }

    // This function calculates exp to grant to monster
    public int CalculateExp()
    {
        return (11  * currentTargetedMonster.level);
    }

    #region Old Code
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
    #endregion
}
