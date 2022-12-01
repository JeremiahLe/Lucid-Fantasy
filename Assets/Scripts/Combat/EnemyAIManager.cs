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
                currentEnemyMonsterAttack = GetRandomAttack();

                if (currentEnemyMonsterAttack == null)
                    combatManagerScript.buttonManagerScript.PassButtonClicked();

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
                        if (currentEnemyTurnGameObject.GetComponent<CreateMonster>().listofCurrentStatusEffects.Contains(Modifier.StatusEffectType.Dazed))
                        {
                            currentEnemyTargetGameObject = GetRandomTarget(GetRandomList());
                        }
                        else
                        {
                            currentEnemyTargetGameObject = GetRandomTarget(combatManagerScript.ListOfAllys);
                        }
                        break;
                }

                currentEnemyTarget = currentEnemyTargetGameObject.GetComponent<CreateMonster>().monsterReference;

                combatManagerScript.CurrentMonsterTurnAnimator = currentEnemyTurnGameObject.GetComponent<Animator>();
                combatManagerScript.CurrentTargetedMonster = currentEnemyTargetGameObject;
                monsterAttackManager.currentMonsterAttack = currentEnemyMonsterAttack;

                // Random chance to change row position
                CreateMonster.MonsterStance newStance = GetRandomStance();
                CreateMonster monsterComponent = combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>();

                if (newStance != monsterComponent.monsterStance)
                {
                    monsterComponent.SetMonsterStance(newStance);
                }

                if (monsterAttackManager.currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.AllTargets)
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

                monsterAttackManager.Invoke("UseMonsterAttack", 0.2f);
                break;

            default:
                Debug.Log("Missing AI Level or monster reference?", this);
                break;
        }
    }

    // This function returns a random move from the monsters list of monster attacks
    public MonsterAttack GetRandomAttack()
    {
        List<MonsterAttack> tempList = enemyListOfMonsterAttacks.Where(Attack => Attack.monsterAttackSPCost <= currentEnemyTurn.currentSP).ToList();

        if (tempList.Count == 0)
            return null;

        MonsterAttack randomAttack = tempList[Random.Range(0, tempList.Count)];

        return randomAttack;
    }

    // This function returns a random target from the list of ally monsters // GitHub edit
    public GameObject GetRandomTarget(List<GameObject> whoAmITargeting)
    {
        GameObject randTarget = whoAmITargeting[Random.Range(0, whoAmITargeting.Count)];
        float randValue = 1;
        List<GameObject> tempList;

        // If initial target is backrow, chance to target another monster. If initial target is centerrow, chance to target front row monster
        if (whoAmITargeting.Count > 1 && randTarget.GetComponent<CreateMonster>().monsterStance == CreateMonster.MonsterStance.Defensive)
        {
            randValue = Random.value;
            //Debug.Log($"Generated random value: {randValue}");
            if (randValue < .66f)
            {
                tempList = whoAmITargeting.Where(monster => monster != randTarget).ToList();
                randTarget = tempList[Random.Range(0, tempList.Count)];
                //Debug.Log($"Targeting another unit!");

                randValue = Random.value;
                tempList = whoAmITargeting.Where(monster => monster != randTarget && monster.GetComponent<CreateMonster>().monsterStance == CreateMonster.MonsterStance.Aggressive).ToList();
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
        if (whoAmITargeting.Count > 1 && randTarget.GetComponent<CreateMonster>().monsterStance == CreateMonster.MonsterStance.Neutral)
        {
            randValue = Random.value;
            tempList = whoAmITargeting.Where(monster => monster != randTarget && monster.GetComponent<CreateMonster>().monsterStance == CreateMonster.MonsterStance.Aggressive).ToList();
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
        return randList;
    }

    public static CreateMonster.MonsterStance GetRandomStance()
    {
        int rand = Random.Range(0, 3);

        switch (rand)
        {
            case (0):
                return CreateMonster.MonsterStance.Defensive;

            case (1):
                return CreateMonster.MonsterStance.Neutral;

            case (2):
                return CreateMonster.MonsterStance.Aggressive;

            default:
                return CreateMonster.MonsterStance.Neutral;
        }
    }
}
