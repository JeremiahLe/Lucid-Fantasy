using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using System.Threading.Tasks;
using static Monster;

public class AdventureManager : MonoBehaviour
{
    [Title("Helper Functions")]
    [Button(50)]
    public void AddRerolls()
    {
        rerollAmount += 100;
        subscreenManager.rerollsLeftText.text = ($"Rerolls left: {rerollAmount}");
    }

    [Button(50)]
    public void AddRandomEquipment()
    {
        for (int i = 0; i < 16; i++)
        {
            ListOfCurrentEquipment.Add(subscreenManager.GetRandomEquipment());
            ResetEquipmentList();
        }
    }

    [Button(50)]
    public void AddRandomItems()
    {
        for (int i = 0; i < 16; i++)
        {
            ListOfInventoryItems.Add(GetRandomItemByRarity());
            //ResetItemList();
        }
    }

    [Button(50)]
    public void LevelUpMonsters()
    {
        foreach (Monster monster in ListOfCurrentMonsters)
        {
            CreateMonster createMonster = new CreateMonster();
            createMonster.monsterReference = monster;
            createMonster.monster = monster;

            while (monster.level < 20)
            {
                createMonster.LevelUpOutsideOfCombat();
            }
        }
    }

    [Button(50)]
    public void AddRandomMonsters()
    {
        for (int i = 0; i < 4; i++)
        {
            ListOfCurrentMonsters.Add(subscreenManager.GetRandomMonster());
            ListOfCurrentMonsters[i].monsterIsOwned = true;
        }
    }

    [Title("Bools")]
    public bool adventureBegin = false;
    public bool AdventureMode = false;
    public bool adventureFailed = false;
    public bool BossDefeated = false;
    public bool BossBattle = false;
    public bool lockEquipmentInCombat = false;

    [Title("Pre-Adventure Components")]
    public static GameObject thisManager;
    public GameObject ConfirmAdventureMenu;
    public SceneButtonManager sceneButtonManager;

    [Title("SFX")]
    public AudioSource GameManagerAudioSource;
    public AudioSource GameManagerSFXSource;

    public AudioClip monsterAscendedSFX;
    public AudioClip UIHoverSFX;
    public AudioClip equipmentSelectSFX;
    public AudioClip equipmentDeselectSFX;
    public AudioClip UIScreenSelectSFX;

    [Title("BGM")]
    public AudioClip adventureBGM;
    public AudioClip combatBGM;
    public AudioClip bossBGM;

    public AudioClip winBGM;
    public AudioClip defeatBGM;

    [Title("Adventure Components")]
    public float adventureTimer;
    public bool timeStarted = false;

    public GameObject nodeSelectionTargeter;
    public string adventureSceneName;
    public Adventure currentSelectedAdventure;

    public GameObject currentSelectedNode;
    public GameObject cachedSelectedNode;
    public CreateNode NodeComponent;
    public Monster adventureBoss;

    public GameObject AdventureMenu;
    public GameObject SubscreenMenu;
    public GameObject MonstersSubscreen;

    public TextMeshProUGUI routeText;
    public TextMeshProUGUI subScreenMenuText;

    public SubscreenManager subscreenManager;
    public MonstersSubScreenManager monstersSubScreenManager;

    public GameObject RewardSlotOne;
    public GameObject RewardSlotTwo;
    public GameObject RewardSlotThree;

    [Title("Adventure - Modifier Passive Effects")]
    public float bonusExp = 1;
    public float legendaryChanceRate = 1f;
    public float rareChanceRate = 1f;
    public float uncommonChanceRate = 1f;
    public float commonChanceRate = 100f;

    [Title("Adventure - Player Components")]
    public int playerMonsterLimit = 4;

    public int rerollAmount = 0;
    public int timesRerolled = 0;

    public int playerGold = 0;
    public int playerGoldSpent = 0;

    public int playerMonstersLost = 0;
    public int playerMonstersKilled = 0;

    public int adventureNGNumber = 1;
    public int playerRetrys = 1;

    public List<Monster> ListOfCurrentMonsters;

    public List<Modifier> ListOfCurrentModifiers;

    public List<Monster> ListOfAllMonsters;

    public List<Modifier> ListOfCurrentEquipment;

    public List<Item> ListOfInventoryItems;

    public List<Monster> ListOfAllyDeadMonsters;

    [Title("Other Adventure Modules")]
    public enum RewardType { Monster, Modifier, Equipment }
    public RewardType currentRewardType;

    public Monster currentHoveredRewardMonster;
    public Modifier currentHoveredRewardModifier;

    public Modifier currentSelectedEquipment;
    public Monster currentSelectedMonsterForEquipment;

    [Title("Nodes")]
    public GameObject dontDisappear;
    public GameObject NodeToReturnTo;

    public GameObject[] ListOfAllNodes;
    public GameObject[] ListOfSavedNodes;

    [Title("Rewards")]
    public List<Monster> ListOfAvailableRewardMonsters;
    public List<Modifier> ListOfAvailableRewardModifiers;
    public List<Modifier> ListOfAvailableRewardEquipment;
    public List<Item> ListOfAvailableItems;

    public List<Modifier> DefaultListOfAvailableRewardModifiers;
    public List<Modifier> DefaultListOfAvailableRewardEquipment;
    public List<Item> DefaultListOfAvailableItems;

    [Title("Pre-Battle Setup")]
    public int randomBattleMonsterCount;
    public int randomBattleMonsterLimit;

    [Title("Adventure - Battle Setup and Components")]
    public GameObject CombatManagerObject;

    public CombatManagerScript combatManagerScript;

    public List<Monster> ListOfInitialEnemyBattleMonsters;

    public List<Monster> ListOfEnemyBattleMonsters;

    public List<Monster> ListOfAllyBattleMonsters;

    public List<Modifier> ListOfEnemyModifiers;

    public void Start()
    {
    }

    public void Update()
    {
        if (timeStarted == true)
        {
            adventureTimer += Time.deltaTime;
        }
    }

    public void Awake()
    {
        thisManager = gameObject;
        
        sceneButtonManager = GetComponent<SceneButtonManager>();
        //GameManagerAudioSource = GetComponent<AudioSource>();
        adventureBegin = false;
        CopyDefaultModifierList();
        CopyDefaultEquipmentList();
        CopyDefaultItemList();

        //
        GameObject[] managers = GameObject.FindGameObjectsWithTag("GameManager");
        int numManagers = managers.Length;
        if (numManagers > 1)
        {
            foreach (GameObject manager in managers)
            {
                if (manager != thisManager)
                {
                    Destroy(manager);
                }
            }
        }
    }

    // This function copies the modifier list to modify it
    public void CopyDefaultModifierList()
    {
        // Copy list
        foreach (var item in ListOfAvailableRewardModifiers)
        {
            DefaultListOfAvailableRewardModifiers.Add(new Modifier
            {
                modifierName = item.modifierName,
                modifierType = item.modifierType,
                modifierAmount = item.modifierAmount,
                modifierDescription = item.modifierDescription,
                modifierCurrentDuration = item.modifierCurrentDuration,
                modifierDuration = item.modifierDuration,
                modifierDurationType = item.modifierDurationType,
                modifierRarity = item.modifierRarity,
                modifierSource = item.modifierSource,
                statModified = item.statModified,
                baseSprite = item.baseSprite,
                isStatusEffect = item.isStatusEffect,
                statusEffectType = item.statusEffectType,
                statChangeType = item.statChangeType,
                listOfModifierTriggers = item.listOfModifierTriggers
            });
        }
    }

    // This function copies the equipment list to modify it
    public void CopyDefaultEquipmentList()
    {
        // Copy list
        foreach (var item in ListOfAvailableRewardEquipment)
        {
            DefaultListOfAvailableRewardEquipment.Add(new Modifier
            {
                modifierName = item.modifierName,
                modifierType = item.modifierType,
                modifierAmount = item.modifierAmount,
                modifierDescription = item.modifierDescription,
                modifierCurrentDuration = item.modifierCurrentDuration,
                modifierDuration = item.modifierDuration,
                modifierDurationType = item.modifierDurationType,
                modifierRarity = item.modifierRarity,
                modifierSource = item.modifierSource,
                statModified = item.statModified,
                baseSprite = item.baseSprite,
                modifierAmountFlatBuff = item.modifierAmountFlatBuff,
            });
        }
    }

    // This function copies the item list to modify it
    public void CopyDefaultItemList()
    {
        // Copy list
        foreach (var item in ListOfAvailableItems)
        {
            DefaultListOfAvailableItems.Add(new Item
            {
                itemName = item.itemName,
                itemDescription = item.itemDescription,
                baseSprite = item.baseSprite,
                itemRarity = item.itemRarity,
                itemType = item.itemType
            });
        }
    }

    // This function resets the currently displayed modifier list upon reroll to prevent duplicates
    public void ResetModifierList()
    {
        ListOfAvailableRewardModifiers.Clear();

        // Copy list
        foreach (var item in DefaultListOfAvailableRewardModifiers)
        {
            ListOfAvailableRewardModifiers.Add(new Modifier
            {
                modifierName = item.modifierName,
                modifierType = item.modifierType,
                modifierAmount = item.modifierAmount,
                modifierDescription = item.modifierDescription,
                modifierCurrentDuration = item.modifierCurrentDuration,
                modifierDuration = item.modifierDuration,
                modifierDurationType = item.modifierDurationType,
                modifierRarity = item.modifierRarity,
                modifierSource = item.modifierSource,
                statModified = item.statModified,
                baseSprite = item.baseSprite,
                isStatusEffect = item.isStatusEffect,
                statusEffectType = item.statusEffectType,
                statChangeType = item.statChangeType,
                listOfModifierTriggers = item.listOfModifierTriggers
            });
        }
    }

    // This function resets the currently displayed equipment list upon reroll to prevent duplicates
    public void ResetEquipmentList()
    {
        ListOfAvailableRewardEquipment.Clear();

        // Copy list
        foreach (var item in DefaultListOfAvailableRewardEquipment)
        {
            ListOfAvailableRewardEquipment.Add(new Modifier
            {
                modifierName = item.modifierName,
                modifierType = item.modifierType,
                modifierAmount = item.modifierAmount,
                modifierDescription = item.modifierDescription,
                modifierCurrentDuration = item.modifierCurrentDuration,
                modifierDuration = item.modifierDuration,
                modifierDurationType = item.modifierDurationType,
                modifierRarity = item.modifierRarity,
                modifierSource = item.modifierSource,
                modifierAmountFlatBuff = item.modifierAmountFlatBuff,
                statModified = item.statModified,
                baseSprite = item.baseSprite
            });
        }
    }

    // This function resets the currently displayed item list upon reroll to prevent duplicates
    public void ResetItemList()
    {
        ListOfAvailableItems.Clear();

        // Copy list
        foreach (var item in DefaultListOfAvailableItems)
        {
            ListOfAvailableItems.Add(new Item
            {
                itemName = item.itemName,
                itemDescription = item.itemDescription,
                baseSprite = item.baseSprite,
                itemRarity = item.itemRarity,
                itemType = item.itemType,
                listOfItemEffectTriggers = item.listOfItemEffectTriggers
            });
        }
    }

    // This function enables the confirmAdventureButton after selected adventure
    public void EnableConfirmAdventureButton(Adventure adventureReference)
    {
        ConfirmAdventureMenu.SetActive(true);
        currentSelectedAdventure = adventureReference;
    }

    // This function enables the confirmAdventureButton after selected adventure
    public void DisableConfirmAdventureButton()
    {
        ConfirmAdventureMenu.SetActive(false);
        currentSelectedAdventure = null;
    }

    // This function handles the saving of nodes before going to the battle scene
    public void GoToBattleScene()
    {
        DontDestroyOnLoad(gameObject);

        // save nodes
        if (ListOfSavedNodes.Count() == 0 || ListOfSavedNodes[0] == null)
        {
            foreach (GameObject node in ListOfAllNodes)
            {
                node.GetComponent<CreateNode>().nodeInDefaultState = false;
                node.SetActive(false);
                DontDestroyOnLoad(node);
            }

            // Fix missing saved nodes bug on NG+
            ListOfSavedNodes = ListOfAllNodes;
        }

        // Fix missing saved nodes bug on NG+
        //ListOfSavedNodes = ListOfAllNodes; // NVM, this breaks everything

        foreach (GameObject node in ListOfSavedNodes)
        {
            node.SetActive(false);
        }

        NodeToReturnTo = cachedSelectedNode;

        if (NodeToReturnTo.GetComponent<CreateNode>().nodeType == CreateNode.NodeType.Boss)
        {
            BossBattle = true;
        }

        lockEquipmentInCombat = true;

        SceneManager.LoadScene("AdventureBattleScene");
    }

    // This function goes to selected adventure scene
    public void GoToAdventureScene()
    {
        switch (currentSelectedAdventure.adventureName)
        {
            case "Basic Adventure":
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
                adventureBegin = false;
                sceneButtonManager.GoToSceneCoroutine("BasicAdventureScene");
                break;

            case "Basic Adventure Hard":
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
                sceneButtonManager.GoToSceneCoroutine("BasicAdventureHardScene");
                break;

            default:
                break;
        }
    }

    // When adventure scene is loaded, get scene data
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case ("StartScreen"):

                // Destroy manager object and clear nodes
                Destroy(gameObject);
                foreach (GameObject node in ListOfAllNodes)
                {
                    Destroy(node);
                }
                foreach (GameObject node in ListOfSavedNodes)
                {
                    Destroy(node);
                }
                break;
                

            case ("AdventureBattleScene"):

                // Get combat scene components
                CombatManagerObject = GameObject.FindGameObjectWithTag("GameController");
                combatManagerScript = CombatManagerObject.GetComponent<CombatManagerScript>();
                combatManagerScript.adventureMode = true;
                combatManagerScript.previousSceneName = adventureSceneName;

                // Boss Music
                if (BossBattle)
                {
                    PlayNewBGM(bossBGM, .040f);
                    return;
                }

                // Combat Music
                PlayNewBGM(combatBGM, .040f);
                break;

            default:
                InitiateSceneData();
                break;
        }
    }

    // This function is called whenever an out of combat passive modifier is obtained
    public async void ApplyPassiveModifiers(Modifier modifier)
    {
        foreach (IAbilityTrigger modifierTrigger in modifier.listOfModifierTriggers)
        {
            if (modifierTrigger.abilityTriggerTime != AttackEffect.EffectTime.OutOfCombatPassive)
                continue;

            await modifierTrigger.TriggerModifier(this);
        }
    }

    public async Task<int> TriggerAdventureModifiers(AttackEffect.EffectTime triggerTime, AIType aIType)
    {
        List<Modifier> listOfAdventureModifiers;

        if (aIType == Monster.AIType.Ally)
            listOfAdventureModifiers = ListOfCurrentModifiers;
        else
            listOfAdventureModifiers = ListOfEnemyModifiers;

        foreach (Modifier modifier in listOfAdventureModifiers)
        {
            int i = 0;
            foreach (IAbilityTrigger modifierTrigger in modifier.listOfModifierTriggers)
            {
                if (modifierTrigger.abilityTriggerTime == triggerTime)
                {
                    Debug.Log($"Triggering {aIType} {modifier.modifierName}!");
                    
                    await modifier.listOfModifierTriggers[i].TriggerModifier(combatManagerScript, aIType, triggerTime);
                   
                    await Task.Delay(300);
                }

                i++;
            }
        }

        return 1;
    }

    // This function is called as soon as the adventure scene is loaded
    public void InitiateSceneData()
    {
        // Get important components
        timeStarted = true;
        adventureSceneName = SceneManager.GetActiveScene().name;
        AdventureMenu = GameObject.FindGameObjectWithTag("AdventureMenu");
        routeText = AdventureMenu.GetComponentInChildren<TextMeshProUGUI>();
        SubscreenMenu = GameObject.FindGameObjectWithTag("SubscreenMenu");
        lockEquipmentInCombat = false;

        // Find subscreen if still not found
        if (SubscreenMenu == null)
        {
            SubscreenMenu = FindInActiveObjectByTag("SubscreenMenu");
        }

        subscreenManager = SubscreenMenu.GetComponent<SubscreenManager>();
        subScreenMenuText = SubscreenMenu.GetComponentInChildren<TextMeshProUGUI>();
        monstersSubScreenManager = subscreenManager.monstersSubScreenManager;

        nodeSelectionTargeter = GameObject.FindGameObjectWithTag("Targeter");

        // Once all components are accounted for, call other inits
        InitiateAdventureData();
        InitiateNodes();
    }

    // This function handles which nodes should be disabled/enabled
    public void InitiateNodes()
    {
        Debug.Log("InitiateNodes got called!");

        // Get all nodes in scene at adventure start
        ListOfAllNodes = GameObject.FindGameObjectsWithTag("Node");

        foreach (GameObject node in ListOfAllNodes)
        {
            // Get node component and apply targeter component etc.
            node.SetActive(true);
            CreateNode nodeComponent = node.GetComponent<CreateNode>();
            nodeComponent.nodeSelectionTargeter = nodeSelectionTargeter;
            nodeComponent.routeText = routeText;
            nodeComponent.GetComponent<Interactable>().interactableDescriptionWindow = subscreenManager.ToolTipWindow;
            nodeComponent.GetComponent<Interactable>().interactableText = subscreenManager.ToolTipWindowTextComponent;

            // If not beginning of adventure, all default nodes should be turned off
            if (nodeComponent.nodeInDefaultState == true && adventureBegin)
            {
                node.SetActive(false);
            }

            // If null, the adventure just started
            if (NodeToReturnTo == null)
            {
                // Set start to unlocked if start of adventure
                if (nodeComponent.nodeType == CreateNode.NodeType.Start)
                {
                    nodeComponent.SetNodeState(CreateNode.NodeState.Unlocked);
                    node.GetComponent<Animator>().SetBool("unlocked", true);

                    // set targeted node to start node
                    Transform selectedPosition = node.GetComponent<CreateNode>().selectedPosition;
                    nodeSelectionTargeter.transform.position = new Vector3(selectedPosition.transform.position.x, selectedPosition.transform.position.y + 1.55f, selectedPosition.transform.position.z);
                    currentSelectedNode = node;
                }
                else
                {
                    nodeComponent.SetNodeState(CreateNode.NodeState.Locked);
                }
            }
            else
            {
                InitiateSavedNodes();
            }
        }

        // Adventure has begun!
        adventureBegin = true;
    }

    // This function activates the nodes post combat
    public void InitiateSavedNodes()
    {
        // Activate new set of nodes
        foreach(GameObject node in ListOfSavedNodes)
        {
            node.SetActive(true);
            CreateNode nodeComponent = node.GetComponent<CreateNode>();
            nodeComponent.GetSceneComponents();
            nodeComponent.GetComponent<Interactable>().interactableDescriptionWindow = subscreenManager.ToolTipWindow;
            nodeComponent.GetComponent<Interactable>().interactableText = subscreenManager.ToolTipWindowTextComponent;
        }

        // Unlock next node from node to return to
        ActivateNextNode();
    }

    // This function sets the adventure data
    public void InitiateAdventureData()
    {
        // Set adventure boss
        if (adventureBoss == null)
        {
            adventureBoss = currentSelectedAdventure.adventureBoss;
        }

        // Is the boss still alive
        if (!BossDefeated)
        {
            SubscreenMenu.SetActive(false);
        }

        // Check if battle over
        if (!adventureFailed)
        {
            PlayNewBGM(adventureBGM, .09f);
        }
        else
        {
            PlayNewBGM(defeatBGM, .08f);
            ShowFinalResultsMenu(false);
        }
    }

    // This function resets all locked nodes to initiate NG+
    public void InitiateNewGame()
    {
        adventureNGNumber += 1;
        rerollAmount += 1;

        BossBattle = false;
        BossDefeated = false;
        adventureFailed = false;
        adventureBegin = false;
        NodeToReturnTo = null;
        lockEquipmentInCombat = false;

        // Return equipped equipment from dead allies and then clear the graveyard
        foreach(Monster allyDeadMonster in ListOfAllyDeadMonsters)
        {
            foreach (Modifier equipment in allyDeadMonster.ListOfModifiers.Where(equipment => equipment.modifierType == Modifier.ModifierType.equipmentModifier).ToList())
            {
                allyDeadMonster.ListOfModifiers.Where(equipment => equipment.modifierType == Modifier.ModifierType.equipmentModifier).ToList().Remove(equipment);
                equipment.modifierOwner = null;
            }
        }

        ListOfAllyDeadMonsters.Clear();

        // start giving enemies modifiers
        if (adventureNGNumber >= 3)
        {
            ListOfEnemyModifiers.Add(subscreenManager.GetRandomModifier());

            if (adventureNGNumber >= 5)
            {
                ListOfEnemyModifiers.Add(subscreenManager.GetRandomModifier());
            }
        }

        // Clear lists
        foreach (GameObject node in ListOfAllNodes)
        {
            Destroy(node);
        }
        foreach (GameObject node in ListOfSavedNodes)
        {
            Destroy(node);
        }

        //ListOfUnlockedNodes.Clear();
        //ListOfLockedNodes.Clear();

        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    // This function resets all locked nodes to restart game
    public void RestartGame()
    {
        rerollAmount = 3;

        playerGold = 0;

        playerMonstersKilled = 0;

        playerMonstersLost = 0;

        timesRerolled = 0;

        BossBattle = false;

        BossDefeated = false;

        adventureFailed = false;

        adventureBegin = false;

        NodeToReturnTo = null;

        lockEquipmentInCombat = false;

        // Clear lists
        foreach (GameObject node in ListOfAllNodes)
        {
            Destroy(node);
        }
        foreach (GameObject node in ListOfSavedNodes)
        {
            Destroy(node);
        }

        //ListOfUnlockedNodes.Clear();
        //ListOfLockedNodes.Clear();

        // Reset Modifiers + Equipment
        ListOfCurrentModifiers.Clear();

        ListOfInventoryItems.Clear();

        ListOfCurrentEquipment.Clear();

        // Reset Allys and Enemies
        ListOfCurrentMonsters.Clear();

        ListOfAllMonsters.Clear();

        ListOfAllyDeadMonsters.Clear();

        ListOfInitialEnemyBattleMonsters.Clear();

        ListOfEnemyBattleMonsters.Clear();

        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    // This function runs when a node is clicked
    public void CheckNodeLocked()
    {
        if (currentSelectedNode == null)
        {
            return;
        }

        if (currentSelectedNode != null)
        {
            NodeComponent = currentSelectedNode.GetComponent<CreateNode>();
        }

        if (NodeComponent.nodeLocked && !BossDefeated)
        {
            routeText.text = ($"Route is locked!");
            return;
        }

        if (!BossDefeated)
        {
            ShowSubscreenMenu();
        }
    }

    // This function activates the next node to unlock
    public void ActivateNextNode()
    {
        if (!adventureFailed)
        {
            foreach (GameObject node in cachedSelectedNode.GetComponent<CreateNode>().nodesToUnlock)
            {
                node.GetComponent<CreateNode>().nodeLocked = false;
                node.GetComponent<CreateNode>().SetNodeState(CreateNode.NodeState.Unlocked);
                node.GetComponent<Animator>().SetBool("unlocked", true);
                //ListOfUnlockedNodes.Add(node);
                //nodeSelectionTargeter.transform.position = ListOfUnlockedNodes[0].GetComponent<CreateNode>().selectedPosition.transform.position;
            }
            //
            foreach (GameObject node in cachedSelectedNode.GetComponent<CreateNode>().nodesToLock)
            {
                node.GetComponent<CreateNode>().nodeLocked = true;
                node.GetComponent<CreateNode>().SetNodeState(CreateNode.NodeState.Locked);
                //ListOfLockedNodes.Add(node);
            }
        }

        if (BossDefeated)
        {
            routeText.text = ($"You Win!");
            ShowFinalResultsMenu(true);
        }
    }

    // This function displays the subscreen menu, showing either pre-combat data or avaiable rewards - Extend to show current team/modifiers/equipment
    public void ShowSubscreenMenu()
    {
        SubscreenMenu.SetActive(true);
        cachedSelectedNode = currentSelectedNode;

        switch (NodeComponent.nodeType)
        {
            case CreateNode.NodeType.Start:
                subScreenMenuText.text = ($"Select starting Chimeric...\n(Right-click for more info)");
                subscreenManager.LoadRewardSlots(RewardType.Monster);
                break;

            case CreateNode.NodeType.RandomReward:
                currentRewardType = ReturnRandomRewardType();
                subScreenMenuText.text = ($"Select {currentRewardType.ToString()}...");
                subscreenManager.LoadRewardSlots(currentRewardType);
                break;

            case CreateNode.NodeType.ModifierReward:
                currentRewardType = RewardType.Modifier;
                subScreenMenuText.text = ($"Select Modifier...");
                subscreenManager.LoadRewardSlots(RewardType.Modifier);
                break;

            case CreateNode.NodeType.EquipmentReward:
                currentRewardType = RewardType.Equipment;
                subScreenMenuText.text = ($"Select {currentRewardType.ToString()}...");
                subscreenManager.LoadRewardSlots(RewardType.Equipment);
                break;

            case CreateNode.NodeType.MonsterReward:
                currentRewardType = RewardType.Monster;
                subScreenMenuText.text = ($"Select Chimeric...\n(Right-click for more info)");
                subscreenManager.LoadRewardSlots(RewardType.Monster);
                break;

            case CreateNode.NodeType.RandomCombat:
                subScreenMenuText.text = ($"Select Chimerics and begin battle...");
                subscreenManager.HideRewardSlots();
                subscreenManager.LoadRandomBattle();
                break;

            case CreateNode.NodeType.Boss:
                subScreenMenuText.text = ($"Select Chimerics and begin final battle...");
                subscreenManager.HideRewardSlots();
                subscreenManager.LoadRandomBattle();
                break;

            default:
                break;
        }
    }

    // This function displays the monsters menu, showing the player's team
    public void ShowMonstersSubscreenMenu()
    {
        MonstersSubscreen.SetActive(true);
    }

    // This function displays the final screen post-adventure
    public void ShowFinalResultsMenu(bool Win)
    {
        SubscreenMenu.SetActive(true);
        subscreenManager.ShowFinalResultsMenu(Win);
    }

    // This function returns the MVP monster at the end of the adventure, sorted by damage done and kills
    public Monster GetMVPMonster()
    {
        // Get monster from list with most damage done
        ListOfAllMonsters = ListOfAllMonsters.OrderByDescending(Monster => Monster.cachedDamageDone).ToList();
        Monster mvp = ListOfAllMonsters[0];

        return mvp;
    }

    // This function returns a random reward type
    RewardType ReturnRandomRewardType()
    {
        RewardType randReward = (RewardType)Random.Range(0, 1);
        return randReward;
    }

    // This function returns a random monster scriptableObject from a list
    public Monster GetRandomMonster(List<Monster> listOfMonsters)
    {
        Monster monster = listOfMonsters[Random.Range(0, listOfMonsters.Count)];
        return monster;
    }

    // This function returns a random monster gameObject from a list
    public GameObject GetRandomMonsterGameObject(List<GameObject> listOfMonsters)
    {
        GameObject monster = listOfMonsters[Random.Range(0, listOfMonsters.Count)];
        return monster;
    }

    // This helper function finds inactive nodes to add into a list
    GameObject FindInActiveObjectByTag(string tag)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].hideFlags == HideFlags.None)
            {
                if (objs[i].CompareTag(tag))
                {
                    return objs[i].gameObject;
                }
            }
        }
        return null;
    }

    // This function handles the currently playing BGM - TODO - Create an AudioManager Script
    public void PlayNewBGM(AudioClip newBGM, float scale)
    {
        GameManagerAudioSource.Stop();
        GameManagerAudioSource.clip = newBGM;
        GameManagerAudioSource.volume = scale;
        GameManagerAudioSource.Play();
    }

    // This function removes all monsters Equipped equipment
    public void RemoveMonsterEquipment(Monster _monster)
    {
        List<Modifier> currentEquipment = _monster.ListOfModifiers.Where(equipment => equipment.modifierType == Modifier.ModifierType.equipmentModifier).ToList();
        foreach (Modifier equipment in currentEquipment)
        {
            equipment.modifierOwner = null;
        }
    }

    // Play UI SFX
    public void PlaySFX(AudioClip _sound)
    {
        GameManagerSFXSource.PlayOneShot(_sound);
    }

    public void NewUIScreenSelected()
    {
        GameManagerSFXSource.PlayOneShot(UIScreenSelectSFX);
    }

    // Unload adventure mode data
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public int GetGoldReward(List<GameObject> listOfMonsters)
    {
        int monsterAvgLevel = 0;

        foreach(GameObject monsterGameObject in listOfMonsters)
        {
            monsterAvgLevel += monsterGameObject.GetComponent<CreateMonster>().monsterReference.level;
        }

        return monsterAvgLevel / listOfMonsters.Count;
    }

    public Item GetRandomItemByRarity()
    {
        if (ListOfAvailableItems.Count == 0)
        {
            ResetItemList();
        }

        Item randItem = null;
        List<Item> curatedListOfItems = new List<Item>();

        // First, check common
        float randValue = Random.value;
        Debug.Log($"Generated random value: {randValue}");
        if (randValue < ReturnModifierWeightedProbability(Modifier.ModifierRarity.Common) / 100f)
        {
            curatedListOfItems = ListOfAvailableItems.Where(item => item.itemRarity == Modifier.ModifierRarity.Common).ToList();
            if (curatedListOfItems.Count == 0)
            {
                ResetItemList();
                curatedListOfItems = ListOfAvailableItems.Where(item => item.itemRarity == Modifier.ModifierRarity.Common).ToList();
            }
            randItem = curatedListOfItems[Random.Range(0, curatedListOfItems.Count)];
            //Debug.Log($"Hit common! {randValue} < {ReturnModifierWeightedProbability(Modifier.ModifierRarity.Common) / 100f}");
        }

        // Check uncommon
        if (randValue < ReturnModifierWeightedProbability(Modifier.ModifierRarity.Uncommon) / 100f)
        {
            curatedListOfItems = ListOfAvailableItems.Where(item => item.itemRarity == Modifier.ModifierRarity.Uncommon).ToList();
            if (curatedListOfItems.Count == 0)
            {
                ResetItemList();
                curatedListOfItems = ListOfAvailableItems.Where(item => item.itemRarity == Modifier.ModifierRarity.Uncommon).ToList();
            }
            randItem = curatedListOfItems[Random.Range(0, curatedListOfItems.Count)];
            //Debug.Log($"Hit common! {randValue} < {ReturnModifierWeightedProbability(Modifier.ModifierRarity.Common) / 100f}");
        }

        // Check rare
        if (randValue < ReturnModifierWeightedProbability(Modifier.ModifierRarity.Rare) / 100f)
        {
            curatedListOfItems = ListOfAvailableItems.Where(item => item.itemRarity == Modifier.ModifierRarity.Rare).ToList();
            if (curatedListOfItems.Count == 0)
            {
                ResetItemList();
                curatedListOfItems = ListOfAvailableItems.Where(item => item.itemRarity == Modifier.ModifierRarity.Rare).ToList();
            }
            randItem = curatedListOfItems[Random.Range(0, curatedListOfItems.Count)];
            //Debug.Log($"Hit common! {randValue} < {ReturnModifierWeightedProbability(Modifier.ModifierRarity.Common) / 100f}");
        }

        // Lastly, check legendary
        if (randValue < ReturnModifierWeightedProbability(Modifier.ModifierRarity.Legendary) / 100f)
        {
            curatedListOfItems = ListOfAvailableItems.Where(item => item.itemRarity == Modifier.ModifierRarity.Legendary).ToList();
            if (curatedListOfItems.Count == 0)
            {
                ResetItemList();
                curatedListOfItems = ListOfAvailableItems.Where(item => item.itemRarity == Modifier.ModifierRarity.Legendary).ToList();
            }
            randItem = curatedListOfItems[Random.Range(0, curatedListOfItems.Count)];
            //Debug.Log($"Hit common! {randValue} < {ReturnModifierWeightedProbability(Modifier.ModifierRarity.Legendary) / 100f}");
        }

        //ListOfAvailableItems.Remove(randItem);
        Item randItemSO = Instantiate(randItem);
        randItemSO.name = randItemSO.itemName;

        curatedListOfItems.Clear();

        return randItemSO;
    }

    public float ReturnModifierWeightedProbability(Modifier.ModifierRarity modifierRarity)
    {
        switch (modifierRarity)
        {
            case (Modifier.ModifierRarity.Common):
                return commonChanceRate;

            case (Modifier.ModifierRarity.Uncommon):
                return uncommonChanceRate;

            case (Modifier.ModifierRarity.Rare):
                return rareChanceRate;

            case (Modifier.ModifierRarity.Legendary):
                return legendaryChanceRate;

            default:
                return 45;
        }
    }

    public Item GetRandomItem()
    {
        if (ListOfAvailableItems.Count == 0)
        {
            return null;
        }

        Item randItem = ListOfAvailableItems[Random.Range(0, ListOfAvailableItems.Count)];
        ListOfAvailableItems.Remove(randItem);

        Item randItemSO = Instantiate(randItem);
        return randItemSO;
    }

    public void ReviveAllDeadAlliedMonstersBeforeBattle()
    {
        foreach (Monster allyMonster in ListOfAllyDeadMonsters.OrderBy(monsterIndex => monsterIndex.monsterCachedBattleIndex).ToList())
        {
            Monster revivedAlly = Instantiate(allyMonster);

            ListOfCurrentMonsters.Add(revivedAlly);

            ListOfAllyBattleMonsters.Add(revivedAlly);

            revivedAlly.health = revivedAlly.cachedHealthAtBattleStart;

            if (revivedAlly.health <= 0)
                revivedAlly.health = 1;

            revivedAlly.ListOfModifiers = revivedAlly.ListOfModifiers.Where(mod => mod.modifierType == Modifier.ModifierType.equipmentModifier).ToList();

            revivedAlly.physicalAttack = revivedAlly.cachedPhysicalAttack;

            revivedAlly.magicAttack = revivedAlly.cachedMagicAttack;

            revivedAlly.physicalDefense = revivedAlly.cachedPhysicalDefense;

            revivedAlly.magicDefense = revivedAlly.cachedMagicDefense;

            revivedAlly.speed = revivedAlly.cachedSpeed;

            revivedAlly.evasion = revivedAlly.cachedEvasion;

            revivedAlly.critChance = revivedAlly.cachedCritChance;

            revivedAlly.critDamage = revivedAlly.cachedCritDamage;

            revivedAlly.bonusAccuracy = revivedAlly.cachedBonusAccuracy;
        }

        ListOfAllyDeadMonsters.Clear();
    }

    public void ReviveAllDeadEnemyMonstersBeforeBattle()
    {
        foreach (Monster allyMonster in ListOfInitialEnemyBattleMonsters.OrderBy(monsterIndex => monsterIndex.monsterCachedBattleIndex).ToList())
        {
            Monster revivedAlly = Instantiate(allyMonster);

            ListOfEnemyBattleMonsters.Add(revivedAlly);

            revivedAlly.health = revivedAlly.cachedHealthAtBattleStart;

            if (revivedAlly.health <= 0)
                revivedAlly.health = 1;

            revivedAlly.ListOfModifiers = revivedAlly.ListOfModifiers.Where(mod => mod.modifierType == Modifier.ModifierType.equipmentModifier).ToList();

            revivedAlly.physicalAttack = revivedAlly.cachedPhysicalAttack;

            revivedAlly.magicAttack = revivedAlly.cachedMagicAttack;

            revivedAlly.physicalDefense = revivedAlly.cachedPhysicalDefense;

            revivedAlly.magicDefense = revivedAlly.cachedMagicDefense;

            revivedAlly.speed = revivedAlly.cachedSpeed;

            revivedAlly.evasion = revivedAlly.cachedEvasion;

            revivedAlly.critChance = revivedAlly.cachedCritChance;

            revivedAlly.critDamage = revivedAlly.cachedCritDamage;

            revivedAlly.bonusAccuracy = revivedAlly.cachedBonusAccuracy;
        }

        ListOfInitialEnemyBattleMonsters.Clear();
    }

    public  void RetryBattle()
    {
        ReviveAllDeadAlliedMonstersBeforeBattle();

        ReviveAllDeadEnemyMonstersBeforeBattle();
    }

    internal void HealMonster(Monster monsterToHeal, float modifierAmount)
    {
        throw new System.NotImplementedException();
    }
}
