using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class CombatManagerScript : MonoBehaviour
{
    [Title("Monster Lists")]
    public GameObject[] MonstersInBattle;
    public List<GameObject> BattleSequence;
    public List<GameObject> ListOfAllys;
    public List<GameObject> ListOfEnemies;

    [Title("Auto-set Components")]
    public MessageManager CombatLog;
    public HUDAnimationManager HUDanimationManager;
    public ButtonManagerScript buttonManagerScript;
    public EnemyAIManager enemyAIManager;
    public MonsterAttackManager monsterAttackManager;
    public UIManager uiManager;
    public SoundEffectManager soundEffectManager;

    [Title("Battle Variables")]
    public GameObject firstMonsterTurn;
    public GameObject CurrentMonsterTurn;

    public GameObject CurrentTargetedMonster;
    public GameObject monsterTargeter;

    public Animator CurrentMonsterTurnAnimator;
    public MonsterAttack CurrentMonsterAttack;

    public enum MonsterTurn { AllyTurn, EnemyTurn }
    public MonsterTurn monsterTurn;

    public int currentRound = 1;

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
        CheckInputs();
    }

    public void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

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

        // randomizes positions of speed ties // I don't want to shuffle on game start. There is no need. Nvm
        
        var speedTies = BattleSequence.GroupBy(Monster => Monster.GetComponent<CreateMonster>().monsterReference.speed).Where(timesRepeated => timesRepeated.Count() > 1).Select(y => new { Value = y.Key, Count = y.Count() });

        foreach (var speed in speedTies)
        {
            int startIndex = BattleSequence.IndexOf(BattleSequence.First(Monster => Monster.GetComponent<CreateMonster>().monsterReference.speed == speed.Value));
            int repititionCount = speed.Count;

            // Shuffle range
            ShuffleRange(BattleSequence, startIndex, repititionCount);
        }

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

        StartCoroutine(IncrementNewRoundIE()); // Initiate the battle
    }

    // This override function sorts the monster battle sequence by speed after round increment
    public void SortMonsterBattleSequence()
    {
        uiManager.CombatOrderTextList.text = ($"");
        BattleSequence = BattleSequence.OrderByDescending(Monster => Monster.GetComponent<CreateMonster>().monsterReference.speed).ToList(); // fixed non-refreshing list speeds

        // randomizes positions of speed ties
        var speedTies = BattleSequence.GroupBy(Monster => Monster.GetComponent<CreateMonster>().monsterReference.speed).Where(timesRepeated => timesRepeated.Count() > 1).Select(y => new { Value = y.Key, Count = y.Count() });

        foreach(var speed in speedTies)
        {
            int startIndex = BattleSequence.IndexOf(BattleSequence.First(Monster => Monster.GetComponent<CreateMonster>().monsterReference.speed == speed.Value));
            int repititionCount = speed.Count;

            // Shuffle range
            ShuffleRange(BattleSequence, startIndex, repititionCount);
        }

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

        /*
        if (currentIndex == BattleSequence.Count) //SetCurrentMonsterTurn(); // fix for speed bug?
        {
            currentIndex = BattleSequence.Count - 1;
        } */
    }

    // Helper Linq function
    public void ShuffleRange<T>(IList<T> list, int startIndex, int count)
    {
        int n = startIndex + count;
        int limit = startIndex + 1;
        while (n > limit)
        {
            n--;
            int k = Random.Range(startIndex, n + 1); // get random, in index position
            T value = list[k]; // get value at randomized position
            list[k] = list[n]; // set randomized position value to other value
            list[n] = value; // set other value to what the randomized position's was
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

    // This new godlike function returns the monster with the highest speed that hasn't had an action yet
    GameObject MonsterNextTurn()
    {
        float highestSpeed = 0;
        GameObject nextFastestMonsterWithActionAvailable = null;

        foreach (GameObject monster in BattleSequence)
        {
            CreateMonster monsterRef = monster.GetComponent<CreateMonster>();
            if (monsterRef.monsterReference.speed > highestSpeed && monsterRef.monsterActionAvailable)
            {
                highestSpeed = monsterRef.monsterReference.speed;
                nextFastestMonsterWithActionAvailable = monster;
            }
        }

        if (nextFastestMonsterWithActionAvailable == null)
        {
            Debug.Log("Returned Null");
            return null;
        }

        // At round start in the case of a speed tie
        if (nextFastestMonsterWithActionAvailable != firstMonsterTurn && firstMonsterTurn.GetComponent<CreateMonster>().monsterActionAvailable)
        {
            Debug.Log("Returned first monster!");
            return firstMonsterTurn;
        }

        return nextFastestMonsterWithActionAvailable;
    }

    // This function sets the monster turn by speed and priority
    public void SetCurrentMonsterTurn()
    {
        #region Old Code
        /*
        if (currentIndex == 0)
        {
            StartCoroutine(IncrementNewRoundIE());
        }
        */

        /*
        if (currentIndex == 0)
        {
            // THIS ACTUALLY FIXED THE SPEED NEXT ROUND BUFF LMFAO
            while (CurrentMonsterTurn != firstMonsterTurn)
            {
                SortMonsterBattleSequence();
                CurrentMonsterTurn = BattleSequence[0];
                Debug.Log("Called speed fix!");
            }
        }
        */
        #endregion

        CurrentTargetedMonster = null;
        CurrentMonsterTurn = MonsterNextTurn();

        if (CurrentMonsterTurn == null)
        {
            StartCoroutine(IncrementNewRoundIE());
        }
        else
        {
            InitiateCombat();
            Debug.Log("Combat!");
        }

        #region Old Code BYE BYE JANK INDEX SYSTEM
        /*
        // First check if they are the last monster in the sequence
        if (currentIndex == BattleSequence.Count) // last monster in list ( 0,1 + 1 = 2?)
        {
            CurrentMonsterTurn = BattleSequence[BattleSequence.Count - 1];
            Debug.Log("Called last sequence!");
        }
        else // else, next monster
        if (BattleSequence[currentIndex] != null)
        {
            CurrentMonsterTurn = BattleSequence[currentIndex];
            Debug.Log("Called normal sequence!");
        }
        else if (BattleSequence.Count == 2 && currentIndex == 0)
        {
            currentRound -= 1;
            StartCoroutine(IncrementNewRoundIE());
            Debug.Log("Called weird round skip sequence!");
        }
        else
        {
            NextMonsterTurn(); // idk what this does - DUAL TURNS?
            Debug.Log("Called NextMonsterTurn due to null!");
        }

        // Final check for third turn skip bug
        if (currentIndex != 0 && CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.speed < BattleSequence[currentIndex - 1].GetComponent<CreateMonster>().monsterReference.speed)
        {
            // Have they had their action yet?
            if (BattleSequence[currentIndex - 1].GetComponent<CreateMonster>().monsterActionAvailable)
            {
                currentIndex--;
                CurrentMonsterTurn = BattleSequence[currentIndex];
                Debug.Log("Called third action skip fix!");
            }
        }

        // Another bug fix
        if (CurrentMonsterTurn.GetComponent<CreateMonster>().monsterActionAvailable == false)
        {
            currentIndex = 0;
            NextMonsterTurn();
            Debug.Log("Called double turn after speed loss fix!");
        }
        */

        // After proper monster turn is selected, begin combat!
        //InitiateCombat();
        #endregion

        #region Old Code
        /*
        If reached the beginning of the combat order, increment turn count
        if (CurrentMonsterTurn == firstMonsterTurn)
        {
            Debug.Log("Incremented round (firstmonsterturn)"); //  implement a fix for when the first monster dies before it is their turn again
            IncrementNewRound(false);
        }
        */
        #endregion
    }

    // This function initiates combat and serves to clean up the SetCurrentMonsterTurn function
    public void InitiateCombat()
    {
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

    // This OVVERIDE function sets the monster turn by speed and priority when starting auto battle
    public void SetCurrentMonsterTurn(bool autoBattle)
    {
        // This should work? haha
        InitiateCombat();
    }

    // This coroutine acts as an artificial delay for autobattle similar to the enemy AI
    IEnumerator ReturnAllyMoveAndTargetCoroutine(Monster monster)
    {
        yield return new WaitForSeconds(1.5f);
        buttonManagerScript.AssignAttackMoves(monster);

        CurrentMonsterTurnAnimator = CurrentMonsterTurn.GetComponent<Animator>();

        monsterAttackManager.currentMonsterAttack = GetRandomMove();

        AttackTypeTargeting();

        Monster targetedMonster = CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;

        monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 2.5f, CurrentTargetedMonster.transform.position.z);
        monsterTargeter.SetActive(true);

        uiManager.EditCombatMessage($"Ally {monster.name} will use {monsterAttackManager.currentMonsterAttack.monsterAttackName} on {targetedMonster.aiType} {targetedMonster.name}!");
        monsterAttackManager.Invoke("UseMonsterAttack", 1.7f);
    }

    #region Old Code
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
    #endregion

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

    // what type of attack was selected
    public void AttackTypeTargeting()
    {
        // What type of attack move was selected?
        switch (monsterAttackManager.currentMonsterAttack.monsterAttackTargetType)
        {
            // If self targeting move, return self
            case (MonsterAttack.MonsterAttackTargetType.SelfTarget):
                CurrentTargetedMonster = CurrentMonsterTurn;
                break;

            case (MonsterAttack.MonsterAttackTargetType.AllyTarget):
                CurrentTargetedMonster = GetRandomTarget(ListOfAllys);
                break;

            default:
                CurrentTargetedMonster = GetRandomTarget(ListOfEnemies);
                break;
        }
    }

    // This function returns a random target from the list of ally monsters // GitHub edit
    public GameObject GetRandomTarget(List<GameObject> whoAmITargeting)
    {
        GameObject randTarget = whoAmITargeting[Random.Range(0, whoAmITargeting.Count)];
      
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
                AttackTypeTargeting(); // for autoBattle fixme properly
                monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 2.5f, CurrentTargetedMonster.transform.position.z);
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

    // Helper function for SetNextMonsterTurn()
    public void NextMonsterTurn()
    {
        #region Old Code
        //if (BattleSequence.Count == currentIndex + 1) // fix an index bug
        //{
        //   CurrentTargetedMonster = null; // may fix a weird non-targeting bug with miss text gameobject
        //    SetCurrentMonsterTurn();
        //    return;
        //}

        // if the current monster turn == first monster?
        /*
        if (currentIndex + 1 >= BattleSequence.Count && CurrentMonsterTurn != BattleSequence[0] && CurrentMonsterTurn != null)
        {
            if (currentIndex + 1 == BattleSequence.Count && BattleSequence[BattleSequence.Count - 1].GetComponent<CreateMonster>().monsterActionAvailable)
            {
                currentIndex = BattleSequence.Count - 1;
            }
            else if (BattleSequence[currentIndex].GetComponent<CreateMonster>().monsterActionAvailable == false)
            {
                currentIndex = 0;
                Debug.Log("index set to zero!");
            }
        }
        else if (currentIndex + 1 <= BattleSequence.Count)
        {
            currentIndex++;
        }
        else if (currentIndex == BattleSequence.Count)
        {
            currentIndex = BattleSequence.Count - 1;
        }
        */
        #endregion

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

            // check if any cooldowns need to be updated // toArray fixes on round start poison death
            foreach (GameObject monster in BattleSequence.ToArray())
            {
                monster.GetComponent<CreateMonster>().OnRoundStart();
            }

            // Then call Next Monster
            SetCurrentMonsterTurn();
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

    #region Old Code
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
    #endregion

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

    #region Old Code
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
    #endregion

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

