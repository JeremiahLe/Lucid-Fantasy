using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIManager : MonoBehaviour
{
    public GameObject currentEnemyTurnGameObject;
    public Monster currentEnemyTurn;

    public GameObject currentEnemyTargetGameObject;
    public Monster currentEnemyTarget;

    public MonsterAttack currentEnemyMonsterAttack;
    public List<MonsterAttack> enemyListOfMonsterAttacks;

    public GameObject monsterTargeter;
    public List<GameObject> listOfAllies;

    public MonsterAttackManager monsterAttackManager;
    public CombatManagerScript combatManagerScript;
    public UIManager uiManager;

    public List<List<GameObject>> allListsOfMonsters;

    // Start function
    public void Start()
    {
        InitializeComponents();
    }

    // This function initializes the gameObject components
    public void InitializeComponents()
    {
        monsterAttackManager = GetComponent<MonsterAttackManager>();
        combatManagerScript = GetComponent<CombatManagerScript>();
        uiManager = GetComponent<UIManager>();
    }

    // This function initiates list of each list for Dazed Status
    public void InformLists()
    {
        allListsOfMonsters = new List<List<GameObject>>();
        allListsOfMonsters.Add(combatManagerScript.ListOfAllys);
        allListsOfMonsters.Add(combatManagerScript.ListOfEnemies);
    }

    // This function selects a monster attack for the enemy AI
    public void SelectMove()
    {
        enemyListOfMonsterAttacks = currentEnemyTurn.ListOfMonsterAttacks;

        switch (currentEnemyTurn.aiLevel)
        {
            case Monster.AILevel.Random:
                currentEnemyMonsterAttack = GetRandomMove();

                // What type of attack move was selected?
                switch (currentEnemyMonsterAttack.monsterAttackTargetType)
                {
                    // If self targeting move, return self
                    case (MonsterAttack.MonsterAttackTargetType.SelfTarget):
                        currentEnemyTargetGameObject = combatManagerScript.CurrentMonsterTurn;
                        break;

                    case (MonsterAttack.MonsterAttackTargetType.AllyTarget):
                        currentEnemyTargetGameObject = GetRandomTarget(combatManagerScript.ListOfEnemies);
                        break;

                    default:
                        if (currentEnemyTurnGameObject.GetComponent<CreateMonster>().monsterIsDazed)
                        {
                            currentEnemyTargetGameObject = GetRandomTarget(GetRandomList());
                        }
                        else
                        {
                            currentEnemyTargetGameObject = GetRandomTarget(combatManagerScript.ListOfAllys);
                        }
                        break;
                }

                //currentEnemyTargetGameObject = GetRandomTarget();

                currentEnemyTarget = currentEnemyTargetGameObject.GetComponent<CreateMonster>().monsterReference;

                monsterTargeter.SetActive(true);
                monsterTargeter.transform.position = new Vector3(currentEnemyTargetGameObject.transform.position.x, currentEnemyTargetGameObject.transform.position.y + 2.5f, currentEnemyTargetGameObject.transform.position.z);

                // Targeting self?
                if (currentEnemyTarget == currentEnemyTurn)
                {
                    // Dazed?
                    if (currentEnemyTurnGameObject.GetComponent<CreateMonster>().monsterIsDazed)
                    {
                        uiManager.EditCombatMessage($"Enemy {currentEnemyTurn.name} is Dazed and will use {currentEnemyMonsterAttack.monsterAttackName} on itself!");
                    }
                    else
                    {
                        uiManager.EditCombatMessage($"Enemy {currentEnemyTurn.name} will use {currentEnemyMonsterAttack.monsterAttackName} on itself!");
                    }
                }
                else {
                    // Dazed?
                    if (currentEnemyTurnGameObject.GetComponent<CreateMonster>().monsterIsDazed)
                    {
                        uiManager.EditCombatMessage($"Enemy {currentEnemyTurn.name} is Dazed and will use {currentEnemyMonsterAttack.monsterAttackName} on {currentEnemyTarget.aiType} {currentEnemyTarget.name}!");
                    }
                    else
                    {
                        uiManager.EditCombatMessage($"Enemy {currentEnemyTurn.name} will use {currentEnemyMonsterAttack.monsterAttackName} on {currentEnemyTarget.aiType} {currentEnemyTarget.name}!");
                    }
                }

                // Dazed?
                if (currentEnemyTurnGameObject.GetComponent<CreateMonster>().monsterIsDazed)
                {

                }
                    combatManagerScript.CurrentMonsterTurnAnimator = currentEnemyTurnGameObject.GetComponent<Animator>();
                combatManagerScript.CurrentTargetedMonster = currentEnemyTargetGameObject;
                monsterAttackManager.currentMonsterAttack = currentEnemyMonsterAttack;

                monsterAttackManager.Invoke("UseMonsterAttack", 1.7f);
                break;
            default:
                Debug.Log("Missing AI Level or monster reference?", this);
                break;
        }
    }

    // This function returns a random move from the monsters list of monster attacks
    public MonsterAttack GetRandomMove()
    {
        MonsterAttack randMove = enemyListOfMonsterAttacks[Random.Range(0, enemyListOfMonsterAttacks.Count)];
        Debug.Log($"Random move selected: {randMove.monsterAttackName}");
        while (randMove.attackOnCooldown)
        {
            randMove = enemyListOfMonsterAttacks[Random.Range(0, enemyListOfMonsterAttacks.Count)];
        }
        return randMove;
    }

    // This function returns a random target from the list of ally monsters // GitHub edit
    public GameObject GetRandomTarget(List<GameObject> whoAmITargeting)
    {
        GameObject randTarget = whoAmITargeting[Random.Range(0, whoAmITargeting.Count)];
        Debug.Log($"Random target selected: {randTarget.GetComponent<CreateMonster>().monsterReference.name}");
        return randTarget;
    }

    // This function returns a random target from the list of ally monsters // GitHub edit
    public List<GameObject> GetRandomList()
    {
        List<GameObject> randList = allListsOfMonsters[Random.Range(0, allListsOfMonsters.Count)];
        Debug.Log($"Random list selected: {randList}");
        return randList;
    }
}
