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

    // This function selects a monster attack for the enemy AI
    public void SelectMove()
    {
        enemyListOfMonsterAttacks = currentEnemyTurn.ListOfMonsterAttacks;

        switch (currentEnemyTurn.aiLevel)
        {
            case Monster.AILevel.Random:
                currentEnemyMonsterAttack = GetRandomMove();
                currentEnemyTargetGameObject = GetRandomTarget();

                currentEnemyTarget = currentEnemyTargetGameObject.GetComponent<CreateMonster>().monsterReference;

                monsterTargeter.SetActive(true);
                monsterTargeter.transform.position = new Vector3(currentEnemyTargetGameObject.transform.position.x, currentEnemyTargetGameObject.transform.position.y + 2.5f, currentEnemyTargetGameObject.transform.position.z);

                uiManager.EditCombatMessage($"Enemy {currentEnemyTurn.name} will use {currentEnemyMonsterAttack.monsterAttackName} on {currentEnemyTarget.aiType} {currentEnemyTarget.name}!");

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
        Debug.Log($"{randMove.monsterAttackName}");
        return randMove;
    }

    // This function returns a random target from the list of ally monsters // GitHub edit
    public GameObject GetRandomTarget()
    {
        GameObject randTarget = listOfAllies[Random.Range(0, listOfAllies.Count)];
        Debug.Log($"{randTarget.GetComponent<CreateMonster>().monsterReference.name}");
        return randTarget;
    }
}
