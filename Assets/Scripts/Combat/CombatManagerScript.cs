using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using System.Threading.Tasks;

public class CombatManagerScript : MonoBehaviour
{
    [Title("Monster Lists")]
    public GameObject[] MonstersInBattle;
    public List<GameObject> BattleSequence;
    public List<GameObject> ListOfAllys;
    public List<GameObject> ListOfEnemies;

    [Title("Monster Positions")]
    public List<GameObject> ListOfAllyPositions;
    public List<GameObject> ListOfEnemyPositions;
    public bool wonBattle;

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

    public string previousSceneName;
    public bool adventureMode = false;
    public bool testAdventureMode = false;
    public bool autoBattle = false;
    public bool battleOver = false;
    public bool targeting = false;

    public GameObject AdventureManagerGameObject;
    public AdventureManager adventureManager;

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
        else if (Input.GetKeyDown(KeyCode.R) && !adventureMode)
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

        // Adventure mode
        if (adventureMode)
        {
            AdventureManagerGameObject = GameObject.FindGameObjectWithTag("GameManager");
            adventureManager = AdventureManagerGameObject.GetComponent<AdventureManager>();

            // initiate allies
            int i = 0;
            foreach(GameObject monsterPos in ListOfAllyPositions)
            {
                if (adventureManager.ListOfAllyBattleMonsters.Count > i)
                {
                    monsterPos.SetActive(true);
                    monsterPos.GetComponent<CreateMonster>().monster = adventureManager.ListOfAllyBattleMonsters[i];
                    monsterPos.GetComponent<CreateMonster>().monster.aiType = Monster.AIType.Ally;
                    monsterPos.GetComponent<CreateMonster>().monster.aiLevel = Monster.AILevel.Player;
                    monsterPos.GetComponent<CreateMonster>().monsterSpeed = (int)adventureManager.ListOfAllyBattleMonsters[i].speed;
                }
                i++;
            }

            // initiate enemies
            i = 0;
            foreach (GameObject monsterPos in ListOfEnemyPositions)
            {
                if (adventureManager.ListOfEnemyBattleMonsters.Count > i)
                {
                    monsterPos.SetActive(true);
                    monsterPos.GetComponent<CreateMonster>().monster = adventureManager.ListOfEnemyBattleMonsters[i];
                    monsterPos.GetComponent<CreateMonster>().monster.aiType = Monster.AIType.Enemy;
                    monsterPos.GetComponent<CreateMonster>().monster.aiLevel = Monster.AILevel.Random;
                    monsterPos.GetComponent<CreateMonster>().monsterSpeed = (int)adventureManager.ListOfEnemyBattleMonsters[i].speed;
                }
                i++;
            }
        }
        else if (testAdventureMode)
        {
            AdventureManagerGameObject = GameObject.FindGameObjectWithTag("GameManager");
            adventureManager = AdventureManagerGameObject.GetComponent<AdventureManager>();

            // initiate allies
            int i = 0;
            foreach (GameObject monsterPos in ListOfAllyPositions)
            {
                if (adventureManager.ListOfAllyBattleMonsters.Count > i)
                {
                    monsterPos.SetActive(true);
                    monsterPos.GetComponent<CreateMonster>().monster = Instantiate(adventureManager.ListOfAllyBattleMonsters[i]);
                    monsterPos.GetComponent<CreateMonster>().monster.monsterIsOwned = true;
                    monsterPos.GetComponent<CreateMonster>().monster.aiType = Monster.AIType.Ally;
                    monsterPos.GetComponent<CreateMonster>().monster.aiLevel = Monster.AILevel.Player;
                    monsterPos.GetComponent<CreateMonster>().monsterSpeed = (int)adventureManager.ListOfAllyBattleMonsters[i].speed;
                }
                i++;
            }

            // initiate enemies
            i = 0;
            foreach (GameObject monsterPos in ListOfEnemyPositions)
            {
                if (adventureManager.ListOfEnemyBattleMonsters.Count > i)
                {
                    monsterPos.SetActive(true);
                    monsterPos.GetComponent<CreateMonster>().monster = Instantiate(adventureManager.ListOfEnemyBattleMonsters[i]);
                    monsterPos.GetComponent<CreateMonster>().monster.monsterIsOwned = true;
                    monsterPos.GetComponent<CreateMonster>().monster.aiType = Monster.AIType.Enemy;
                    monsterPos.GetComponent<CreateMonster>().monster.aiLevel = Monster.AILevel.Random;
                    monsterPos.GetComponent<CreateMonster>().monsterSpeed = (int)adventureManager.ListOfEnemyBattleMonsters[i].speed;
                }
                i++;
            }
        }

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

        enemyAIManager.InformLists(); // Tell the enemy AI manager that all lists have been compiled
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

            // Get text color
            if (monster.aiType == Monster.AIType.Ally)
            {
                uiManager.CombatOrderTextList.text += ($"{monster.aiType} {monster.name} || Speed: <b>{monster.speed}</b>\n");
            }
            else if (monster.aiType == Monster.AIType.Enemy)
            {
                uiManager.CombatOrderTextList.text += ($"<color=red>{monster.aiType} {monster.name}</color> || Speed: <b>{monster.speed}</b>\n");
            }

            if (monsterJoinedBattle)
            {
                CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} has joined the battle!"); // Update Combat Log with all monsters in battle
            }
        }

        // if adventure mode, check adventure modifiers
        if (adventureMode || testAdventureMode)
        {
            foreach (GameObject monsterObj in ListOfAllys)
            {
                Monster monster = monsterObj.GetComponent<CreateMonster>().monster;

                adventureManager.ApplyAdventureModifiers(monster, monsterObj, Monster.AIType.Ally);
                monsterObj.GetComponent<CreateMonster>().UpdateStats(false, null, false);
                monsterObj.GetComponent<CreateMonster>().CheckAdventureEquipment();
            }

            adventureManager.ApplyGameStartAdventureModifiers(Monster.AIType.Ally);

            // Enemy modifiers and equipment
            foreach (GameObject monsterObj in ListOfEnemies)
            {
                Monster monster = monsterObj.GetComponent<CreateMonster>().monster;
                adventureManager.ApplyAdventureModifiers(monster, monsterObj, Monster.AIType.Enemy);
                monsterObj.GetComponent<CreateMonster>().UpdateStats(false, null, false);
                monsterObj.GetComponent<CreateMonster>().CheckAdventureEquipment();
            }

            adventureManager.ApplyGameStartAdventureModifiers(Monster.AIType.Enemy);
        }

        StartCoroutine(IncrementNewRoundIE()); // Initiate the battle
    }

    // This text function returns a color based on ai type
    Color ReturnColor()
    {
        Color color = Color.red;
        return color;
    }

    // This override function sorts the monster battle sequence by speed after round increment
    public void SortMonsterBattleSequence()
    {
        // When no monster are remaining, return
        if (BattleSequence.Count <= 0) return;

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

            // Get text color
            if (monster.aiType == Monster.AIType.Ally)
            {
                uiManager.CombatOrderTextList.text += ($"{monster.aiType} {monster.name} || Speed: <b>{monster.speed}</b>\n"); //Monster {i + 1}:
            }
            else if (monster.aiType == Monster.AIType.Enemy)
            {
                uiManager.CombatOrderTextList.text += ($"<color=red>{monster.aiType} {monster.name}</color> || Speed: <b>{monster.speed}</b>\n");
            }
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
            //Debug.Log("Returned Null");
            return null;
        }

        // At round start in the case of a speed tie
        if (nextFastestMonsterWithActionAvailable != firstMonsterTurn && firstMonsterTurn.GetComponent<CreateMonster>().monsterActionAvailable)
        {
            //Debug.Log("Returned first monster!");
            return firstMonsterTurn;
        }

        return nextFastestMonsterWithActionAvailable;
    }

    // This function sets the monster turn by speed and priority
    public async void SetCurrentMonsterTurn()
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
            // Call Round End Abilities or Modifiers only if the battle is NOT over
            if (!CheckIfBattleOver())
                await CallRoundEndFunctions();

            StartCoroutine(IncrementNewRoundIE());
        }
        else
        {
            InitiateCombat();
            //Debug.Log("Combat!");
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

    // This function is called at round end to apply any round end abilities or adventure modifiers
    async Task<int> CallRoundEndFunctions()
    {
        await Task.Delay(300);

        //// Call ally round end abilities
        //foreach(GameObject monsterObj in ListOfAllys)
        //{
        //    Monster monster = monsterObj.GetComponent<CreateMonster>().monsterReference;
        //    if (monster.monsterAbility.abilityTriggerTime == Ability.AbilityTriggerTime.RoundEnd)
        //    {
        //       monster.monsterAbility.TriggerAbility();
        //    }
        //}

        //// Call enemy round end abilities
        //foreach (GameObject monsterObj in ListOfEnemies)
        //{
        //    Monster monster = monsterObj.GetComponent<CreateMonster>().monsterReference;
        //    if (monster.monsterAbility.abilityTriggerTime == Ability.AbilityTriggerTime.RoundEnd)
        //    {
        //        monster.monsterAbility.TriggerAbility();
        //    }
        //}

        // Call Round End adventure modifiers
        if ((adventureMode || testAdventureMode) && ListOfAllys.Count > 0)
        {
            adventureManager.ApplyRoundEndAdventureModifiers(Monster.AIType.Ally);
            adventureManager.ApplyRoundEndAdventureModifiers(Monster.AIType.Enemy);
        }

        // Once all Round End effects have been called, End Round
        await Task.Delay(500);
        return 1;
    }

    // This function initiates combat and serves to clean up the SetCurrentMonsterTurn function
    public void InitiateCombat()
    {
        uiManager.InitiateMonsterTurnIndicator(CurrentMonsterTurn);
        Monster monster = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;

        if (!battleOver)
        {
            // If enemy, AI mov
            if (monster.aiType == Monster.AIType.Enemy)
            {
                targeting = false;
                monsterTurn = MonsterTurn.EnemyTurn;
                buttonManagerScript.HideAllButtons("All");

                // First check if the monster is stunned or doesn't have an action available
                if (CurrentMonsterTurn.GetComponent<CreateMonster>().monsterIsStunned)
                {
                    CurrentMonsterTurn.GetComponent<CreateMonster>().monsterActionAvailable = false;
                    HUDanimationManager.MonsterCurrentTurnText.text = ($"Enemy {monster.name} is Stunned!");
                    CombatLog.SendMessageToCombatLog($"Enemy {monster.name} is Stunned!");
                    Invoke("NextMonsterTurn", 1f);
                    return;
                }

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
                    // First check if the monster is stunned or doesn't have an action available
                    if (CurrentMonsterTurn.GetComponent<CreateMonster>().monsterIsStunned)
                    {
                        CurrentMonsterTurn.GetComponent<CreateMonster>().monsterActionAvailable = false;
                        HUDanimationManager.MonsterCurrentTurnText.text = ($"Ally {monster.name} is Stunned!");
                        CombatLog.SendMessageToCombatLog($"Ally {monster.name} is Stunned!");
                        Invoke("NextMonsterTurn", 1f);
                        return;
                    }

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

        monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 1.75f, CurrentTargetedMonster.transform.position.z);
        monsterTargeter.SetActive(true);

        //uiManager.EditCombatMessage($"Ally {monster.name} will use {monsterAttackManager.currentMonsterAttack.monsterAttackName} on {targetedMonster.aiType} {targetedMonster.name}!");
        monsterAttackManager.Invoke("UseMonsterAttack", 0.1f);
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
                monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 1.75f, CurrentTargetedMonster.transform.position.z);
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
            monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 1.75f, CurrentTargetedMonster.transform.position.z);
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

    // This function updates the targeter position on daze call or otherwise
    public void UpdateTargeterPosition(GameObject newTarget)
    {
        CurrentTargetedMonster = newTarget;
        monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 1.75f, CurrentTargetedMonster.transform.position.z);
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
    public IEnumerator IncrementNewRound()
    {
        if (!battleOver)
        {
            autoBattle = false;
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

            // call round start Adventure Modifiers
            if (adventureMode || testAdventureMode)
            {
                StartCoroutine(adventureManager.ApplyRoundStartAdventureModifiers(Monster.AIType.Ally));
                StartCoroutine(adventureManager.ApplyRoundStartAdventureModifiers(Monster.AIType.Enemy));
            }

            // check if any cooldowns need to be updated // toArray fixes on round start poison death
            foreach (GameObject monster in BattleSequence.ToArray())
            {
                monster.GetComponent<CreateMonster>().OnRoundStart();
                yield return new WaitForSeconds(0.25f);
            }

            // Sort after monster on round starts are called
            SortMonsterBattleSequence(); // non battle start version

            // Then call Next Monster
            SetCurrentMonsterTurn();
        }
    }

    // Coroutine to to increment round counter after a delay
    IEnumerator IncrementNewRoundIE()
    {
        yield return new WaitForSeconds(.15f);
        StartCoroutine(IncrementNewRound());
    }

    // Coroutine to initialize HUD elements at start of match
    IEnumerator InitializeMatch()
    {
        yield return new WaitForSeconds(0.5f);
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
        Monster monster = monsterToRemove.GetComponent<CreateMonster>().monster;

        switch (allyOrEnemy)
        {
            case Monster.AIType.Ally:
                ListOfAllys.Remove(monsterToRemove);
                Destroy(monsterToRemove);
                uiManager.UpdateMonsterList(ListOfAllys, Monster.AIType.Ally);
                //Debug.Log($"Removed {monsterToRemove.GetComponent<CreateMonster>().monsterReference.name}", this);
                if (adventureMode || testAdventureMode)
                {
                    // Remove monster references from adventure manager
                    adventureManager.ListOfAllyBattleMonsters.Remove(monster);
                    adventureManager.ListOfCurrentMonsters.Remove(monster);

                    // Add ally to list of dead monsters (for potential revival) and remove equipped equipment
                    adventureManager.ListOfAllyDeadMonsters.Add(monster);
                    adventureManager.RemoveMonsterEquipment(monster);

                    adventureManager.playerMonstersLost += 1;
                }
                break;

            case Monster.AIType.Enemy:
                ListOfEnemies.Remove(monsterToRemove);
                Destroy(monsterToRemove);
                uiManager.UpdateMonsterList(ListOfEnemies, Monster.AIType.Enemy);
                //Debug.Log($"Removed {monsterToRemove.GetComponent<CreateMonster>().monsterReference.name}", this);
                if (adventureMode)
                {
                    adventureManager.playerMonstersKilled += 1;
                }
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
        // First check if adventure mode
        if (adventureMode)
        {
            if (ListOfAllys.Count() == 0)
            {
                battleOver = true;
                CurrentMonsterTurn = null;
                targeting = false;

                buttonManagerScript.HideAllButtons("All");

                uiManager.EditCombatMessage("You lose!");
                wonBattle = false;
                Invoke("AdventureBattleOver", 3.0f);
            }
            else if (ListOfEnemies.Count() == 0)
            {
                battleOver = true;
                CurrentMonsterTurn = null;
                targeting = false;

                buttonManagerScript.HideAllButtons("All");

                uiManager.EditCombatMessage("You win!");
                wonBattle = true;

                // Grant all alive allies a bit of exp for winning
                foreach (GameObject monsterObj in ListOfAllys)
                {
                    if (monsterObj != null)
                    {
                        monsterObj.GetComponent<CreateMonster>().GrantExp(Mathf.RoundToInt(.10f * monsterObj.GetComponent<CreateMonster>().monsterReference.monsterExpToNextLevel));
                    }
                }

                Invoke("AdventureBattleOver", 3.0f);
            }

            return;
        }

        // Else, regular battle
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

    public bool CheckIfBattleOver()
    {
        if (ListOfAllys.Count == 0 || ListOfEnemies.Count == 0)
        {
            return true;
        }

        return false;
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

    // Tjis function should call all battle over functions
    public void AdventureBattleOver()
    {
        battleOver = true;
        CurrentMonsterTurn = null;
        targeting = false;

        buttonManagerScript.HideAllButtons("All");

        if (wonBattle)
        {   
            // Clear out stat changes
            foreach(Monster monster in adventureManager.ListOfAllyBattleMonsters.ToList()) // might have to remove this ToList()
            {
                // Only remove non equipment modifiers
                monster.ListOfModifiers = monster.ListOfModifiers.Where(mod => mod.adventureEquipment == true).ToList();

                monster.physicalAttack = monster.cachedPhysicalAttack;
                monster.magicAttack = monster.cachedMagicAttack;

                monster.physicalDefense = monster.cachedPhysicalDefense;
                monster.magicDefense = monster.cachedMagicDefense;

                monster.speed = monster.cachedSpeed;
                monster.evasion = monster.cachedEvasion;
                monster.critChance = monster.cachedCritChance;
                monster.critDamage = monster.cachedCritDamage;

                monster.bonusAccuracy = monster.cachedBonusAccuracy;

                // Clear temporary monster attack effects
                foreach (MonsterAttack attack in monster.ListOfMonsterAttacks.ToList()) // might have to remove this ToList()
                {
                    foreach(AttackEffect effect in attack.ListOfAttackEffects.ToList()) // might have to remove this ToList()
                    {
                        if (effect.attackEffectDuration == AttackEffect.AttackEffectDuration.Temporary)
                        {
                            attack.ListOfAttackEffects.Remove(effect);
                        }
                    }
                }
            }

            adventureManager.ListOfAllyBattleMonsters.Clear();
            adventureManager.ListOfEnemyBattleMonsters.Clear();

            // If boss fight
            if (adventureManager.BossBattle)
            {
                adventureManager.BossDefeated = true;
            }

            SceneManager.LoadScene(previousSceneName);
        }
        else
        {
            adventureManager.adventureFailed = true;
            SceneManager.LoadScene(previousSceneName);
        }
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

