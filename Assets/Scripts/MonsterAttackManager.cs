using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterAttackManager : MonoBehaviour
{
    public MonsterAttack currentMonsterAttack;

    public ButtonManagerScript buttonManagerScript;
    public CombatManagerScript combatManagerScript;
    public HUDAnimationManager HUDanimationManager;

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
    }

    // This function assigns the monster attack that is connected to the pressed button
    public void AssignCurrentButtonAttack(string buttonNumber)
    {
        currentMonsterAttack = buttonManagerScript.ListOfMonsterAttacks[int.Parse(buttonNumber)];
        HUDanimationManager.MonsterCurrentTurnText.text = ($"Windwoof will use {ReturnCurrentButtonAttack().monsterAttackName} on what target?");
        combatManagerScript.TargetingEnemyMonsters(true);
    }

    // This function assigns the monster attack that is connected to the pressed button
    public MonsterAttack ReturnCurrentButtonAttack()
    {
        return currentMonsterAttack;
    }
}
