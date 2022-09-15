using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
                        uiManager.EditCombatMessage($"Enemy {currentEnemyTurn.name} is Dazed will use {currentEnemyMonsterAttack.monsterAttackName} on itself!");
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

                combatManagerScript.CurrentMonsterTurnAnimator = currentEnemyTurnGameObject.GetComponent<Animator>();
                combatManagerScript.CurrentTargetedMonster = currentEnemyTargetGameObject;
                monsterAttackManager.currentMonsterAttack = currentEnemyMonsterAttack;

                // Random chance to change row position
                CreateMonster.MonsterRowPosition randRowPos = RandomRowPosition();
                //Debug.Log($"{randRowPos}");
                CreateMonster monsterComponent = combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>();

                if (randRowPos != monsterComponent.monsterRowPosition)
                {
                    monsterComponent.SetPositionAndOrientation(monsterComponent.transform, monsterComponent.combatOrientation, randRowPos, monsterComponent.monsterRowPosition);
                }

                monsterAttackManager.Invoke("UseMonsterAttack", 0.2f);
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
        //Debug.Log($"Random move selected: {randMove.monsterAttackName}");
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
        float randValue = 1;
        List<GameObject> tempList;

        // If initial target is backrow, chance to target another monster. If initial target is centerrow, chance to target front row monster
        if (whoAmITargeting.Count > 1 && randTarget.GetComponent<CreateMonster>().monsterRowPosition == CreateMonster.MonsterRowPosition.BackRow)
        {
            randValue = Random.value;
            //Debug.Log($"Generated random value: {randValue}");
            if (randValue < .66f)
            {
                tempList = whoAmITargeting.Where(monster => monster != randTarget).ToList();
                randTarget = tempList[Random.Range(0, tempList.Count)];
                //Debug.Log($"Targeting another unit!");

                randValue = Random.value;
                tempList = whoAmITargeting.Where(monster => monster != randTarget && monster.GetComponent<CreateMonster>().monsterRowPosition == CreateMonster.MonsterRowPosition.FrontRow).ToList();
                if (tempList.Count != 0)
                {
                    if (randValue < .66f)
                    {
                        //Debug.Log($"Targeting FRONT ROW!");
                        randTarget = tempList[Random.Range(0, tempList.Count)];
                    }
                }

            }
        }
        else 
        if (whoAmITargeting.Count > 1 && randTarget.GetComponent<CreateMonster>().monsterRowPosition == CreateMonster.MonsterRowPosition.CenterRow)
        {
            randValue = Random.value;
            tempList = whoAmITargeting.Where(monster => monster != randTarget && monster.GetComponent<CreateMonster>().monsterRowPosition == CreateMonster.MonsterRowPosition.FrontRow).ToList();
            if (tempList.Count != 0)
            {
                if (randValue < .66f)
                {
                    //Debug.Log($"Targeting FRONT ROW!");
                    randTarget = tempList[Random.Range(0, tempList.Count)];
                }
            }
        }

        //Debug.Log($"Random target selected: {randTarget.GetComponent<CreateMonster>().monsterReference.name}");
        return randTarget;
    }

    // This function returns a random target from the list of ally monsters // GitHub edit
    public List<GameObject> GetRandomList()
    {
        List<GameObject> randList = allListsOfMonsters[Random.Range(0, allListsOfMonsters.Count)];
        //Debug.Log($"Random list selected: {randList}");
        return randList;
    }

    public static CreateMonster.MonsterRowPosition RandomRowPosition()
    {
        int rand = Random.Range(0, 3);

        switch (rand)
        {
            case (0):
                return CreateMonster.MonsterRowPosition.BackRow;

            case (1):
                return CreateMonster.MonsterRowPosition.CenterRow;

            case (2):
                return CreateMonster.MonsterRowPosition.FrontRow;

            default:
                return CreateMonster.MonsterRowPosition.CenterRow;
        }
    }
}
