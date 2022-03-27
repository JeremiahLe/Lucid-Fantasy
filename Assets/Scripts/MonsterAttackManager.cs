using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterAttackManager : MonoBehaviour
{
    public MonsterAttack currentMonsterAttack;

    public GameObject currentTargetedMonsterGameObject;
    public Monster currentTargetedMonster;

    public ButtonManagerScript buttonManagerScript;
    public CombatManagerScript combatManagerScript;
    public HUDAnimationManager HUDanimationManager;
    public EnemyAIManager enemyAIManager;

    public TextMeshProUGUI currentMonsterAttackDescription;

    public MessageManager CombatLog;

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

        currentMonsterAttackDescription.gameObject.SetActive(false);
    }

    // This function assigns the monster attack that is connected to the pressed button
    public void AssignCurrentButtonAttack(string buttonNumber)
    {
        currentMonsterAttack = buttonManagerScript.ListOfMonsterAttacks[int.Parse(buttonNumber)];
        combatManagerScript.TargetingEnemyMonsters(true);
        HUDanimationManager.MonsterCurrentTurnText.text = ($"Windwoof will use {ReturnCurrentButtonAttack().monsterAttackName} on {combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.name}?");
        buttonManagerScript.HideAllButtons("AttacksHUDButtons");
        buttonManagerScript.ShowButton("ConfirmButton");

        currentMonsterAttackDescription.gameObject.SetActive(true);
        currentMonsterAttackDescription.text = ($"{currentMonsterAttack.monsterAttackName}" +
            $"\n{currentMonsterAttack.monsterAttackDescription}" +
            $"\nBase Power: {currentMonsterAttack.monsterAttackDamage} ({currentMonsterAttack.monsterAttackType.ToString()})" +
            $"\nElement: {currentMonsterAttack.monsterAttackElement.ToString()}");
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
    }

    // This function uses the selected monster attack on the selected monster
    public void UseMonsterAttack()
    {
        combatManagerScript.CurrentMonsterTurnAnimator.SetBool("attackAnimationPlaying", true);
        buttonManagerScript.HideAllButtons("All");
        ResetHUD();
        combatManagerScript.EditCombatMessage();
    }

    // This function uses the current move to deal damage to the target (should be called after attack animation ends)
    public void DealDamage()
    {
        currentTargetedMonster = combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
        currentTargetedMonster.health -= CalculatedDamage(combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference, currentMonsterAttack);

        currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
        currentTargetedMonsterGameObject.GetComponent<CreateMonster>().UpdateStats();
        combatManagerScript.monsterTargeter.SetActive(false);

        combatManagerScript.Invoke("NextMonsterTurn", 0.1f);
    }

    // This function returns a damage number based on attack, defense + other calcs
    public int CalculatedDamage(Monster currentMonster, MonsterAttack monsterAttack)
    {
        int calculatedDamage = 0;

        if (monsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Magical)
        {
            calculatedDamage = currentMonster.magicAttack + (currentMonster.magicAttack * Mathf.RoundToInt(currentMonsterAttack.monsterAttackDamage * .1f)) - currentTargetedMonster.magicDefense;
        }
        else if (monsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.Physical)
        {
            calculatedDamage = currentMonster.physicalAttack + (currentMonster.magicAttack * Mathf.RoundToInt(currentMonsterAttack.monsterAttackDamage * .1f)) - currentTargetedMonster.physicalDefense;
        }

        CombatLog.SendMessageToCombatLog($"{currentMonster.aiType} {currentMonster.name} used {currentMonsterAttack.monsterAttackName} on {currentTargetedMonster.aiType} {currentTargetedMonster.name} for {calculatedDamage} damage!");
        return calculatedDamage;
    }
}
