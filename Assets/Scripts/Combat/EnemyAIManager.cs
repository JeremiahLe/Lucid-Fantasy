using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Modifier;

public class EnemyAIManager : MonoBehaviour
{
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

    public void SelectMonsterAttackByAILevel()
    {
        GameObject currentMonsterGameObject = combatManagerScript.CurrentMonsterTurn;
        CreateMonster monsterComponent = currentMonsterGameObject.GetComponent<CreateMonster>();
        Monster currentMonster = monsterComponent.monsterReference;
        MonsterAttack selectedAttack = null;

        if (CheckAndChangeMonsterStance(currentMonsterGameObject))
        {
            combatManagerScript.StanceChanged();
            return;
        }

        switch (currentMonster.aiLevel)
        {
            case Monster.AILevel.Offensive:
                selectedAttack = GetDamagingAttack(currentMonsterGameObject);
                break;

            case Monster.AILevel.Random:
                selectedAttack = GetRandomAttack(currentMonsterGameObject);
                break;

            case Monster.AILevel.Supportive:
                selectedAttack = GetSupportiveAttack(currentMonsterGameObject);
                break;

            case Monster.AILevel.Player:
                selectedAttack = GetRandomAttack(currentMonsterGameObject);
                break;

            default:
                Debug.Log("Missing or incorrect AI Level reference?", this);
                break;
        }

        if (selectedAttack == null)
        {
            combatManagerScript.PassTurn();
            return;
        }

        AdjustTargetMonsterByAttack(selectedAttack, currentMonsterGameObject);
    }

    public void AdjustTargetMonsterByAttack(MonsterAttack selectedAttack, GameObject currentMonsterGameObject)
    {
        List<GameObject> myListOfAllys = new List<GameObject>();
        List<GameObject> myListOfEnemies = new List<GameObject>();
        CreateMonster monsterComponent = currentMonsterGameObject.GetComponent<CreateMonster>();

        monsterAttackManager.ListOfCurrentlyTargetedMonsters.Clear();

        if (monsterComponent.monsterReference.aiType == Monster.AIType.Ally) 
        {
            myListOfAllys = combatManagerScript.ListOfAllys;
            myListOfEnemies = combatManagerScript.ListOfEnemies;
        }
        else
        {
            myListOfAllys = combatManagerScript.ListOfEnemies;
            myListOfEnemies = combatManagerScript.ListOfAllys;
        }

        switch (selectedAttack.monsterAttackTargetType)
        {
            case MonsterAttack.MonsterAttackTargetType.EnemyTarget:
                combatManagerScript.CurrentTargetedMonster = GetRandomTarget(myListOfEnemies);
                break;

            case MonsterAttack.MonsterAttackTargetType.AllyTarget:
                combatManagerScript.CurrentTargetedMonster = GetRandomTarget(myListOfAllys);
                break;

            case MonsterAttack.MonsterAttackTargetType.SelfTarget:
                combatManagerScript.CurrentTargetedMonster = currentMonsterGameObject;
                break;

            case MonsterAttack.MonsterAttackTargetType.Any:
                combatManagerScript.CurrentTargetedMonster = GetRandomTarget(myListOfEnemies);
                break;
        }

        if (monsterComponent.listofCurrentStatusEffects.Contains(StatusEffectType.Enraged))
        {
            combatManagerScript.CurrentTargetedMonster = monsterComponent.monsterEnragedTarget;

            if (selectedAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.MultiTarget)
            {
                for (int i = 0; i < selectedAttack.monsterAttackTargetCountNumber; i++)
                {
                    monsterAttackManager.ListOfCurrentlyTargetedMonsters.Add(combatManagerScript.CurrentTargetedMonster);
                }
            }
        }

        if (!monsterComponent.listofCurrentStatusEffects.Contains(StatusEffectType.Enraged))
        {
            if (selectedAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.MultiTarget && selectedAttack.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.Any)
            {
                for (int i = 0; i < selectedAttack.monsterAttackTargetCountNumber; i++)
                {
                    monsterAttackManager.ListOfCurrentlyTargetedMonsters.Add(GetRandomTarget(myListOfEnemies));
                }
            }

            if (selectedAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.MultiTarget && selectedAttack.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.AllyTarget)
            {
                for (int i = 0; i < selectedAttack.monsterAttackTargetCountNumber; i++)
                {
                    monsterAttackManager.ListOfCurrentlyTargetedMonsters.Add(GetRandomTarget(myListOfAllys));
                }
            }

            if (selectedAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.MultiTarget && selectedAttack.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.EnemyTarget)
            {
                for (int i = 0; i < selectedAttack.monsterAttackTargetCountNumber; i++)
                {
                    monsterAttackManager.ListOfCurrentlyTargetedMonsters.Add(GetRandomTarget(myListOfEnemies));
                }
            }
        }

        monsterAttackManager.currentMonsterAttack = selectedAttack;
        monsterAttackManager.Invoke(nameof(monsterAttackManager.UseMonsterAttack), 0.2f);
    }

    public MonsterAttack GetSupportiveAttack(GameObject currentMonsterGameObject)
    {
        CreateMonster monsterComponent = currentMonsterGameObject.GetComponent<CreateMonster>();
        Monster currentMonster = monsterComponent.monsterReference;
        List<MonsterAttack> tempList = currentMonster.ListOfMonsterAttacks.Where(Attack => Attack.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.SelfTarget || Attack.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.AllyTarget).ToList();

        if (tempList.Count == 0)
        {
            currentMonster.aiLevel = Monster.AILevel.Supportive;
            return null;
        }

        if (monsterComponent.listofCurrentStatusEffects.Contains(StatusEffectType.Enraged))
        {
            tempList = currentMonster.ListOfMonsterAttacks.Where(Attack => Attack.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.EnemyTarget || Attack.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.Any).ToList();
        }

        if (tempList.Count == 0)
        {
            currentMonster.aiLevel = Monster.AILevel.Supportive;
            return null;
        }

        MonsterAttack randomAttack = tempList[Random.Range(0, tempList.Count)];

        if (randomAttack.monsterAttackSPCost > currentMonster.currentSP)
            return null;

        return randomAttack;
    }

    public MonsterAttack GetDamagingAttack(GameObject currentMonsterGameObject)
    {
        CreateMonster monsterComponent = currentMonsterGameObject.GetComponent<CreateMonster>();
        Monster currentMonster = monsterComponent.monsterReference;
        List<MonsterAttack> tempList = currentMonster.ListOfMonsterAttacks.Where(Attack => Attack.monsterAttackTargetType != MonsterAttack.MonsterAttackTargetType.SelfTarget || Attack.monsterAttackTargetType != MonsterAttack.MonsterAttackTargetType.AllyTarget).ToList();

        if (tempList.Count == 0)
        {
            currentMonster.aiLevel = Monster.AILevel.Supportive;
            return null;
        }

        if (monsterComponent.listofCurrentStatusEffects.Contains(StatusEffectType.Enraged))
        {
            tempList = currentMonster.ListOfMonsterAttacks.Where(Attack => Attack.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.EnemyTarget || Attack.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.Any).ToList();
        }

        if (tempList.Count == 0)
        {
            currentMonster.aiLevel = Monster.AILevel.Supportive;
            return null;
        }

        MonsterAttack randomAttack = tempList[Random.Range(0, tempList.Count)];

        if (randomAttack.monsterAttackSPCost > currentMonster.currentSP)
            return null;

        return randomAttack;
    }

    // This function returns a random move from the monsters list of monster attacks
    public MonsterAttack GetRandomAttack(GameObject currentMonsterGameObject)
    {
        CreateMonster monsterComponent = currentMonsterGameObject.GetComponent<CreateMonster>();
        Monster currentMonster = monsterComponent.monsterReference;
        List<MonsterAttack> tempList = currentMonster.ListOfMonsterAttacks.Where(Attack => Attack.monsterAttackSPCost <= currentMonster.currentSP).ToList();

        if (currentMonster.currentSP == 1)
            if (RandomChanceToChangeStance(.35f))
                return null;

        if (tempList.Count == 0)
            return null;

        if (monsterComponent.listofCurrentStatusEffects.Contains(StatusEffectType.Enraged))
        {
            tempList = currentMonster.ListOfMonsterAttacks.Where(Attack => Attack.monsterAttackSPCost <= currentMonster.currentSP && Attack.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.EnemyTarget || Attack.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.Any).ToList();
        }

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

    public bool CheckAndChangeMonsterStance(GameObject currentMonsterGameObject)
    {
        CreateMonster monsterComponent = currentMonsterGameObject.GetComponent<CreateMonster>();
        Monster currentMonster = monsterComponent.monsterReference;

        if (currentMonster.aiLevel == Monster.AILevel.Offensive && monsterComponent.monsterStance != CreateMonster.MonsterStance.Aggressive)
        {
            if (RandomChanceToChangeStance(.50f))
            {
                monsterComponent.SetMonsterStance(CreateMonster.MonsterStance.Aggressive);
                return true;
            }
        }

        if (currentMonster.aiLevel == Monster.AILevel.Supportive && monsterComponent.monsterStance != CreateMonster.MonsterStance.Defensive)
        {
            if (RandomChanceToChangeStance(.50f))
            {
                monsterComponent.SetMonsterStance(CreateMonster.MonsterStance.Defensive);
                return true;
            }
        }

        if (currentMonster.aiLevel == Monster.AILevel.Random)
        {
            if (RandomChanceToChangeStance(.25f))
            {
                CreateMonster.MonsterStance randomStance = GetRandomStance();

                if (randomStance == monsterComponent.monsterStance)
                    return false;

                monsterComponent.SetMonsterStance(randomStance);
                return true;
            }
        }

        return false;
    }

    public bool RandomChanceToChangeStance(float chance)
    {
        float randValue = Random.value;

        if (randValue < chance)
        {
            return true;
        }

        return false;
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
