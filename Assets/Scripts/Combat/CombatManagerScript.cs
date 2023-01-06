using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using static Modifier;

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

    public bool itemPending = false;

    public GameObject AdventureManagerGameObject;
    public AdventureManager adventureManager;
    public ConsumableWindowScript consumableWindowScript;

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

            adventureManager.ListOfInitialEnemyBattleMonsters.Clear();

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

                    adventureManager.ListOfInitialEnemyBattleMonsters.Add(adventureManager.ListOfEnemyBattleMonsters[i]);
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
                    monsterPos.GetComponent<CreateMonster>().monster.aiLevel = Monster.AILevel.Player;
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
                BattleSequence.Add(monster);
            }
            else if (monster.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Enemy && ListOfEnemies.Contains(monster) != true)
            {
                ListOfEnemies.Add(monster);
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
        uiManager.CombatOrderTextList.text = ($"");

        for (int i = 0; i < BattleSequence.Count; i++)
        {
            Monster monster = BattleSequence[i].GetComponent<CreateMonster>().monsterReference;

            // Get text color
            if (monster.aiType == Monster.AIType.Ally)
            {
                //uiManager.CombatOrderTextList.text += ($"{monster.aiType} {monster.name} || Speed: <b>{monster.speed}</b>\n");
                monster.cachedHealthAtBattleStart = monster.health;
            }
            else if (monster.aiType == Monster.AIType.Enemy)
            {
                //uiManager.CombatOrderTextList.text += ($"<color=red>{monster.aiType} {monster.name}</color> || Speed: <b>{monster.speed}</b>\n");
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

                //adventureManager.ApplyAdventureModifiers(monster, monsterObj, Monster.AIType.Ally);

                //monsterComponent.CheckAdventureEquipment();

                await monsterAttackManager.TriggerAbilityEffects(monster, monsterObj, monster, AttackEffect.EffectTime.GameStart, monsterObj);

                await monsterComponent.UpdateStats(false, null, false, 0);
            }

            //adventureManager.ApplyGameStartAdventureModifiers(Monster.AIType.Ally);

            // Enemy modifiers and equipment
            foreach (GameObject monsterObj in ListOfEnemies)
            {
                CreateMonster monsterComponent = monsterObj.GetComponent<CreateMonster>();
                Monster monster = monsterComponent.monsterReference;

                //adventureManager.ApplyAdventureModifiers(monster, monsterObj, Monster.AIType.Enemy);

                //monsterComponent.CheckAdventureEquipment();

                await monsterAttackManager.TriggerAbilityEffects(monster, monsterObj, monster, AttackEffect.EffectTime.GameStart, monsterObj);

                await monsterComponent.UpdateStats(false, null, false, 0);
            }

            //adventureManager.ApplyGameStartAdventureModifiers(Monster.AIType.Enemy);
        }

        SetBattleState(BattleState.InBattle);
        StartCoroutine(IncrementNewRoundIE()); // Initiate the battle
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
        if (!CheckMonstersAlive())
            return;

        CurrentTargetedMonster = null;

        monsterAttackManager.ListOfCurrentlyTargetedMonsters.Clear();

        uiManager.ClearAllTargeters();

        CurrentMonsterTurn = MonsterNextTurn();

        if (CurrentMonsterTurn == null)
        {
            uiManager.ClearCombatMessage();

            // Call Round End Abilities or Modifiers only if the battle is NOT over
            if (battleState == BattleState.InBattle)
                await CallRoundEndFunctions();

            if (!CheckMonstersAlive())
                return;

            StartCoroutine(IncrementNewRoundIE());
        }
        else
        {
            CreateMonster monsterComponent = CurrentMonsterTurn.GetComponent<CreateMonster>();

            if (monsterComponent.monsterReference.currentSP < monsterComponent.monsterReference.maxSP)
                monsterComponent.ModifySP(monsterComponent.monsterReference.spRegen);

            InitiateCombat();
        }
    }

    // This function is called at round end to apply any round end abilities or adventure modifiers
    public async Task<int> CallRoundEndFunctions()
    {
        // Call Round End adventure modifiers
        if ((adventureMode || testAdventureMode) && ListOfAllys.Count > 0)
        {
            adventureManager.ApplyRoundEndAdventureModifiers(Monster.AIType.Ally); // needs to be awaited
            adventureManager.ApplyRoundEndAdventureModifiers(Monster.AIType.Enemy); // needs to be awaited
        }

        // check if any cooldowns need to be updated // toArray fixes on round start poison death
        foreach (GameObject monster in BattleSequence.ToArray())
        {
            if (monster == null)
                continue;

            if (monster.TryGetComponent(out CreateMonster monsterComponent))
            {
                await monsterComponent.OnRoundEnd();
            }

            await Task.Delay(150);
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

    public bool CheckMonsterIsStunned(CreateMonster monsterComponent)
    {
        // First check if the monster is stunned or doesn't have an action available
        if (monsterComponent.listofCurrentStatusEffects.Contains(StatusEffectType.Stunned))
        {
            Monster monster = monsterComponent.monsterReference;

            monsterComponent.monsterActionAvailable = false;

            HUDanimationManager.MonsterCurrentTurnText.text = ($"{monster.aiType} {monster.name} is Stunned!");

            CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} is Stunned!");

            Invoke(nameof(SetCurrentMonsterTurn), 1f);

            return true;
        }

        return false;
    }

    public async Task<int> CheckEnragedTargetMissing(GameObject monsterObj)
    {
        CreateMonster monsterComponent = monsterObj.GetComponent<CreateMonster>();

        if (monsterComponent.listofCurrentStatusEffects.Contains(StatusEffectType.Enraged))
        {
            if (monsterComponent.monsterEnragedTarget == null || monsterComponent.monsterEnragedTarget.TryGetComponent(out CreateMonster createMonster) == false || monsterComponent.monsterEnragedTarget.GetComponent<CreateMonster>().monsterReference.health <= 0)
            {
                Monster monster = monsterComponent.monsterReference;

                Modifier modifier = monster.ListOfModifiers.Find(modifier => modifier.statusEffectType == StatusEffectType.Enraged);

                await modifier.ResetModifiedStat(monster, monsterObj);

                monster.ListOfModifiers.Remove(modifier);
            }
        }

        await Task.Delay(150);
        return 1;
    }

    public async Task<int> CheckOnMonsterDeathEvents()
    {
        foreach (GameObject monsterObj in BattleSequence.ToArray())
        {
            if (monsterObj == null)
                continue;

            if (!monsterObj.TryGetComponent(out CreateMonster createMonster))
                continue;

            await CheckEnragedTargetMissing(monsterObj);

            await Task.Delay(150);
        }

        await Task.Delay(150);
        return 1;
    }

    // This function initiates combat and serves to clean up the SetCurrentMonsterTurn function
    public async void InitiateCombat()
    {
        // ResetCamera();

        if (battleState != BattleState.InBattle)
            return;

        CreateMonster monsterComponent = CurrentMonsterTurn.GetComponent<CreateMonster>();

        Monster monster = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;

        uiManager.InitiateMonsterTurnIndicator(CurrentMonsterTurn);

        CurrentMonsterTurnAnimator = CurrentMonsterTurn.GetComponent<Animator>();

        if (CheckMonsterIsStunned(monsterComponent))
            return;

        await CheckEnragedTargetMissing(CurrentMonsterTurn);

        // If enemy, AI move
        if (monster.aiType == Monster.AIType.Enemy && monster.aiLevel != Monster.AILevel.Player)
        {
            targeting = false;
            monsterTurn = MonsterTurn.EnemyTurn;
            buttonManagerScript.HideAllButtons("All");
            HUDanimationManager.MonsterCurrentTurnText.text = ($"Enemy {monster.name} turn...");
            enemyAIManager.Invoke(nameof(enemyAIManager.SelectMonsterAttackByAILevel), 1.7f);
        }
        // If ally, give player move
        else /*if (monster.aiType == Monster.AIType.Ally)*/
        {
            if (battleControlType == BattleControlType.Manual) // manual targeting
            {
                monsterTurn = MonsterTurn.AllyTurn;
                buttonManagerScript.ListOfMonsterAttacks.Clear();
                HUDanimationManager.MonsterCurrentTurnText.text = ($"What will {monster.name} do?");
                buttonManagerScript.AssignAttackMoves(monster);
                buttonManagerScript.ResetHUD();
            }
            else // auto battle - TODO - make auto battle less random!!
            {
                monsterTurn = MonsterTurn.AllyTurn;
                buttonManagerScript.ListOfMonsterAttacks.Clear();
                HUDanimationManager.MonsterCurrentTurnText.text = ($"{monster.aiType} {monster.name} turn...");
                enemyAIManager.Invoke(nameof(enemyAIManager.SelectMonsterAttackByAILevel), 1.7f);
            }       
        }
    }

    public void PassTurn()
    {
        CreateMonster monsterComponent = CurrentMonsterTurn.GetComponent<CreateMonster>();
        Monster currentMonsterTurn = monsterComponent.monsterReference;

        CombatLog.SendMessageToCombatLog($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} passed!");
        uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} passed!");

        monsterComponent.monsterActionAvailable = false;
        Invoke(nameof(SetCurrentMonsterTurn), 1.0f);
    }

    public void StanceChanged()
    {
        CreateMonster monsterComponent = CurrentMonsterTurn.GetComponent<CreateMonster>();
        Monster currentMonsterTurn = monsterComponent.monsterReference;

        CombatLog.SendMessageToCombatLog($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} has changed to {monsterComponent.monsterStance} stance!");
        uiManager.EditCombatMessage($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} has changed to {monsterComponent.monsterStance} stance!");

        monsterComponent.monsterActionAvailable = false;
        Invoke(nameof(SetCurrentMonsterTurn), 1.0f);
    }

    public void ReturnFromItems()
    {
        targeting = false;
        itemPending = false;

        buttonManagerScript.ResetHUD();
    }

    public void ItemUsed()
    {
        CreateMonster monsterComponent = CurrentMonsterTurn.GetComponent<CreateMonster>();

        monsterComponent.monsterActionAvailable = false;
        Invoke(nameof(SetCurrentMonsterTurn), 1.0f);
    }

    public void QueueUsableItem(Item currentItem)
    {
        itemPending = true;
        targeting = true;

        Monster currentMonster = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;

        buttonManagerScript.HideAllButtons("All");

        uiManager.EditCombatMessage($"{currentMonster.aiType} {currentMonster.name} will use {currentItem.itemName} on {currentMonster.aiType} {currentMonster.name}?");
    }

    public void UseItem(GameObject targetMonsterGameObject)
    {
        itemPending = false;
        targeting = false;
        buttonManagerScript.HideButton(buttonManagerScript.ReturnFromItemButton);

        Monster currentMonster = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;
        Monster targetMonster = targetMonsterGameObject.GetComponent<CreateMonster>().monsterReference;
        Item currentItem = uiManager.ConsumableWindow.GetComponent<ConsumableWindowScript>().GetCurrentItem();

        uiManager.EditCombatMessage($"{currentMonster.aiType} {currentMonster.name} used {currentItem.itemName} on {targetMonster.aiType} {targetMonster.name}!");
        CombatLog.SendMessageToCombatLog($"{currentMonster.aiType} {currentMonster.name} used {currentItem.itemName} on {targetMonster.aiType} {targetMonster.name}!");

        TriggerItemEffects();
    }

    public async void TriggerItemEffects()
    {
        Item currentItem = uiManager.ConsumableWindow.GetComponent<ConsumableWindowScript>().GetCurrentItem();

        foreach (IAbilityTrigger abilityTrigger in currentItem.listOfItemEffectTriggers)
        {
            await Task.Delay(150);

            await abilityTrigger.TriggerItem(currentItem, this);
        }

        uiManager.ConsumableWindow.GetComponent<ConsumableWindowScript>().SetCurrentItem(null);

        adventureManager.ListOfInventoryItems.Remove(currentItem);

        ItemUsed();
    }

    // This function returns a random move from the monsters list of monster attacks
    public MonsterAttack GetRandomMove()
    {
        Monster monster = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;
        List<MonsterAttack> tempList = monster.ListOfMonsterAttacks;

        if (CurrentMonsterTurn.GetComponent<CreateMonster>().listofCurrentStatusEffects.Contains(StatusEffectType.Enraged))
        {
            tempList = monster.ListOfMonsterAttacks.Where(Attack => Attack.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.EnemyTarget || Attack.monsterAttackTargetType == MonsterAttack.MonsterAttackTargetType.Any).ToList();
        }

        if (tempList.Count == 0)
            return null;

        if (tempList.Where(attack => attack.monsterAttackSPCost > monster.currentSP).ToList().Count == 4)
        {
            return null;
        }

        MonsterAttack randMove = monster.ListOfMonsterAttacks[Random.Range(0, tempList.Count)];

        while (randMove.monsterAttackSPCost > monster.currentSP)
        {
            randMove = monster.ListOfMonsterAttacks[Random.Range(0, tempList.Count)];
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
                uiManager.GetCurrentlyTargetedMonsters();
                break;

            case false:
                Debug.Log("Not Targeting");
                targeting = false;
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

        CurrentTargetedMonster = newTarget;

        if (itemPending)
        {
            Monster currentMonster = CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference;
            Monster targetMonster = CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
            uiManager.EditCombatMessage($"{currentMonster.aiType} {currentMonster.name} will use {uiManager.ConsumableWindow.GetComponent<ConsumableWindowScript>().GetCurrentItem().itemName} on {targetMonster.aiType} {targetMonster.name}?");
            return;
        }

        monsterAttackManager.UpdateCurrentTargetText();
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
                monsterComponent.Invoke(nameof(monsterComponent.LevelUp), 0.5f);
                monsterLeveledUp = true;
                return;
            }
        }

        if (battleState == BattleState.WonBattle || battleState == BattleState.LostBattle)
        {
            StartCoroutine(uiManager.ShowBattleResultsScreen(battleState));
            Debug.Log($"No more level ups! Showing Battle Results Screen! {battleState}");
            return;
        }

        if (!monsterLeveledUp && battleState == BattleState.InBattle)
        {
            SetCurrentMonsterTurn();
            Debug.Log("No more level ups! Returning!");
            return;
        }
    }


    // This function handles all new round calls (status effects, speed adjustments etc.)
    public async void IncrementNewRound()
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
                if (monster == null)
                    continue;

                if (monster.TryGetComponent(out CreateMonster monsterComponent))
                {
                    await monsterComponent.OnRoundStart();
                }

                await Task.Delay(75);
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
        yield return new WaitForSeconds(.10f);
        IncrementNewRound();
    }

    // Coroutine to initialize HUD elements at start of match
    IEnumerator InitializeMatch()
    {
        yield return new WaitForSeconds(0.5f);
        GetAllMonstersInBattle();
    }

    // This function removes a monster from a list
    public void RemoveMonsterFromList(GameObject monsterToRemove, Monster.AIType allyOrEnemy)
    {
        Monster monster = monsterToRemove.GetComponent<CreateMonster>().monster;

        switch (allyOrEnemy)
        {
            case Monster.AIType.Ally:
                ListOfAllys.Remove(monsterToRemove);

                Destroy(monsterToRemove, 1f);

                if (adventureMode || testAdventureMode)
                {
                    // Remove monster references from adventure manager
                    adventureManager.ListOfAllyBattleMonsters.Remove(monster);
                    adventureManager.ListOfCurrentMonsters.Remove(monster);

                    // Add ally to list of dead monsters (for potential revival) and remove equipped equipment
                    adventureManager.ListOfAllyDeadMonsters.Add(monster);
                    //adventureManager.RemoveMonsterEquipment(monster);

                    adventureManager.playerMonstersLost += 1;
                }
                break;

            case Monster.AIType.Enemy:
                ListOfEnemies.Remove(monsterToRemove);

                Destroy(monsterToRemove, 1f);

                if (adventureMode)
                    adventureManager.playerMonstersKilled += 1;

                break;

            default:
                Debug.Log("Missing monster object or AI type reference?", this);
                break;
        }

        BattleSequence.Remove(monsterToRemove);

        SortMonsterBattleSequence();
    }

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
                uiManager.ClearCombatMessage();
                BattleOver();
                break;

            case (BattleState.WonBattle):
                buttonManagerScript.HideAllButtons("All");
                uiManager.ClearCombatMessage();
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
        if (adventureMode || testAdventureMode)
        {
            Invoke(nameof(AdventureBattleOver), 0.5f);
            return;
        }

        Invoke(nameof(RestartBattleScene), 3.0f);
    }

    // This function should call all battle over functions
    public void AdventureBattleOver()
    {
        //if (battleState == BattleState.LostBattle)
        //{
        //    adventureManager.adventureFailed = true;
        //    SceneManager.LoadScene(previousSceneName);
        //    return;
        //}

        // Clear out stat changes
        if (!testAdventureMode)
        {
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
        }

        adventureManager.ListOfAllyBattleMonsters.Clear();

        adventureManager.ListOfEnemyBattleMonsters.Clear();
        
        foreach (GameObject allyMonster in ListOfAllys)
        {
            CreateMonster monsterComponent = allyMonster.GetComponent<CreateMonster>();
            Monster monsterReference = monsterComponent.monsterReference;
            monsterComponent.GrantExp(Mathf.RoundToInt(.10f * monsterReference.monsterExpToNextLevel));
        }

        CheckMonsterLevelUps();
    }

    // This function resets the battle scene
    public void RestartBattleScene()
    {
        if (adventureManager.playerRetrys > 0)
        {
            adventureManager.playerRetrys--;
        }

        adventureManager.RetryBattle();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToAdventureScene()
    {
        SceneManager.LoadScene(previousSceneName);
    }
}

