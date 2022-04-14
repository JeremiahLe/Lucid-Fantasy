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
    }

    // This function assigns the monster attack that is connected to the pressed button
    public void AssignCurrentButtonAttack(string buttonNumber)
    {
        currentMonsterAttack = buttonManagerScript.ListOfMonsterAttacks[int.Parse(buttonNumber)];
        currentMonsterTurn = combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference; // unrelated to UI popup (missing reassignment calls?)
        combatManagerScript.CurrentMonsterAttack = currentMonsterAttack;

        combatManagerScript.TargetingEnemyMonsters(true);
        HUDanimationManager.MonsterCurrentTurnText.text = ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name} will use {ReturnCurrentButtonAttack().monsterAttackName} on {combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.aiType} {combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.name}?");

        buttonManagerScript.HideAllButtons("AttacksHUDButtons");
        buttonManagerScript.ShowButton("ConfirmButton");

        currentMonsterAttackDescription.gameObject.SetActive(true);
        currentMonsterAttackDescription.text = ($"{currentMonsterAttack.monsterAttackName}" +
            $"\n{currentMonsterAttack.monsterAttackDescription}" +
            $"\nBase Power: {currentMonsterAttack.monsterAttackDamage} ({currentMonsterAttack.monsterAttackType.ToString()}) | Accuracy: {currentMonsterAttack.monsterAttackAccuracy}%" +
            $"\nElement: {currentMonsterAttack.monsterAttackElement.ToString()}");
    }

    // This function updates the targeted enemy text on screen
    public void UpdateCurrentTargetText()
    {
        HUDanimationManager.MonsterCurrentTurnText.text = ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name} will use {ReturnCurrentButtonAttack().monsterAttackName} on {combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.aiType} {combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.name}?");
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
        currentMonsterAttackDescription.gameObject.SetActive(false);
        combatManagerScript.monsterTargeter.SetActive(false);
    }

    // This function uses the selected monster attack on the selected monster
    public void UseMonsterAttack()
    {
        currentMonsterTurnGameObject = combatManagerScript.CurrentMonsterTurn;
        currentMonsterTurn = combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference; // unrelated to UI popup (missing reassignment calls?)

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
        currentTargetedMonster = combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
        currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;

        if (currentTargetedMonsterGameObject != null)
        {
            cachedTransform = currentTargetedMonsterGameObject.transform.position;
            cachedTransform.y += 1;
        }

        if (CheckAttackHit())
        {
            currentTargetedMonster.health -= CalculatedDamage(combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference, currentMonsterAttack);
            currentTargetedMonsterGameObject.GetComponent<Animator>().SetBool("hitAnimationPlaying", true);
            combatManagerScript.GetComponent<AudioSource>().PlayOneShot(HitSound);

            // Trigger all attack after effects (buffs, debuffs etc.) - TODO - Implement other buffs/debuffs and durations

            foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
            {
                effect.TriggerEffects(this);
                //Debug.Log("Called attack effect!");
            }
            

            //currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().UpdateStats();
            combatManagerScript.monsterTargeter.SetActive(false);
            combatManagerScript.targeting = false;

            combatManagerScript.Invoke("NextMonsterTurn", 0.1f);
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
            combatManagerScript.Invoke("NextMonsterTurn", 0.1f);
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

        if (monsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Magical)
        {
            calculatedDamage = currentMonster.magicAttack + (currentMonster.magicAttack * Mathf.RoundToInt(currentMonsterAttack.monsterAttackDamage * .1f)) - currentTargetedMonster.magicDefense;
        }
        else if (monsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Physical)
        {
            calculatedDamage = currentMonster.physicalAttack + (currentMonster.physicalAttack * Mathf.RoundToInt(currentMonsterAttack.monsterAttackDamage * .1f)) - currentTargetedMonster.physicalDefense;
        }

        if (CheckAttackCrit())
        {
            calculatedDamage *= 2;
            CombatLog.SendMessageToCombatLog($"Critical Hit!!! {currentMonster.aiType} {currentMonster.name} used {currentMonsterAttack.monsterAttackName} on {currentTargetedMonster.aiType} {currentTargetedMonster.name} for {calculatedDamage} damage!");
            monsterCritText.SetActive(true);
            monsterCritText.transform.position = cachedTransform;
            cachedDamage = calculatedDamage;
            combatManagerScript.GetComponent<AudioSource>().PlayOneShot(CritSound);
            return calculatedDamage;
        }

        CombatLog.SendMessageToCombatLog($"{currentMonster.aiType} {currentMonster.name} used {currentMonsterAttack.monsterAttackName} on {currentTargetedMonster.aiType} {currentTargetedMonster.name} for {calculatedDamage} damage!");
        cachedDamage = calculatedDamage;
        return calculatedDamage;
    }
}
