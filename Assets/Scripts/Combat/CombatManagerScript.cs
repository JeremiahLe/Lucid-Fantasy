using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class CombatManagerScript : MonoBehaviour
{
    public List<GameObject> BattleSequence;
    public GameObject[] MonstersInBattle;

    public List<GameObject> ListOfAllys;
    public List<GameObject> ListOfEnemies;

    public List<GameObject> initialAllyList;
    public List<GameObject> initialEnemyList;

    public MessageManager CombatLog;
    public HUDAnimationManager HUDanimationManager;
    public ButtonManagerScript buttonManagerScript;
    public EnemyAIManager enemyAIManager;
    public MonsterAttackManager monsterAttackManager;
    public UIManager uiManager;

    public GameObject CurrentMonsterTurn;
    public MonsterAttack CurrentMonsterAttack;
    public Animator CurrentMonsterTurnAnimator;

    public enum MonsterTurn { AllyTurn, EnemyTurn }
    public MonsterTurn monsterTurn;

    public GameObject monsterTargeter;
    public GameObject CurrentTargetedMonster;

    public int currentRound = 1;
    public GameObject firstMonsterTurn;

    public int currentIndex = 0;
    public bool autoBattle = false;
    public bool battleOver = false;
    public bool targeting = false;

    // For Later
    //public List<Action> BattleActions;
    
    // TODO - clean up this script

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        StartCoroutine(InitializeMatch());
    }

    private void Update()
    {
        //CheckInputs();
    }

    /*
    public void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Ally)
                CycleTargets(Monster.AIType.Enemy, 1);
        }
        else
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Ally)
                CycleTargets(Monster.AIType.Enemy, -1);
        }
    }
    */

    // This function initializes all components at start
    public void InitializeComponents()
    {
        // This fixes the build bug
        Resources.LoadAll<Monster>("Assets/Monsters");
        Resources.LoadAll<MonsterAttack>("Assets/Monster Attacks");

        # region Old Code
        /*
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            switch (child.name)
            {
                case "BattleStartText":
                    //BattleStartTextPopup = child.GetComponent<TextMeshProUGUI>();
                    //break;

                case "CombatOrderText":
                    //CombatOrderTextList = child.GetComponent<TextMeshProUGUI>();
                    //break;

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
        */
        #endregion

        HUDanimationManager = GetComponent<HUDAnimationManager>();
        buttonManagerScript = GetComponent<ButtonManagerScript>();
        enemyAIManager = GetComponent<EnemyAIManager>();
        monsterAttackManager = GetComponent<MonsterAttackManager>();
        uiManager = GetComponent<UIManager>();
        CombatLog = GetComponent<MessageManager>();

        // This hides all HUD elements before round start animation is done
        uiManager.HideEverything();
    }

    // This function adds all monsters in battle into a list of monsters
    public void GetAllMonstersInBattle()
    {
        MonstersInBattle = GameObject.FindGameObjectsWithTag("Monster"); // TODO: Fix delete monsters bug - GitHub Comment

        foreach (GameObject monster in MonstersInBattle)
        {
            if (monster.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Ally && ListOfAllys.Contains(monster) != true)
            {
                ListOfAllys.Add(monster);
                UpdateMonsterList(monster, Monster.AIType.Ally);
                BattleSequence.Add(monster);
            }
            else if (monster.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Enemy && ListOfEnemies.Contains(monster) != true)
            {
                ListOfEnemies.Add(monster);
                UpdateMonsterList(monster, Monster.AIType.Enemy);
                BattleSequence.Add(monster);
            }
        }

        SortMonsterBattleSequence(true); // This should be called after the foreach loop, meaning all monsters in battle have been added to list
    }

    // This function sorts the monster battle sequence by speed after all monsters have been identified and added to list
    public void SortMonsterBattleSequence(bool monsterJoinedBattle)
    {
        BattleSequence = BattleSequence.OrderByDescending(Monster => Monster.GetComponent<CreateMonster>().monsterSpeed).ToList();
        firstMonsterTurn = BattleSequence[0];
        uiManager.CombatOrderTextList.text = ($"Combat Order:\n");

        for (int i = 0; i < BattleSequence.Count; i++)
        {
            Monster monster = BattleSequence[i].GetComponent<CreateMonster>().monsterReference;
            uiManager.CombatOrderTextList.text += ($"Monster {i + 1}: {monster.aiType} {monster.name} || Speed: {monster.speed}\n");
            if (monsterJoinedBattle)
            {
                CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} has joined the battle!"); // Update Combat Log with all monsters in battle
            }
        }

        SetCurrentMonsterTurn(); // This should be called after the order of monsters is decided.
    }

    // This override function sorts the monster battle sequence by speed after round increment
    public void SortMonsterBattleSequence()
    {
        uiManager.CombatOrderTextList.text = ($"");
        BattleSequence = BattleSequence.OrderByDescending(Monster => Monster.GetComponent<CreateMonster>().monsterReference.speed).ToList(); // fixed non-refreshing list speeds
        firstMonsterTurn = BattleSequence[0];
        uiManager.CombatOrderTextList.text = ($"Combat Order:\n");

        for (int i = 0; i < BattleSequence.Count; i++)
        {
            Monster monster = BattleSequence[i].GetComponent<CreateMonster>().monsterReference;
            if (monster == null)
            {
                continue;
            }
            uiManager.CombatOrderTextList.text += ($"Monster {i + 1}: {monster.aiType} {monster.name} || Speed: {monster.speed}\n");
        }

        //SetCurrentMonsterTurn(); // fix for speed bug?
        if (currentIndex == BattleSequence.Count)
        {
            currentIndex = BattleSequence.Count - 1;
        }
    }

    // This function properly updates the lists of ally and enemy monsters based on what is passed in
    public void UpdateMonsterList(GameObject monster, Monster.AIType aIType)
    {
        Monster _monster = monster.GetComponent<CreateMonster>().monsterReference;

        if (aIType == Monster.AIType.Ally)
        {
            uiManager.UpdateMonsterList(ListOfAllys, Monster.AIType.Ally);
        }
        else if (aIType == Monster.AIType.Enemy)
        {
            uiManager.UpdateMonsterList(ListOfEnemies, Monster.AIType.Enemy);
        }
    }

    // This function sets the monster turn by speed and priority
    public void SetCurrentMonsterTurn()
    {
        if (currentIndex == 0)
        {
            StartCoroutine(IncrementNewRoundIE());
        }

        if (currentIndex == 0)
        {
            // THIS ACTUALLY FIXED THE SPEED NEXT ROUND BUFF LMFAO
            while (CurrentMonsterTurn != firstMonsterTurn)
            {
                SortMonsterBattleSequence();
                CurrentMonsterTurn = BattleSequence[0];
            }
        }

        if (currentIndex == BattleSequence.Count) // last monster in list ( 0,1 + 1 = 2?)
        {
            CurrentMonsterTurn = BattleSequence[BattleSequence.Count - 1];
        }
        else
        if (BattleSequence[currentIndex] != null)
        {
            CurrentMonsterTurn = BattleSequence[currentIndex];
        }
        else if (BattleSequence.Count == 2 && currentIndex == 0)
        {
            currentRound -= 1;
            StartCoroutine(IncrementNewRoundIE());
        }
        else
        {
            NextMonsterTurn(); // idk what this does - DUAL TURNS?
        }

        /*
        If reached the beginning of the combat order, increment turn count
        if (CurrentMonsterTurn == firstMonsterTurn)
        {
            Debug.Log("Incremented round (firstmonsterturn)"); //  implement a fix for when the first monster dies before it is their turn again
            IncrementNewRound(false);
        }
        */

        uiManager.InitiateMonsterTurnIndicator(CurrentMonsterTurn);
        Monster monster = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;

        if (!battleOver)
        {
            // If enemy, AI move
            if (monster.aiType == Monster.AIType.Enemy)
            {
                targeting = false;
                monsterTurn = MonsterTurn.EnemyTurn;
                buttonManagerScript.HideAllButtons("All");
                HUDanimationManager.MonsterCurrentTurnText.text = ($"Enemy {monster.name} turn...");

                // Call enemy AI script after a delay
                enemyAIManager.currentEnemyTurnGameObject = CurrentMonsterTurn;
                enemyAIManager.currentEnemyTurn = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;
                enemyAIManager.listOfAllies = ListOfAllys;
                enemyAIManager.Invoke("SelectMove", 1.7f);
            }
            // If ally, give player move
            else if (monster.aiType == Monster.AIType.Ally)
            {
                if (!autoBattle) // manual targeting
                {
                    monsterTurn = MonsterTurn.AllyTurn;

                    buttonManagerScript.ListOfMonsterAttacks.Clear();
                    HUDanimationManager.MonsterCurrentTurnText.text = ($"What will {monster.name} do?");
                    buttonManagerScript.AssignAttackMoves(monster);
                    buttonManagerScript.ResetHUD();

                    CurrentMonsterTurnAnimator = CurrentMonsterTurn.GetComponent<Animator>();
                }
                else // auto battle - TODO - make auto battle less random!!
                {
                    monsterTurn = MonsterTurn.AllyTurn;

                    buttonManagerScript.ListOfMonsterAttacks.Clear();
                    HUDanimationManager.MonsterCurrentTurnText.text = ($"Ally {monster.name} turn...");
                    StartCoroutine(ReturnAllyMoveAndTargetCoroutine(monster)); // start delay
                }
            }
        }
    }

    // This OVVERIDE function sets the monster turn by speed and priority
    public void SetCurrentMonsterTurn(bool autoBattle)
    {
        if (currentIndex == 0)
        {
            //StartCoroutine(IncrementNewRoundIE()); // delay before incrementing round
        }

        if (BattleSequence[currentIndex] != null)
        {
            CurrentMonsterTurn = BattleSequence[currentIndex];
        }
        else
        {
            NextMonsterTurn(); // idk what this does - DUAL TURNS?
        }

        /*
        If reached the beginning of the combat order, increment turn count
        if (CurrentMonsterTurn == firstMonsterTurn)
        {
            Debug.Log("Incremented round (firstmonsterturn)"); //  implement a fix for when the first monster dies before it is their turn again
            IncrementNewRound(false);
        }
        */

        uiManager.InitiateMonsterTurnIndicator(CurrentMonsterTurn);
        Monster monster = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;

        if (!battleOver)
        {
            // If enemy, AI move
            if (monster.aiType == Monster.AIType.Enemy)
            {
                targeting = false;
                monsterTurn = MonsterTurn.EnemyTurn;
                buttonManagerScript.HideAllButtons("All");
                HUDanimationManager.MonsterCurrentTurnText.text = ($"Enemy {monster.name} turn...");

                // Call enemy AI script after a delay
                enemyAIManager.currentEnemyTurnGameObject = CurrentMonsterTurn;
                enemyAIManager.currentEnemyTurn = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;
                enemyAIManager.listOfAllies = ListOfAllys;
                enemyAIManager.Invoke("SelectMove", 1.7f);
            }
            // If ally, give player move
            else if (monster.aiType == Monster.AIType.Ally)
            {
                if (!autoBattle) // manual targeting
                {
                    monsterTurn = MonsterTurn.AllyTurn;

                    buttonManagerScript.ListOfMonsterAttacks.Clear();
                    HUDanimationManager.MonsterCurrentTurnText.text = ($"What will {monster.name} do?");
                    buttonManagerScript.AssignAttackMoves(monster);
                    buttonManagerScript.ResetHUD();

                    CurrentMonsterTurnAnimator = CurrentMonsterTurn.GetComponent<Animator>();
                }
                else // auto battle - TODO - make auto battle less random!!
                {
                    monsterTurn = MonsterTurn.AllyTurn;

                    buttonManagerScript.ListOfMonsterAttacks.Clear();
                    HUDanimationManager.MonsterCurrentTurnText.text = ($"Ally {monster.name} turn...");
                    StartCoroutine(ReturnAllyMoveAndTargetCoroutine(monster)); // start delay
                }
            }
        }
    }

    // This coroutine acts as an artificial delay for autobattle similar to the enemy AI
    IEnumerator ReturnAllyMoveAndTargetCoroutine(Monster monster)
    {
        yield return new WaitForSeconds(1.5f);
        buttonManagerScript.AssignAttackMoves(monster);

        CurrentMonsterTurnAnimator = CurrentMonsterTurn.GetComponent<Animator>();

        monsterAttackManager.currentMonsterAttack = GetRandomMove();
        CurrentTargetedMonster = GetRandomTarget();
        Monster targetedMonster = CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;

        monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 2.5f, CurrentTargetedMonster.transform.position.z);
        monsterTargeter.SetActive(true);

        uiManager.EditCombatMessage($"Ally {monster.name} will use {monsterAttackManager.currentMonsterAttack.monsterAttackName} on {targetedMonster.aiType} {targetedMonster.name}!");
        monsterAttackManager.Invoke("UseMonsterAttack", 1.7f);
    }

    // This override function is called on autobattle to not increment round count
    /*
    public void SetCurrentMonsterTurn(bool autoBattle)
    {
        if (BattleSequence[currentIndex] != null)
        {
            CurrentMonsterTurn = BattleSequence[currentIndex];
        }
        else
        {
            NextMonsterTurn(); // idk what this does - DUAL TURNS?
        }

        Monster monster = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;

        if (!battleOver)
        {
            // If enemy, AI move
            if (monster.aiType == Monster.AIType.Enemy)
            {
                monsterTurn = MonsterTurn.EnemyTurn;
                buttonManagerScript.HideAllButtons("All");
                HUDanimationManager.MonsterCurrentTurnText.text = ($"Enemy {monster.name} turn...");

                // Call enemy AI script after a delay
                enemyAIManager.currentEnemyTurnGameObject = CurrentMonsterTurn;
                enemyAIManager.currentEnemyTurn = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;
                enemyAIManager.currentEnemyTurnGameObject.GetComponent<Animator>().SetBool("monsterCurrentTurn", true);
                enemyAIManager.listOfAllies = ListOfAllys;
                enemyAIManager.Invoke("SelectMove", 1.7f);
            }
            // If ally, give player move
            else if (monster.aiType == Monster.AIType.Ally)
            {
                if (!autoBattle) // manual targeting
                {
                    monsterTurn = MonsterTurn.AllyTurn;

                    buttonManagerScript.ListOfMonsterAttacks.Clear();
                    HUDanimationManager.MonsterCurrentTurnText.text = ($"What will {monster.name} do?");
                    buttonManagerScript.AssignAttackMoves(monster);
                    buttonManagerScript.ResetHUD();

                    CurrentMonsterTurnAnimator = CurrentMonsterTurn.GetComponent<Animator>();
                }
                else // auto battle - TODO - make auto battle less random!!
                {
                    monsterTurn = MonsterTurn.AllyTurn;

                    buttonManagerScript.ListOfMonsterAttacks.Clear();
                    HUDanimationManager.MonsterCurrentTurnText.text = ($"What will {monster.name} do?");
                    buttonManagerScript.AssignAttackMoves(monster);

                    CurrentMonsterTurnAnimator = CurrentMonsterTurn.GetComponent<Animator>();

                    monsterAttackManager.currentMonsterAttack = GetRandomMove();
                    CurrentTargetedMonster = GetRandomTarget();
                    Monster targetedMonster = CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;

                    monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 2.5f, CurrentTargetedMonster.transform.position.z);
                    monsterTargeter.SetActive(true);

                    uiManager.EditCombatMessage($"Ally {monster.name} will use {monsterAttackManager.currentMonsterAttack.monsterAttackName} on {targetedMonster.aiType} {targetedMonster.name}!");
                    monsterAttackManager.Invoke("UseMonsterAttack", 2.1f);
                }
            }
        }
    }
    */

    // This function returns a random move from the monsters list of monster attacks
    public MonsterAttack GetRandomMove()
    {
        MonsterAttack randMove = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.ListOfMonsterAttacks[Random.Range(0, CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.ListOfMonsterAttacks.Count)];
        while (randMove.attackOnCooldown)
        {
            randMove = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.ListOfMonsterAttacks[Random.Range(0, CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.ListOfMonsterAttacks.Count)];
        }
        return randMove;
    }

    // This function returns a random target from the list of ally monsters // GitHub edit
    public GameObject GetRandomTarget()
    {
        GameObject randTarget = ListOfEnemies[Random.Range(0, ListOfEnemies.Count)];
      
        if (randTarget.transform.position != randTarget.GetComponent<CreateMonster>().startingPosition.transform.position)
        {
            randTarget.transform.position = randTarget.GetComponent<CreateMonster>().startingPosition.transform.position;
        }
        return randTarget;
    }

    // This function gets called when an attack is chosen and the target is required
    public void TargetingEnemyMonsters(bool _targeting)
    {
        switch (_targeting)
        {
            case true:
                monsterTargeter.SetActive(true);
                targeting = true;
                CurrentTargetedMonster = GetRandomTarget(); // for autoBattle fixme properly
                //monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 2.5f, CurrentTargetedMonster.transform.position.z);
                break;

            default:
                Debug.Log("Missing target or attack reference?", this);
                break;
        }
    }

    // This function cycles through either list of targetable monsters (ally or enemy)
    public void CycleTargets(GameObject newTarget)
    {
        if (targeting)
        {
            CurrentTargetedMonster = newTarget;
            monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 2.5f, CurrentTargetedMonster.transform.position.z);
            monsterAttackManager.UpdateCurrentTargetText();
        }

        #region Old Code
        /*
        int index;

        switch (targetingWho)
        {
            case Monster.AIType.Ally:

                index = ListOfAllys.IndexOf(CurrentTargetedMonster);

                if (index + 1 >= ListOfAllys.Count || index - 1 <= ListOfAllys.Count)
                {
                    CurrentTargetedMonster = ListOfAllys[0];
                    monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 2.5f, CurrentTargetedMonster.transform.position.z);
                }
                else
                {
                    CurrentTargetedMonster = ListOfAllys[index + 1];
                    monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 2.5f, CurrentTargetedMonster.transform.position.z);
                }
                break;

            case Monster.AIType.Enemy:

                index = ListOfEnemies.IndexOf(CurrentTargetedMonster);

                if (index + 1 >= ListOfEnemies.Count)
                {
                    CurrentTargetedMonster = ListOfEnemies[0];
                    monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 2.5f, CurrentTargetedMonster.transform.position.z);
                    monsterAttackManager.UpdateCurrentTargetText();
                }
                else
                {
                    CurrentTargetedMonster = ListOfEnemies[index];
                    monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 2.5f, CurrentTargetedMonster.transform.position.z);
                    monsterAttackManager.UpdateCurrentTargetText();
                }
                break;

            default:
                Debug.Log($"Missing list of targets or monster type reference?", this);
                break;
        }
        */
        #endregion
    }

    // This function moves the current monster turn to the next in line
    public void NextMonsterTurn()
    {
        //if (BattleSequence.Count == currentIndex + 1) // fix an index bug
        //{
        //   CurrentTargetedMonster = null; // may fix a weird non-targeting bug with miss text gameobject
        //    SetCurrentMonsterTurn();
        //    return;
        //}

        // if the current monster turn == first monster?
        if (currentIndex + 1 >= BattleSequence.Count && CurrentMonsterTurn != BattleSequence[0] && CurrentMonsterTurn != null)
        {
            currentIndex = 0;
        }
        else if (currentIndex + 1 <= BattleSequence.Count)
        {
            currentIndex++;
        }
        else if (currentIndex == BattleSequence.Count)
        {
            currentIndex = BattleSequence.Count - 1;
        }
                

        CurrentTargetedMonster = null; // may fix a weird non-targeting bug with miss text gameobject
        SetCurrentMonsterTurn();
    }

    // This function handles all new round calls (status effects, speed adjustments etc.)
    public void IncrementNewRound()
    {
        if (!battleOver)
        {
            currentRound += 1;
            uiManager.IncrementRoundCount(currentRound);

            if (!autoBattle)
            {
                //buttonManagerScript.ResetHUD(); // causes a bug
            }

            //uiManager.RoundStartTextPopup.faceColor = new Color32(255, 255, 255, 0);
            //uiManager.RoundStartTextPopup.text = ($"Round {currentRound}");
            //uiManager.FadeText(uiManager.RoundStartTextPopup);

            CombatLog.SendMessageToCombatLog($"-- Round {currentRound} -- ");
            SortMonsterBattleSequence(); // non battle start version

            // check if any cooldowns need to be updated
            foreach (GameObject monster in BattleSequence)
            {
                monster.GetComponent<CreateMonster>().CheckCooldowns();
            }
        }
    }

    // Coroutine to to increment round counter after a delay
    IEnumerator IncrementNewRoundIE()
    {
        yield return new WaitForSeconds(.15f);
        IncrementNewRound();
    }

    // Coroutine to initialize HUD elements at start of match
    IEnumerator InitializeMatch()
    {
        yield return new WaitForSeconds(1.0f);
        GetAllMonstersInBattle();
    }

    /*
    // This override function only increments round counter (does not increment twice)
    public void IncrementNewRound(bool adjustBattleSequence)
    {
        if (!battleOver)
        {
            currentRound += 1;
            uiManager.IncrementRoundCount(currentRound);
            //SortMonsterBattleSequence(); // this should not be called
        }
    }
    */

    // This function removes a monster from a list
    public void RemoveMonsterFromList(GameObject monsterToRemove, Monster.AIType allyOrEnemy)
    {
        Monster monsterRef = monsterToRemove.GetComponent<CreateMonster>().monsterReference;

        switch (allyOrEnemy)
        {
            case Monster.AIType.Ally:
                ListOfAllys.Remove(monsterToRemove);
                Destroy(monsterToRemove);
                uiManager.UpdateMonsterList(ListOfAllys, Monster.AIType.Ally);
                Debug.Log($"Removed {monsterToRemove.GetComponent<CreateMonster>().monsterReference.name}", this);
                break;

            case Monster.AIType.Enemy:
                ListOfEnemies.Remove(monsterToRemove);
                Destroy(monsterToRemove);
                uiManager.UpdateMonsterList(ListOfEnemies, Monster.AIType.Enemy);
                Debug.Log($"Removed {monsterToRemove.GetComponent<CreateMonster>().monsterReference.name}", this);
                break;

            default:
                Debug.Log("Missing monster object or AI type reference?", this);
                break;
        }

        BattleSequence.Remove(monsterToRemove);
        SortMonsterBattleSequence();
        CheckMonstersAlive();
    }

    /*
    // This function should update the visuals when something changes (death, speed, etc.)
    public void UpdateHUD(GameObject monsterToRemove)
    {
        //CombatOrderTextList.text = "";
        BattleSequence.Remove(monsterToRemove);

        SortMonsterBattleSequence(false);
        //EnemyTextList.text = "";

        CheckMonstersAlive();

        //GetAllMonstersInBattle(); Problematic
    }
    */

    // This function emits temporary win/lose message conditions based on monster lists
    public void CheckMonstersAlive()
    {
        if (ListOfAllys.Count() == 0)
        {
            uiManager.EditCombatMessage("You lose!");
            BattleOver();
        }
        else 
        if (ListOfEnemies.Count() == 0)
        {
            uiManager.EditCombatMessage("You win!");
            BattleOver();
        }
    }

    // Tjis function should call all battle over functions
    public void BattleOver()
    {
        battleOver = true;
        CurrentMonsterTurn = null;
        targeting = false;

        buttonManagerScript.HideAllButtons("All");
        Invoke("RestartBattleScene", 3.0f);
    }

    // This function resets the battle scene
    public void RestartBattleScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // This function starts auto battle
    public void BeginAutoBattle()
    {
        autoBattle = true;
        SetCurrentMonsterTurn(true);
        buttonManagerScript.HideAllButtons("All");
    }
}

