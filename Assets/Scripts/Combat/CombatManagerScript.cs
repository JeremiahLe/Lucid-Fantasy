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

    public List<GameObject> ListOfMonstersToGrantExp;

    [Title("Monster Positions")]
    public List<GameObject> ListOfAllyPositions;
    public List<GameObject> ListOfEnemyPositions;

    [Title("Auto-set Components")]
    public MessageManager CombatLog;
    public HUDAnimationManager HUDanimationManager;
    public ButtonManagerScript buttonManagerScript;
    public EnemyAIManager enemyAIManager;
    public MonsterAttackManager monsterAttackManager;
    public UIManager uiManager;
    public SoundEffectManager soundEffectManager;
    //public CameraController cameraController;

    [Title("Battle Variables")]
    public GameObject firstMonsterTurn;
    public GameObject CurrentMonsterTurn;

    public GameObject CurrentTargetedMonster;
    public GameObject monsterTargeter;

    public Animator CurrentMonsterTurnAnimator;
    public MonsterAttack CurrentMonsterAttack;

    public enum MonsterTurn { AllyTurn, EnemyTurn }
    public MonsterTurn monsterTurn;

    public enum BattleState { BeginBattle, InBattle, WonBattle, LostBattle }
    public BattleState battleState;

    public enum BattleControlType { Manual, Auto }
    public BattleControlType battleControlType;

    public int currentRound = 1;

    public string previousSceneName;
    public bool adventureMode = false;
    public bool testAdventureMode = false;
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
        //Camera.main.GetComponent<CameraController>();

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
                    monsterPos.GetComponent<CreateMonster>().combatManagerObject = gameObject;
                    monsterPos.GetComponent<CreateMonster>().combatManagerScript = this;
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
                    monsterPos.GetComponent<CreateMonster>().combatManagerObject = gameObject;
                    monsterPos.GetComponent<CreateMonster>().combatManagerScript = this;
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
    public async void SortMonsterBattleSequence(bool monsterJoinedBattle)
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
                CreateMonster monsterComponent = monsterObj.GetComponent<CreateMonster>();
                Monster monster = monsterComponent.monsterReference;

                adventureManager.ApplyAdventureModifiers(monster, monsterObj, Monster.AIType.Ally);

                monsterComponent.CheckAdventureEquipment();

                await monsterAttackManager.TriggerAbilityEffects(monster, monsterObj, monster, AttackEffect.EffectTime.GameStart, monsterObj);

                await monsterComponent.UpdateStats(false, null, false, 0);
            }

            adventureManager.ApplyGameStartAdventureModifiers(Monster.AIType.Ally);

            // Enemy modifiers and equipment
            foreach (GameObject monsterObj in ListOfEnemies)
            {
                CreateMonster monsterComponent = monsterObj.GetComponent<CreateMonster>();
                Monster monster = monsterComponent.monsterReference;

                adventureManager.ApplyAdventureModifiers(monster, monsterObj, Monster.AIType.Enemy);

                monsterComponent.CheckAdventureEquipment();

                await monsterAttackManager.TriggerAbilityEffects(monster, monsterObj, monster, AttackEffect.EffectTime.GameStart, monsterObj);

                await monsterComponent.UpdateStats(false, null, false, 0);
            }

            adventureManager.ApplyGameStartAdventureModifiers(Monster.AIType.Enemy);
        }

        SetBattleState(BattleState.InBattle);
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

        //foreach (GameObject monsterGameObject in BattleSequence.ToArray())
        //{
        //    CreateMonster monsterComponent = monsterGameObject.GetComponent<CreateMonster>();
        //    monsterComponent.CheckHealth(true, null);
        //}

        monsterAttackManager.SetMonsterAttackManagerState(MonsterAttackManager.AttackManagerState.PendingAttack);

        if (!CheckMonstersAlive())
            return;

        CurrentTargetedMonster = null;
        CurrentMonsterTurn = MonsterNextTurn();

        if (CurrentMonsterTurn == null)
        {
            // Call Round End Abilities or Modifiers only if the battle is NOT over
            if (battleState == BattleState.InBattle)
                await CallRoundEndFunctions();

            StartCoroutine(IncrementNewRoundIE());
        }
        else
        {
            CreateMonster monsterComponent = CurrentMonsterTurn.GetComponent<CreateMonster>();

            if (monsterComponent.monsterReference.currentSP < monsterComponent.monsterReference.maxSP)
                monsterComponent.ModifySP(1);

            InitiateCombat();
        }

        #region Old Code 
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
    public async Task<int> CallRoundEndFunctions()
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

    //// Helper function to access camera controller coroutine
    public void FocusCamera(GameObject gameObjectToFocusOn)
    {
        Vector3 offset = gameObjectToFocusOn.GetComponent<CreateMonster>().cameraOffset;
        //StartCoroutine(cameraController.FocusOnTarget(gameObjectToFocusOn.transform, offset));
    }

    //// Helper function to access camera controller coroutine
    public void ResetCamera()
    {
        //StartCoroutine(cameraController.ResetPosition());
    }

    // This function initiates combat and serves to clean up the SetCurrentMonsterTurn function
    public void InitiateCombat()
    {
        //ResetCamera();
        uiManager.InitiateMonsterTurnIndicator(CurrentMonsterTurn);
        Monster monster = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;

        if (battleState != BattleState.InBattle)
            return;

        // First check if the monster is stunned or doesn't have an action available
        if (CurrentMonsterTurn.GetComponent<CreateMonster>().listofCurrentStatusEffects.Contains(Modifier.StatusEffectType.Stunned))
        {
            CurrentMonsterTurn.GetComponent<CreateMonster>().monsterActionAvailable = false;
            HUDanimationManager.MonsterCurrentTurnText.text = ($"{monster.aiType} {monster.name} is Stunned!");
            CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} is Stunned!");
            Invoke("NextMonsterTurn", 1f);
            return;
        }

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
            if (battleControlType == BattleControlType.Manual) // manual targeting
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

    // This coroutine acts as an artificial delay for autobattle similar to the enemy AI
    IEnumerator ReturnAllyMoveAndTargetCoroutine(Monster monster)
    {
        yield return new WaitForSeconds(1.5f);
        buttonManagerScript.AssignAttackMoves(monster);

        CurrentMonsterTurnAnimator = CurrentMonsterTurn.GetComponent<Animator>();

        monsterAttackManager.currentMonsterAttack = GetRandomMove();

        AttackTypeTargeting();

        // Auto-battle multi-target missing target adjustment
        if (monsterAttackManager.currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.MultiTarget)
        {
            for (int i = 0; i < monsterAttackManager.currentMonsterAttack.monsterAttackTargetCountNumber; i++)
            {
                monsterAttackManager.ListOfCurrentlyTargetedMonsters.Add(GetRandomTarget(ListOfEnemies));
            }
        }

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
        Monster monster = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;
        MonsterAttack randMove = monster.ListOfMonsterAttacks[Random.Range(0, monster.ListOfMonsterAttacks.Count)];

        while (randMove.monsterAttackSPCost > monster.currentSP)
        {
            randMove = monster.ListOfMonsterAttacks[Random.Range(0, monster.ListOfMonsterAttacks.Count)];
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
      
        //if (randTarget.transform.position != randTarget.GetComponent<CreateMonster>().startingPosition.transform.position)
        //{
        //    randTarget.transform.position = randTarget.GetComponent<CreateMonster>().startingPosition.transform.position;
        //}
        return randTarget;
    }

    // This function gets called when an attack is chosen and the target is required
    public void TargetingEnemyMonsters(bool _targeting, MonsterAttack currentMonsterAttack)
    {
        switch (_targeting)
        {
            case true:
                Debug.Log("Targeting");
                targeting = true;
                AttackTypeTargeting(); // for autoBattle fixme properly
                if (currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.AllTargets)
                {
                    if (CurrentTargetedMonster.GetComponent<CreateMonster>().monster.aiType == Monster.AIType.Enemy)
                    {
                        foreach (GameObject monster in ListOfEnemies)
                        {
                            monsterTargeter = monster.GetComponent<CreateMonster>().monsterTargeterUIGameObject;
                            monsterTargeter.SetActive(true);
                        }
                    }
                    else
                    {
                        foreach (GameObject monster in ListOfAllys)
                        {
                            monsterTargeter = monster.GetComponent<CreateMonster>().monsterTargeterUIGameObject;
                            monsterTargeter.SetActive(true);
                        }
                    }
                    return;
                }

                monsterTargeter = CurrentTargetedMonster.GetComponent<CreateMonster>().monsterTargeterUIGameObject;
                monsterTargeter.SetActive(true);
                //monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 1.75f, CurrentTargetedMonster.transform.position.z);
                break;

            case false:
                Debug.Log("Not Targeting");
                targeting = false;
                foreach (GameObject monster in BattleSequence)
                {
                    monsterTargeter = monster.GetComponent<CreateMonster>().monsterTargeterUIGameObject;
                    monsterTargeter.SetActive(false);
                }
                break;
        }
    }

    // This function cycles through either list of targetable monsters (ally or enemy)
    public void CycleTargets(GameObject newTarget)
    {
        // Only show the targeter UI object if you are currently targeting monsters
        if (!targeting || newTarget == CurrentTargetedMonster)
            return;

        if (newTarget == null)
            return;

        // Set the old targeter UI object off and set the new target
        CurrentTargetedMonster.GetComponent<CreateMonster>().monsterTargeterUIGameObject.SetActive(false);

        CurrentTargetedMonster = newTarget;
        CurrentTargetedMonster.GetComponent<CreateMonster>().monsterTargeterUIGameObject.SetActive(true);

        // If the current monster attack targets all targets, adjust the targeters accordingly
        if (monsterAttackManager.currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.AllTargets)
        {
            // Disable each targeter first
            foreach (GameObject monster in BattleSequence)
            {
                monsterTargeter = monster.GetComponent<CreateMonster>().monsterTargeterUIGameObject;
                monsterTargeter.SetActive(false);
            }

            // If the current targeted monster is an enemy, show all enemy targeters
            if (CurrentTargetedMonster.GetComponent<CreateMonster>().monster.aiType == Monster.AIType.Enemy)
            {
                foreach (GameObject monster in ListOfEnemies)
                {
                    monsterTargeter = monster.GetComponent<CreateMonster>().monsterTargeterUIGameObject;
                    monsterTargeter.SetActive(true);
                }
            }
            else  // Else, show all allied targeters
            {
                foreach (GameObject monster in ListOfAllys)
                {
                    monsterTargeter = monster.GetComponent<CreateMonster>().monsterTargeterUIGameObject;
                    monsterTargeter.SetActive(true);
                }
            }
        }

        // Update the targeted monster combat text
        monsterAttackManager.UpdateCurrentTargetText();

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
        // Disable each targeter first
        foreach (GameObject monster in BattleSequence)
        {
            monsterTargeter = monster.GetComponent<CreateMonster>().monsterTargeterUIGameObject;
            monsterTargeter.SetActive(false);
        }

        CurrentTargetedMonster = newTarget;
        CurrentTargetedMonster.GetComponent<CreateMonster>().monsterTargeterUIGameObject.SetActive(true);
        //monsterTargeter.transform.position = new Vector3(CurrentTargetedMonster.transform.position.x, CurrentTargetedMonster.transform.position.y + 1.75f, CurrentTargetedMonster.transform.position.z);
    }

    public void CheckMonsterLevelUps()
    {
        bool monsterLeveledUp = false;

        foreach(GameObject monster in ListOfAllys)
        {
            CreateMonster monsterComponent = monster.GetComponent<CreateMonster>();
            Monster monsterReference = monsterComponent.monsterReference;

            if (monsterReference.monsterCurrentExp >= monsterReference.monsterExpToNextLevel)
            {
                monsterComponent.Invoke("LevelUp", 0.5f);
                monsterLeveledUp = true;
                return;
            }
        }

        if (!monsterLeveledUp)
            NextMonsterTurn();
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

        Debug.Log("Called Next Monster Turn!", this);

        // Fixes a weird non-targeting bug with miss text gameobject
        CurrentTargetedMonster = null; 

        // Clear Target List
        monsterAttackManager.ListOfCurrentlyTargetedMonsters.Clear();

        // Next monster turn
        SetCurrentMonsterTurn();
    }

    // This function handles all new round calls (status effects, speed adjustments etc.)
    public IEnumerator IncrementNewRound()
    {
        if (battleState == BattleState.InBattle)
        {
            // Reset the battle control
            battleControlType = BattleControlType.Manual;

            // Increment the round counter
            currentRound += 1;
            uiManager.IncrementRoundCount(currentRound);
            CombatLog.SendMessageToCombatLog($"-- Round {currentRound} -- ");

            // Sort the monster battle sequence
            SortMonsterBattleSequence(); 

            // Call All Round Staret
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
        //CheckMonstersAlive();
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
    public bool CheckMonstersAlive()
    {
        if (ListOfAllys.Count() == 0)
        {
            SetBattleState(BattleState.LostBattle);
            return false;
        }
        else if (ListOfEnemies.Count() == 0)
        {
            SetBattleState(BattleState.WonBattle);
            return false;
        }

        return true;
    }

    // This function assigns a new battle state
    public void SetBattleState(BattleState newBattleState)
    {
        battleState = newBattleState;

        switch (newBattleState)
        {
            case (BattleState.LostBattle):
                buttonManagerScript.HideAllButtons("All");
                uiManager.EditCombatMessage("You Lose!");
                BattleOver();
                break;

            case (BattleState.WonBattle):
                buttonManagerScript.HideAllButtons("All");
                uiManager.EditCombatMessage("You Win!");
                BattleOver();
                break;

            case (BattleState.InBattle):
                break;

            case (BattleState.BeginBattle):
                break;

            default:
                Debug.Log("Missing Battle State referenece?", this);
                break;
        }
    }

    // Begin Auto-Battle
    public void BeginAutoBattle()
    {
        battleControlType = BattleControlType.Auto;
        buttonManagerScript.HideAllButtons("All");
        InitiateCombat();
    }

    // This function should call all battle over functions
    public void BattleOver()
    {
        if (adventureMode)
        {
            Invoke("AdventureBattleOver", 3.0f);
            return;
        }

        Invoke("RestartBattleScene", 3.0f);
    }

    // This function should call all battle over functions
    public void AdventureBattleOver()
    {
        if (battleState == BattleState.LostBattle)
        {
            adventureManager.adventureFailed = true;
            SceneManager.LoadScene(previousSceneName);
            return;
        }

        // Clear out stat changes
        foreach (Monster monster in adventureManager.ListOfAllyBattleMonsters.ToList()) // might have to remove this ToList()
        {
            // Only remove non equipment modifiers
            monster.ListOfModifiers = monster.ListOfModifiers.Where(mod => mod.modifierType == Modifier.ModifierType.equipmentModifier).ToList();

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
                foreach (AttackEffect effect in attack.ListOfAttackEffects.ToList()) // might have to remove this ToList()
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

    // This function resets the battle scene
    public void RestartBattleScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

