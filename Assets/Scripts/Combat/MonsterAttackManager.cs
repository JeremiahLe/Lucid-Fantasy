using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterAttackManager : MonoBehaviour
{
    public MonsterAttack currentMonsterAttack;
    public Monster currentMonsterTurn;
    public GameObject currentMonsterTurnGameObject;

    public GameObject currentTargetedMonsterGameObject;
    public Monster currentTargetedMonster;

    public ButtonManagerScript buttonManagerScript;
    public CombatManagerScript combatManagerScript;
    public HUDAnimationManager HUDanimationManager;
    public EnemyAIManager enemyAIManager;
    public UIManager uiManager;
    public MessageManager CombatLog;

    public TextMeshProUGUI currentMonsterAttackDescription;
    public Image TextBackImage;
    public Image TextBackImageBorder; // temporary

    public GameObject monsterAttackMissText;
    public GameObject monsterCritText;

    public Vector3 cachedTransform;
    public float cachedDamage;

    public AudioClip CritSound;
    public AudioClip HitSound;
    public AudioClip MissSound; // TODO - move sound calls to a Sound Manager

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
        HUDanimationManager.MonsterCurrentTurnText.text = ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name} " +
            $"will use {ReturnCurrentButtonAttack().monsterAttackName} " +
            $"on {combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.aiType} " +
            $"{combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.name}?");

        buttonManagerScript.HideAllButtons("AttacksHUDButtons");
        buttonManagerScript.ShowButton("ConfirmButton");

        currentMonsterAttackDescription.gameObject.SetActive(true);
        currentMonsterAttackDescription.text = ($"{currentMonsterAttack.monsterAttackName}" +
            $"\n{currentMonsterAttack.monsterAttackDescription}" +
            $"\nBase Power: {currentMonsterAttack.monsterAttackDamage} ({currentMonsterAttack.monsterAttackType.ToString()}) | Accuracy: {currentMonsterAttack.monsterAttackAccuracy}%" +
            $"\nElement: {currentMonsterAttack.monsterAttackElement.ToString()}");
        TextBackImage.enabled = true;
        TextBackImageBorder.enabled = true;
    }

    // This function updates the targeted enemy text on screen
    public void UpdateCurrentTargetText()
    {
        if (currentMonsterTurnGameObject != null)
        {
            // Is the monster targeting itself?
            if (combatManagerScript.CurrentMonsterTurn == combatManagerScript.CurrentTargetedMonster)
            {
                HUDanimationManager.MonsterCurrentTurnText.text = ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name} " +
                    $"will use {ReturnCurrentButtonAttack().monsterAttackName} on self?");
            }
            else
            {
                HUDanimationManager.MonsterCurrentTurnText.text = ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name} " +
                    $"will use {ReturnCurrentButtonAttack().monsterAttackName} " +
                    $"on {combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.aiType} " +
                    $"{combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.name}?");
            }
        }
    }

    // This function assigns the monster attack that is connected to the pressed button
    public MonsterAttack ReturnCurrentButtonAttack()
    {
        return currentMonsterAttack;
    }

    // This function serves as a reset to visual elements on screen (monster attack description)
    public void ResetHUD()
    {
        currentMonsterAttackDescription.text = "";
        TextBackImage.enabled = false;
        TextBackImageBorder.enabled = false;

        currentMonsterAttackDescription.gameObject.SetActive(false);
        combatManagerScript.monsterTargeter.SetActive(false);
    }

    // This function uses the selected monster attack on the selected monster
    public void UseMonsterAttack()
    {
        currentMonsterTurnGameObject = combatManagerScript.CurrentMonsterTurn;
        currentMonsterTurn = combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference; // unrelated to UI popup (missing reassignment calls?)

        // take away action
        currentMonsterTurnGameObject.GetComponent<CreateMonster>().monsterActionAvailable = false;
        // does attack have a cooldown? if so, activate it
        if (currentMonsterAttack.attackHasCooldown)
        {
            currentMonsterAttack.attackOnCooldown = true;
        }

        combatManagerScript.CurrentMonsterTurnAnimator.SetBool("attackAnimationPlaying", true);
        AudioClip monsterAttackSound = currentMonsterAttack.monsterAttackSoundEffect;
        combatManagerScript.GetComponent<AudioSource>().PlayOneShot(monsterAttackSound);

        buttonManagerScript.HideAllButtons("All");
        ResetHUD();
        uiManager.EditCombatMessage();
    }

    // This function uses the current move to deal damage to the target (should be called after attack animation ends)
    public void DealDamage()
    {
        // Pre Attack Effects Go Here
        
        //
        
        currentTargetedMonster = combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
        currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;

        if (currentTargetedMonsterGameObject != null)
        {
            cachedTransform = currentTargetedMonsterGameObject.transform.position;
            cachedTransform.y += 1;
        }

        if (CheckAttackHit())
        {
            float calculatedDamage = CalculatedDamage(combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference, currentMonsterAttack);
            currentTargetedMonster.health -= calculatedDamage;
            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterDamageTakenThisRound += calculatedDamage;

            currentTargetedMonsterGameObject.GetComponent<Animator>().SetBool("hitAnimationPlaying", true);
            combatManagerScript.GetComponent<AudioSource>().PlayOneShot(HitSound);

            Monster monsterWhoUsedAttack = currentMonsterTurn;
            monsterWhoUsedAttack.health = currentMonsterTurn.health;

            // Trigger all attack after effects (buffs, debuffs etc.) - TODO - Implement other buffs/debuffs and durations
            if (currentMonsterTurnGameObject != null && monsterWhoUsedAttack.health > 0)
            {
                foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
                {
                    effect.TriggerEffects(this);
                    //Debug.Log("Called attack effect!");
                }
            }
            
            //currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().UpdateStats();
            combatManagerScript.monsterTargeter.SetActive(false);
            combatManagerScript.targeting = false;

            combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
        }
        else
        {
            CombatLog.SendMessageToCombatLog
                ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.aiType} " +
                $"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name}'s " +
                $"{currentMonsterAttack.monsterAttackName} missed {currentTargetedMonster.aiType} {currentTargetedMonster.name}!");

            combatManagerScript.GetComponent<AudioSource>().PlayOneShot(MissSound);
            monsterAttackMissText.SetActive(true);
            monsterAttackMissText.transform.position = cachedTransform;
            combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
        }
    }

    // This function checks accuracy and evasion
    public bool CheckAttackHit()
    {
        float hitChance = (currentMonsterAttack.monsterAttackAccuracy - currentTargetedMonster.evasion) / 100;
        float randValue = Random.value;

        if (randValue < hitChance)
        {
            return true;
        }

        return false;
    }

    // This function checks crit chance
    public bool CheckAttackCrit()
    {
        float critChance = currentMonsterAttack.monsterAttackCritChance / 100;
        float randValue = Random.value;

        if (randValue < critChance)
        {
            return true;
        }

        return false;
    }

    // This function returns a damage number based on attack, defense + other calcs
    public float CalculatedDamage(Monster currentMonster, MonsterAttack monsterAttack)
    {
        float calculatedDamage = 0;
        float matchingElementBoost = 0;

        /*
        // Check if targeting self?
        if (currentMonster == currentTargetedMonster)
        {
            calculatedDamage = 0;

            // Send log message
            CombatLog.SendMessageToCombatLog($"{currentMonster.aiType} {currentMonster.name} deal 0 damage to itself..." +
                $"on {currentTargetedMonster.aiType} {currentTargetedMonster.name} for {calculatedDamage} damage!");

            cachedDamage = calculatedDamage;
            return calculatedDamage;
        }
        */

        // First check if attack element matches monster's element. If so, boost base damage by 5%
        if (monsterAttack.monsterAttackElement == currentMonster.monsterType)
        {
            matchingElementBoost = (currentMonsterAttack.monsterAttackDamage * .05f);
        }

        // Calculate resistances
        if (monsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Magical)
        {
            calculatedDamage = currentMonster.magicAttack + (currentMonster.magicAttack * Mathf.RoundToInt(matchingElementBoost + currentMonsterAttack.monsterAttackDamage * .1f)) - currentTargetedMonster.magicDefense;
        }
        else if (monsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Physical)
        {
            calculatedDamage = currentMonster.physicalAttack + (currentMonster.physicalAttack * Mathf.RoundToInt(matchingElementBoost + currentMonsterAttack.monsterAttackDamage * .1f)) - currentTargetedMonster.physicalDefense;
        }

        // Check if resistances are TOO high or base damage TOO low; if so, make the base damage at minimum 1
        if (calculatedDamage <= 0)
        {
            calculatedDamage = 1f;
        }

        // Now check for critical hit
        if (CheckAttackCrit())
        {
            calculatedDamage *= 2f;
            CombatLog.SendMessageToCombatLog($"Critical Hit!!! {currentMonster.aiType} {currentMonster.name} used {currentMonsterAttack.monsterAttackName} " +
                $"on {currentTargetedMonster.aiType} {currentTargetedMonster.name} for {calculatedDamage} damage!");

            monsterCritText.SetActive(true);
            monsterCritText.transform.position = cachedTransform;

            cachedDamage = calculatedDamage;
            combatManagerScript.GetComponent<AudioSource>().PlayOneShot(CritSound);
            return calculatedDamage;
        }

        // Send log message
        CombatLog.SendMessageToCombatLog($"{currentMonster.aiType} {currentMonster.name} used {currentMonsterAttack.monsterAttackName} " +
            $"on {currentTargetedMonster.aiType} {currentTargetedMonster.name} for {calculatedDamage} damage!");

        cachedDamage = calculatedDamage;
        return calculatedDamage;
    }
}
