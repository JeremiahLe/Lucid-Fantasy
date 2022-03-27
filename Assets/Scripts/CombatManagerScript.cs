using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class CombatManagerScript : MonoBehaviour
{
    public List<GameObject> BattleSequence;
    public GameObject[] MonstersInBattle;

    public List<GameObject> ListOfAllys;
    public List<GameObject> ListOfEnemies;

    private TextMeshProUGUI CombatOrderTextList;
    private TextMeshProUGUI BattleStartTextPopup;
    public MessageManager CombatLog;

    private TextMeshProUGUI AllyTextList;
    private TextMeshProUGUI EnemyTextList;

    public GameObject CurrentMonsterTurn;

    public HUDAnimationManager HUDanimationManager;
    public ButtonManagerScript buttonManagerScript;
    public EnemyAIManager enemyAIManager;

    public Animator CurrentMonsterTurnAnimator;

    public GameObject monsterTargeter;
    public GameObject CurrentTargetedMonster;

    public int currentIndex = 0;

    // For Later
    //public List<Action> BattleActions;

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        FadeText(BattleStartTextPopup);
        GetAllMonstersInBattle();
    }

    // This function initializes all components at start
    public void InitializeComponents()
    {
        // This fixes the build bug
        Resources.LoadAll<Monster>("Assets/Monsters");
        Resources.LoadAll<MonsterAttack>("Assets/Monster Attacks");

        Transform[] children = transform.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            switch (child.name)
            {
                case "BattleStartText":
                    BattleStartTextPopup = child.GetComponent<TextMeshProUGUI>();
                    break;

                case "CombatOrderText":
                    CombatOrderTextList = child.GetComponent<TextMeshProUGUI>();
                    break;

                case "ListOfAlliesText":
                    AllyTextList = child.GetComponent<TextMeshProUGUI>();
                    AllyTextList.text = ("Allies:\n");
                    break;

                case "ListOfEnemiesText":
                    EnemyTextList = child.GetComponent<TextMeshProUGUI>();
                    EnemyTextList.text = ("Enemies:\n");
                    break;

                case "CombatManagerObj":
                    CombatLog = GetComponent<MessageManager>();
                    break;

                default:
                    //Debug.Log($"Missing a text object reference? {child.name}", this);
                    break;
            }
        }

        HUDanimationManager = GetComponent<HUDAnimationManager>();
        buttonManagerScript = GetComponent<ButtonManagerScript>();
        enemyAIManager = GetComponent<EnemyAIManager>();
    }

    // This function adds all monsters in battle into a list of monsters
    public void GetAllMonstersInBattle()
    {
        MonstersInBattle = GameObject.FindGameObjectsWithTag("Monster");

        foreach (GameObject monster in MonstersInBattle)
        {
            if (monster.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Ally)
            {
                ListOfAllys.Add(monster);
                UpdateMonsterList(monster, Monster.AIType.Ally);
            }
            else if (monster.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Enemy)
            {
                ListOfEnemies.Add(monster);
                UpdateMonsterList(monster, Monster.AIType.Enemy);
            }

            BattleSequence.Add(monster);
        }

        SortMonsterBattleSequence(); // This should be called after the foreach loop, meaning all monsters in battle have been added to list
    }

    // This function sorts the monster battle sequence by speed after all monsters have been identified and added to list
    public void SortMonsterBattleSequence()
    {
        BattleSequence = BattleSequence.OrderByDescending(Monster => Monster.GetComponent<CreateMonster>().monsterReference.speed).ToList();
        CombatOrderTextList.text = ($"Combat Order:\n");

        for (int i = 0; i < BattleSequence.Count; i++)
        {
            Monster monster = BattleSequence[i].GetComponent<CreateMonster>().monsterReference;
            CombatOrderTextList.text += ($"Monster {i + 1}: {monster.aiType} {monster.name} || Speed: {monster.speed}\n");
            CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} has joined the battle!"); // Update Combat Log with all monsters in battle
        }

        SetCurrentMonsterTurn(); // This should be called after the order of monsters is decided.
    }

    // This function fades text passed in
    public void FadeText(TextMeshProUGUI textToFade)
    {
        textToFade.CrossFadeAlpha(0, 1f, true);
    }

    // This function properly updates the lists of ally and enemy monsters based on what is passed in
    public void UpdateMonsterList(GameObject monster, Monster.AIType aIType)
    {
        Monster _monster = monster.GetComponent<CreateMonster>().monsterReference;

        if (aIType == Monster.AIType.Ally)
        {
            AllyTextList.text += ($"{_monster.name}, lvl: {_monster.level}\n");
        }
        else if (aIType == Monster.AIType.Enemy)
        {
            EnemyTextList.text += ($"{_monster.name}, lvl: {_monster.level}\n");
        }
    }

    // This function sets the monster turn by speed and priority
    public void SetCurrentMonsterTurn()
    {
        CurrentMonsterTurn = BattleSequence[currentIndex];
        Monster monster = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;

        // If enemy, AI move
        if (monster.aiType == Monster.AIType.Enemy)
        {
            buttonManagerScript.HideAllButtons("All");
            HUDanimationManager.MonsterCurrentTurnText.text = ($"Enemy {monster.name} turn...");

            // Call enemy AI script after a delay
            enemyAIManager.currentEnemyTurnGameObject = CurrentMonsterTurn;
            enemyAIManager.currentEnemyTurn = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;
            enemyAIManager.listOfAllies = ListOfAllys;
            enemyAIManager.Invoke("SelectMove", 1.4f);
        }
        // If ally, give player move
        else if (monster.aiType == Monster.AIType.Ally)
        {
            buttonManagerScript.ListOfMonsterAttacks.Clear();
            HUDanimationManager.MonsterCurrentTurnText.text = ($"What will {monster.name} do?");
            buttonManagerScript.AssignAttackMoves(monster);
            buttonManagerScript.ResetHUD();
            CurrentMonsterTurnAnimator = CurrentMonsterTurn.GetComponent<Animator>();
        }
    }

    // This function resets the combat message from an attack or something else to the default what will monster do? It also serves to reset combat targeting
    public void ResetCombatMessage(string monsterName)
    {
        HUDanimationManager.MonsterCurrentTurnText.text = ($"What will {monsterName} do?");
        monsterTargeter.SetActive(false);
    }

    // This override function sets the combat message to something specific
    public void EditCombatMessage(string message)
    {
        HUDanimationManager.MonsterCurrentTurnText.text = (message);
    }

    // This function sets the combat message to nothing
    public void EditCombatMessage()
    {
        HUDanimationManager.MonsterCurrentTurnText.text = "";
    }

    // This function gets called when an attack is chosen and the target is required
    public void TargetingEnemyMonsters(bool targeting)
    {
        switch (targeting)
        {
            case true:
                monsterTargeter.SetActive(true);
                CurrentTargetedMonster = ListOfEnemies[0];
                monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 2.5f, CurrentTargetedMonster.transform.position.z);
                break;

            default:
                Debug.Log("Missing target or attack reference?", this);
                break;
        }
    }

    // This function moves the current monster turn to the next in line
    public void NextMonsterTurn()
    {
        if (currentIndex + 1 >= MonstersInBattle.Count())
        {
            currentIndex = 0;
        }
        else if (currentIndex + 1 <= MonstersInBattle.Count())
        {
            currentIndex++;
        }

        SetCurrentMonsterTurn();
    }
}

