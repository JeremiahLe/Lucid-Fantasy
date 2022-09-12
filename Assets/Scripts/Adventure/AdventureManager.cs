using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

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
            ListOfInventoryItems.Add(subscreenManager.GetRandomItem());
            ResetItemList();
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
    public int rerollAmount = 0;
    public int timesRerolled = 0;

    public int playerGold = 0;
    public int playerGoldSpent = 0;

    public int playerMonstersLost = 0;
    public int playerMonstersKilled = 0;

    public int adventureNGNumber = 1;

    public List<Monster> ListOfCurrentMonsters;

    public List<Modifier> ListOfCurrentModifiers;

    public List<Monster> ListOfAllMonsters;

    public List<Modifier> ListOfCurrentEquipment;
    public List<Item> ListOfInventoryItems;

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

    public List<GameObject> ListOfUnlockedNodes;
    public List<GameObject> ListOfLockedNodes;

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
        GameManagerAudioSource = GetComponent<AudioSource>();
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
                modifierAdventureReference = item.modifierAdventureReference,
                adventureModifier = item.adventureModifier,
                modifierAdventureCallTime = item.modifierAdventureCallTime,
                modifierAmount = item.modifierAmount,
                modifierDescription = item.modifierDescription,
                modifierCurrentDuration = item.modifierCurrentDuration,
                modifierDuration = item.modifierDuration,
                modifierDurationType = item.modifierDurationType,
                modifierRarity = item.modifierRarity,
                modifierSource = item.modifierSource,
                statModified = item.statModified,
                baseSprite = item.baseSprite,
                statusEffect = item.statusEffect,
                statusEffectType = item.statusEffectType,
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
                modifierAdventureReference = item.modifierAdventureReference,
                adventureModifier = item.adventureModifier,
                modifierAdventureCallTime = item.modifierAdventureCallTime,
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
                adventureEquipment = item.adventureEquipment,
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
                modifierAdventureReference = item.modifierAdventureReference,
                adventureModifier = item.adventureModifier,
                modifierAdventureCallTime = item.modifierAdventureCallTime,
                modifierAmount = item.modifierAmount,
                modifierDescription = item.modifierDescription,
                modifierCurrentDuration = item.modifierCurrentDuration,
                modifierDuration = item.modifierDuration,
                modifierDurationType = item.modifierDurationType,
                modifierRarity = item.modifierRarity,
                modifierSource = item.modifierSource,
                statModified = item.statModified,
                baseSprite = item.baseSprite,
                statusEffect = item.statusEffect,
                statusEffectType = item.statusEffectType,
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
                modifierAdventureReference = item.modifierAdventureReference,
                adventureModifier = item.adventureModifier,
                modifierAdventureCallTime = item.modifierAdventureCallTime,
                modifierAmount = item.modifierAmount,
                modifierDescription = item.modifierDescription,
                modifierCurrentDuration = item.modifierCurrentDuration,
                modifierDuration = item.modifierDuration,
                modifierDurationType = item.modifierDurationType,
                modifierRarity = item.modifierRarity,
                modifierSource = item.modifierSource,
                modifierAmountFlatBuff = item.modifierAmountFlatBuff,
                adventureEquipment = item.adventureEquipment,
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
                itemType = item.itemType
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
            foreach (GameObject node in ListOfUnlockedNodes)
            {
                node.SetActive(false);
                DontDestroyOnLoad(node);
            }

            //
            foreach (GameObject node in ListOfLockedNodes)
            {
                node.SetActive(false);
                DontDestroyOnLoad(node);
            }

            //
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

        SceneManager.LoadScene("SetupCombatScene");
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
                

            case ("SetupCombatScene"):

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

        #region Old Code
        /*
        if (scene.name == "StartScreen")
        {
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
        }
        else if (scene.name == "BasicAdventureScene")
        {
            AdventureMenu = GameObject.FindGameObjectWithTag("AdventureMenu");
            SubscreenMenu = GameObject.FindGameObjectWithTag("SubscreenMenu");

            if (SubscreenMenu == null)
            {
                SubscreenMenu = FindInActiveObjectByTag("SubscreenMenu");
            }

            subscreenManager = SubscreenMenu.GetComponent<SubscreenManager>();
            InitiateSceneData();
            adventureSceneName = SceneManager.GetActiveScene().name;

            // Is the boss till alive
            if (!BossDefeated)
            {
                SubscreenMenu.SetActive(false);
            }

            // Check if battle over
            if (!adventureFailed)
            {
                PlayNewBGM(adventureBGM, .60f);
            }
            else
            {
                PlayNewBGM(defeatBGM, .35f);
                ShowFinalResultsMenu(false);
            }
        }
        else if (scene.name == "SetupCombatScene")
        {
            CombatManagerObject = GameObject.FindGameObjectWithTag("GameController");
            combatManagerScript = CombatManagerObject.GetComponent<CombatManagerScript>();
            combatManagerScript.adventureMode = true;
            combatManagerScript.previousSceneName = adventureSceneName;

            // Boss Music
            if (BossBattle)
            {
                PlayNewBGM(bossBGM, .35f);
                return;
            }

            // Combat Music
            PlayNewBGM(combatBGM, .30f);
        }
        */
        #endregion
    }

    // This function is called whenever an out of combat passive modifier is obtained
    public void ApplyPassiveModifiers()
    {
        foreach (Modifier mod in ListOfCurrentModifiers)
        {
            if (mod.modifierAdventureCallTime == Modifier.ModifierAdventureCallTime.OOCPassive)
            {
                switch (mod.modifierAdventureReference)
                {
                    case (AdventureModifiers.AdventureModifierReferenceList.RisingPotential):
                        bonusExp += (mod.modifierAmount / 100f);
                        break;

                    default:
                        Debug.Log("Missing OOC Passive reference?", this);
                        break;
                }
            }
        }
    }

    // This function is called at Game Start, before Round 1, to apply any adventure modifiers to a single-target
    public void ApplyGameStartAdventureModifiers(Monster.AIType aIType)
    {
        List<Modifier> whatListShouldIUse;
        GameObject monsterObj;
        Monster monster;
        Modifier newModifier;

        // Apply ally or enemy modifiers?
        if (aIType == Monster.AIType.Ally)
        {
            whatListShouldIUse = ListOfCurrentModifiers;
        }
        else
        {
            whatListShouldIUse = ListOfEnemyModifiers;
        }

        foreach (Modifier modifier in whatListShouldIUse)
        {
            // Only apply Game Start modifiers
            if (modifier.modifierAdventureCallTime == Modifier.ModifierAdventureCallTime.GameStart)
            {
                // Get specific Modifier
                switch (modifier.modifierAdventureReference)
                {
                    #region WindsweptBoots
                    case (AdventureModifiers.AdventureModifierReferenceList.WindsweptBoots):
                        if (aIType == Monster.AIType.Ally)
                        {
                            monsterObj = GetRandomMonsterGameObject(combatManagerScript.ListOfAllys);
                        }
                        else
                        {
                            monsterObj = GetRandomMonsterGameObject(combatManagerScript.ListOfEnemies);
                        }
                        monster = monsterObj.GetComponent<CreateMonster>().monster;

                        newModifier = Instantiate(modifier);
                        monster.ListOfModifiers.Add(newModifier);
                        monsterObj.GetComponent<CreateMonster>().ModifyStats(modifier.statModified, modifier, true);
                        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name}'s {modifier.statModified} was increased by {modifier.modifierName} (+{modifier.modifierAmount})!");
                        break;
                    #endregion

                    #region Chosen One
                    case (AdventureModifiers.AdventureModifierReferenceList.ChosenOne):
                        if (aIType == Monster.AIType.Ally)
                        {
                            monsterObj = GetRandomMonsterGameObject(combatManagerScript.ListOfAllys);
                        }
                        else
                        {
                            monsterObj = GetRandomMonsterGameObject(combatManagerScript.ListOfEnemies);
                        }
                        monster = monsterObj.GetComponent<CreateMonster>().monster;

                        // Increase stats
                        float modAmount = modifier.modifierAmount / 100f;
                        monster.physicalAttack = Mathf.RoundToInt(monster.physicalAttack * modAmount);
                        monster.magicAttack = Mathf.RoundToInt(monster.magicAttack * modAmount);
                        monster.physicalDefense = Mathf.RoundToInt(monster.physicalDefense * modAmount);
                        monster.magicDefense = Mathf.RoundToInt(monster.magicDefense * modAmount);
                        monster.evasion = Mathf.RoundToInt(monster.evasion * modAmount);
                        monster.critChance = Mathf.RoundToInt(monster.critChance * modAmount);
                        monster.speed = Mathf.RoundToInt(monster.speed * modAmount);
                        monsterObj.GetComponent<CreateMonster>().UpdateStats(false, null, false);
                        monsterObj.GetComponent<CreateMonster>().AddSpecialStatusIcon(modifier);
                        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name}'s stats was increased by {modifier.modifierName} (x{modAmount})!");
                        break;
                    #endregion

                    #region Guardian Angel
                    case (AdventureModifiers.AdventureModifierReferenceList.GuardianAngel):
                        if (aIType == Monster.AIType.Ally)
                        {
                            monsterObj = combatManagerScript.ListOfAllys[0];
                        }
                        else
                        {
                            monsterObj = combatManagerScript.ListOfEnemies[0];
                        }
                        monster = monsterObj.GetComponent<CreateMonster>().monster;

                        // Increase stats
                        newModifier = Instantiate(modifier);
                        monster.ListOfModifiers.Add(newModifier);
                        monsterObj.GetComponent<CreateMonster>().ModifyStats(newModifier.statModified, newModifier, true);
                        monsterObj.GetComponent<CreateMonster>().UpdateStats(false, null, false);
                        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} was granted damage immunity by {newModifier.modifierName} for {newModifier.modifierDuration - 1} rounds!");
                        break;
                    #endregion

                    #region Opening Gambit
                    case (AdventureModifiers.AdventureModifierReferenceList.OpeningGambit):
                        if (aIType == Monster.AIType.Ally)
                        {
                            monsterObj = combatManagerScript.ListOfAllys.OrderByDescending(FastestMonster => FastestMonster.GetComponent<CreateMonster>().monsterReference.speed).ToList().First();
                        }
                        else
                        {
                            monsterObj = combatManagerScript.ListOfEnemies.OrderByDescending(FastestMonster => FastestMonster.GetComponent<CreateMonster>().monsterReference.speed).ToList().First();
                        }
                        monster = monsterObj.GetComponent<CreateMonster>().monster;

                        // Physical Attack
                        newModifier = Instantiate(modifier);
                        newModifier.statModified = AttackEffect.StatEnumToChange.PhysicalAttack;
                        float oldStat = monsterObj.GetComponent<CreateMonster>().GetStatToChange(newModifier.statModified, monster);
                        newModifier.modifierAmount = Mathf.RoundToInt(oldStat * (newModifier.modifierAmount / 100f) - oldStat);
                        monster.ListOfModifiers.Add(newModifier);
                        monsterObj.GetComponent<CreateMonster>().ModifyStats(newModifier.statModified, newModifier, true);

                        // Magic Attack
                        Modifier newModifier2 = Instantiate(modifier);
                        newModifier2.statModified = AttackEffect.StatEnumToChange.MagicAttack;
                        oldStat = monsterObj.GetComponent<CreateMonster>().GetStatToChange(newModifier2.statModified, monster);
                        newModifier2.modifierAmount = Mathf.RoundToInt(oldStat * (newModifier2.modifierAmount / 100f) - oldStat);
                        monster.ListOfModifiers.Add(newModifier2);
                        monsterObj.GetComponent<CreateMonster>().ModifyStats(newModifier2.statModified, newModifier2, true);

                        // CritChance
                        Modifier newModifier3 = Instantiate(modifier);
                        newModifier3.modifierAmount = 100f;
                        newModifier3.statModified = AttackEffect.StatEnumToChange.CritChance;
                        monster.ListOfModifiers.Add(newModifier3);
                        monsterObj.GetComponent<CreateMonster>().ModifyStats(newModifier3.statModified, newModifier3, true);

                        monsterObj.GetComponent<CreateMonster>().UpdateStats(false, null, false);
                        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} was gained 1.5x physical and magic attack and 100% CritChance by {newModifier.modifierName} for {newModifier.modifierDuration - 1} rounds!");
                        break;
                    #endregion

                    #region Deadeye
                    case (AdventureModifiers.AdventureModifierReferenceList.Deadeye):
                        if (aIType == Monster.AIType.Ally)
                        {
                            monsterObj = combatManagerScript.ListOfAllys[0];
                        }
                        else
                        {
                            monsterObj = combatManagerScript.ListOfEnemies[0];
                        }
                        monster = monsterObj.GetComponent<CreateMonster>().monster;

                        newModifier = Instantiate(modifier);
                        monster.ListOfModifiers.Add(newModifier);
                        monsterObj.GetComponent<CreateMonster>().ModifyStats(modifier.statModified, modifier, true);
                        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name}'s {modifier.statModified} was increased by {modifier.modifierName} (+{modifier.modifierAmount})!");
                        break;
                    #endregion

                    default:
                        break;
                }
            }
        }
    }

    // This function is called every Round Start by CombatManagerScript, to apply any adventure modifiers
    public IEnumerator ApplyRoundStartAdventureModifiers(Monster.AIType aIType)
    {
        List<Modifier> whatListShouldIUse;
        List<GameObject> listOfUnstatusedEnemies;
        GameObject monsterObj;
        Monster monsterRef;
        Modifier newModifier;

        // Apply ally or enemy modifiers?
        if (aIType == Monster.AIType.Ally)
        {
            whatListShouldIUse = ListOfCurrentModifiers;
        }
        else
        {
            whatListShouldIUse = ListOfEnemyModifiers;
        }

        foreach (Modifier modifier in whatListShouldIUse)
        {
            // Only apply Round Start modifiers
            if (modifier.modifierAdventureCallTime == Modifier.ModifierAdventureCallTime.RoundStart)
            {
                // Get specific Modifier
                switch (modifier.modifierAdventureReference)
                {               
                    #region Virulent Venom
                    case AdventureModifiers.AdventureModifierReferenceList.VirulentVenom:

                        // Get random enemy from list of unpoisoned enemies
                        if (aIType == Monster.AIType.Ally)
                        {
                            listOfUnstatusedEnemies = combatManagerScript.ListOfEnemies.Where(isPoisoned => isPoisoned.GetComponent<CreateMonster>().monsterIsPoisoned == false).ToList();
                        }
                        else
                        {
                            listOfUnstatusedEnemies = combatManagerScript.ListOfAllys.Where(isPoisoned => isPoisoned.GetComponent<CreateMonster>().monsterIsPoisoned == false).ToList();
                        }

                        // If no monsters are unpoisoned, break out
                        if (listOfUnstatusedEnemies.Count() == 0)
                        {
                            continue;
                        }

                        // Otherwise, poison randomly selected monster
                        if (listOfUnstatusedEnemies.Count != 0) {
                            GameObject randomEnemyToPoison = combatManagerScript.GetRandomTarget(listOfUnstatusedEnemies);
                            Monster monster = randomEnemyToPoison.GetComponent<CreateMonster>().monsterReference;

                            // First check if monster is immune to debuffs, if so, break out.
                            if (randomEnemyToPoison.GetComponent<CreateMonster>().monsterImmuneToDebuffs)
                            {
                                // Send immune message to combat log
                                combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} is immune to status effects and debuffs!");
                                randomEnemyToPoison.GetComponent<CreateMonster>().CreateStatusEffectPopup("Immune!");
                                continue;
                            }

                            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} was poisoned by {modifier.modifierName}.");
                            randomEnemyToPoison.GetComponent<CreateMonster>().monsterIsPoisoned = true;

                            if (aIType == Monster.AIType.Ally)
                            {
                                modifier.modifierOwnerGameObject = combatManagerScript.ListOfAllys[0];
                                modifier.modifierOwner = modifier.modifierOwnerGameObject.GetComponent<CreateMonster>().monsterReference;
                            }

                            newModifier = Instantiate(modifier);
                            monster.ListOfModifiers.Add(newModifier);
                            randomEnemyToPoison.GetComponent<CreateMonster>().InflictStatus(Modifier.StatusEffectType.Poisoned);
                            randomEnemyToPoison.GetComponent<CreateMonster>().AddStatusIcon(newModifier, newModifier.statModified, newModifier.modifierCurrentDuration);
                        }

                        // Clear list
                        listOfUnstatusedEnemies.Clear();
                        break;
                    #endregion

                    #region Raging Fire
                    case AdventureModifiers.AdventureModifierReferenceList.RagingFire:

                        // Get random enemy from list of unpoisoned enemies
                        if (aIType == Monster.AIType.Ally)
                        {
                            listOfUnstatusedEnemies = combatManagerScript.ListOfEnemies.Where(isBurning => isBurning.GetComponent<CreateMonster>().monsterIsBurning == false).ToList();
                        }
                        else
                        {
                            listOfUnstatusedEnemies = combatManagerScript.ListOfAllys.Where(isBurning => isBurning.GetComponent<CreateMonster>().monsterIsBurning == false).ToList();
                        }

                        // If no monsters are unpoisoned, break out
                        if (listOfUnstatusedEnemies.Count == 0)
                        {
                            continue;
                        }

                        // Otherwise, poison randomly selected monster
                        if (listOfUnstatusedEnemies.Count != 0)
                        {
                            GameObject randomEnemyToBurn = combatManagerScript.GetRandomTarget(listOfUnstatusedEnemies);
                            Monster monster = randomEnemyToBurn.GetComponent<CreateMonster>().monsterReference;

                            // First check if monster is immune to debuffs, if so, break out.
                            if (randomEnemyToBurn.GetComponent<CreateMonster>().monsterImmuneToDebuffs)
                            {
                                // Send immune message to combat log
                                combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} is immune to status effects and debuffs!");
                                randomEnemyToBurn.GetComponent<CreateMonster>().CreateStatusEffectPopup("Immune!");
                                continue;
                            }

                            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} was burned by {modifier.modifierName}.");
                            randomEnemyToBurn.GetComponent<CreateMonster>().monsterIsBurning = true;

                            if (aIType == Monster.AIType.Ally)
                            {
                                modifier.modifierOwnerGameObject = combatManagerScript.ListOfAllys[0];
                                modifier.modifierOwner = modifier.modifierOwnerGameObject.GetComponent<CreateMonster>().monsterReference;
                            }

                            newModifier = Instantiate(modifier);
                            monster.ListOfModifiers.Add(newModifier);
                            randomEnemyToBurn.GetComponent<CreateMonster>().InflictStatus(Modifier.StatusEffectType.Burning);
                            randomEnemyToBurn.GetComponent<CreateMonster>().AddStatusIcon(newModifier, newModifier.statModified, newModifier.modifierCurrentDuration);
                        }

                        // Clear list
                        listOfUnstatusedEnemies.Clear();
                        break;
                    #endregion Raging Fire

                    #region Chaos Waves
                    case AdventureModifiers.AdventureModifierReferenceList.ChaosWaves:

                        // Get random enemy from list of unstatused enemies
                        if (aIType == Monster.AIType.Ally)
                        {
                            listOfUnstatusedEnemies = combatManagerScript.ListOfEnemies.Where(isDazed => isDazed.GetComponent<CreateMonster>().monsterIsDazed == false).ToList();
                        }
                        else
                        {
                            listOfUnstatusedEnemies = combatManagerScript.ListOfAllys.Where(isDazed => isDazed.GetComponent<CreateMonster>().monsterIsDazed == false).ToList();
                        }

                        // If no monsters are unpoisoned, break out
                        if (listOfUnstatusedEnemies.Count == 0)
                        {
                            continue;
                        }

                        // Otherwise, poison randomly selected monster
                        if (listOfUnstatusedEnemies.Count != 0)
                        {
                            GameObject randomEnemyToDaze = combatManagerScript.GetRandomTarget(listOfUnstatusedEnemies);
                            Monster monster = randomEnemyToDaze.GetComponent<CreateMonster>().monsterReference;

                            // First check if monster is immune to debuffs, if so, break out.
                            if (randomEnemyToDaze.GetComponent<CreateMonster>().monsterImmuneToDebuffs)
                            {
                                // Send immune message to combat log
                                combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} is immune to status effects and debuffs!");
                                randomEnemyToDaze.GetComponent<CreateMonster>().CreateStatusEffectPopup("Immune!");
                                continue;
                            }

                            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} was dazed by {modifier.modifierName}.");
                            randomEnemyToDaze.GetComponent<CreateMonster>().monsterIsDazed = true;

                            if (aIType == Monster.AIType.Ally)
                            {
                                modifier.modifierOwnerGameObject = combatManagerScript.ListOfAllys[0];
                                modifier.modifierOwner = modifier.modifierOwnerGameObject.GetComponent<CreateMonster>().monsterReference;
                            }

                            newModifier = Instantiate(modifier);
                            monster.ListOfModifiers.Add(newModifier);
                            randomEnemyToDaze.GetComponent<CreateMonster>().InflictStatus(Modifier.StatusEffectType.Dazed);
                            randomEnemyToDaze.GetComponent<CreateMonster>().AddStatusIcon(newModifier, newModifier.statModified, newModifier.modifierCurrentDuration);
                        }

                        // Clear list
                        listOfUnstatusedEnemies.Clear();
                        break;
                    #endregion

                    #region Elusive Spirit
                    case AdventureModifiers.AdventureModifierReferenceList.ElusiveSpirit:
                        if (aIType == Monster.AIType.Ally)
                        {
                            monsterObj = GetRandomMonsterGameObject(combatManagerScript.ListOfAllys);
                        }
                        else
                        {
                            monsterObj = GetRandomMonsterGameObject(combatManagerScript.ListOfEnemies);
                        }
                        monsterRef = monsterObj.GetComponent<CreateMonster>().monster;
                        newModifier = Instantiate(modifier);
                        monsterRef.ListOfModifiers.Add(newModifier);
                        monsterObj.GetComponent<CreateMonster>().ModifyStats(newModifier.statModified, newModifier, true);
                        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterRef.aiType} {monsterRef.name}'s {modifier.statModified} was increased by {modifier.modifierName} (+{modifier.modifierAmount})!");
                        break;
                    #endregion

                    #region Combat Training
                    case (AdventureModifiers.AdventureModifierReferenceList.CombatTraining):
                        if (aIType == Monster.AIType.Ally)
                        {
                            monsterObj = combatManagerScript.ListOfAllys[0];
                        }
                        else
                        {
                            monsterObj = combatManagerScript.ListOfEnemies[0];
                        }
                        monsterRef = monsterObj.GetComponent<CreateMonster>().monster;

                        // Increase stats
                        float modAmount = modifier.modifierAmount / 100f;
                        monsterRef.physicalAttack += Mathf.RoundToInt(monsterRef.physicalAttack * modAmount) + 1;
                        monsterRef.magicAttack += Mathf.RoundToInt(monsterRef.magicAttack * modAmount) + 1;
                        monsterRef.physicalDefense += Mathf.RoundToInt(monsterRef.physicalDefense * modAmount) + 1;
                        monsterRef.magicDefense += Mathf.RoundToInt(monsterRef.magicDefense * modAmount) + 1;
                        monsterRef.evasion += Mathf.RoundToInt(monsterRef.evasion * modAmount) + 1;
                        monsterRef.critChance += Mathf.RoundToInt(monsterRef.critChance * modAmount) + 1;
                        monsterRef.speed += Mathf.RoundToInt(monsterRef.speed * modAmount) + 1;

                        newModifier = Instantiate(modifier);
                        newModifier.modifierName = modifier.modifierName;
                        monsterRef.ListOfModifiers.Add(newModifier);

                        List<Modifier> modList = monsterRef.ListOfModifiers.Where(mod => mod.modifierName == newModifier.modifierName).ToList();

                        if (modList.Count > 1)
                        {
                            StatusEffectIcon statusEffectIconScript = modList.First().statusEffectIconGameObject.GetComponent<StatusEffectIcon>();
                            modList.First().statusEffectIconGameObject.GetComponent<StatusEffectIcon>().currentModifierStack += 1;
                            modList.First().statusEffectIconGameObject.GetComponent<StatusEffectIcon>().modifierDurationText.text = ($"x{statusEffectIconScript.currentModifierStack}");
                            monsterObj.GetComponent<CreateMonster>().UpdateStats(false, null, false);
                            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterRef.aiType} {monsterRef.name}'s stats was increased by {modifier.modifierName} ({modifier.modifierAmount}%)!");
                            continue;
                        }

                        monsterObj.GetComponent<CreateMonster>().UpdateStats(false, null, false);
                        monsterObj.GetComponent<CreateMonster>().ModifyStats(newModifier.statModified, newModifier, true);

                        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterRef.aiType} {monsterRef.name}'s stats was increased by {modifier.modifierName} ({modifier.modifierAmount}%)!");
                        break;
                    #endregion

                    default:
                        break;
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    // This function is called at Round End to apply Round End Modifiers
    public void ApplyRoundEndAdventureModifiers(Monster.AIType aIType)
    {
        List<Modifier> whatListShouldIUse;
        List<GameObject> listOfUnstatusedEnemies;
        GameObject monsterObj;
        Monster monsterRef;
        Modifier newModifier;

        // Apply ally or enemy modifiers?
        if (aIType == Monster.AIType.Ally)
        {
            whatListShouldIUse = ListOfCurrentModifiers;
        }
        else
        {
            whatListShouldIUse = ListOfEnemyModifiers;
        }

        foreach (Modifier modifier in whatListShouldIUse)
        {
            // Only apply Round End modifiers
            if (modifier.modifierAdventureCallTime == Modifier.ModifierAdventureCallTime.RoundEnd)
            {
                // Get specific Modifier
                switch (modifier.modifierAdventureReference)
                {
                    #region Blessing 0f Earth
                    case (AdventureModifiers.AdventureModifierReferenceList.BlessingOfEarth):

                        if (aIType == Monster.AIType.Ally)
                        {
                            listOfUnstatusedEnemies = combatManagerScript.ListOfAllys;
                        }
                        else
                        {
                            listOfUnstatusedEnemies = combatManagerScript.ListOfEnemies;
                        }

                        // Get monster with lowest health
                        monsterObj = listOfUnstatusedEnemies.OrderByDescending(monsterWithLowestHealth => monsterWithLowestHealth.GetComponent<CreateMonster>().monsterReference.health).ToList().Last();
                        monsterRef = monsterObj.GetComponent<CreateMonster>().monsterReference;

                        combatManagerScript.CombatLog.SendMessageToCombatLog($"{modifier.modifierName} was activated!");

                        AttackEffect effect = new AttackEffect(modifier.statModified, modifier.statChangeType, AttackEffect.EffectTime.PostAttack, Modifier.StatusEffectType.None, true, true, false, 0, modifier.modifierAmount, 100f, combatManagerScript);
                        effect.BuffTargetStat(monsterRef, combatManagerScript.monsterAttackManager, monsterObj, modifier.modifierName, true);

                        break;
                    #endregion

                    default:
                        break;
                }
            }
        }
    }
    
    // This function is called at Game Start by CombatManagerScript as it registers every Monster in Combat, before Round 1
    public void ApplyAdventureModifiers(Monster monster, GameObject monsterObj, Monster.AIType aIType)
    {
        Modifier newModifier = null;
        List<Modifier> whatListShouldIUse; 

        // Apply ally or enemy modifiers?
        if (aIType == Monster.AIType.Ally)
        {
            whatListShouldIUse = ListOfCurrentModifiers;
        }
        else
        {
            whatListShouldIUse = ListOfEnemyModifiers;
        }

        foreach (Modifier modifier in whatListShouldIUse)
        {
            // Get specific Modifier
            switch (modifier.modifierAdventureReference)
            {
                case AdventureModifiers.AdventureModifierReferenceList.WildFervor:
                    monster.critChance += modifier.modifierAmount;
                    // Increase stats
                    newModifier = Instantiate(modifier);
                    monster.ListOfModifiers.Add(newModifier);
                    monsterObj.GetComponent<CreateMonster>().AddSpecialStatusIcon(newModifier);
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name}'s critical chance was increased by {modifier.modifierName} (+{modifier.modifierAmount}).");
                    break;

                case AdventureModifiers.AdventureModifierReferenceList.TemperedOffense:
                    int physicalAttackIncrease = Mathf.RoundToInt(monster.cachedPhysicalAttack * (modifier.modifierAmount / 100f) + 1f);
                    monster.physicalAttack += physicalAttackIncrease;
                    int magicAttackIncrease = Mathf.RoundToInt(monster.cachedMagicAttack * (modifier.modifierAmount / 100f) + 1f);
                    monster.magicAttack += magicAttackIncrease;
                    // Increase stats
                    newModifier = Instantiate(modifier);
                    monster.ListOfModifiers.Add(newModifier);
                    monsterObj.GetComponent<CreateMonster>().AddSpecialStatusIcon(newModifier);
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name}'s physical and magic attack was increased by {modifier.modifierName} (+{physicalAttackIncrease}|+{magicAttackIncrease}).");
                    break;

                case AdventureModifiers.AdventureModifierReferenceList.TemperedDefense:
                    int physicalDefenseIncrease = Mathf.RoundToInt(monster.cachedPhysicalDefense * (modifier.modifierAmount / 100f) + 1f);
                    monster.physicalDefense += physicalDefenseIncrease;
                    int magicDefenseIncrease = Mathf.RoundToInt(monster.cachedMagicDefense * (modifier.modifierAmount / 100f) + 1f);
                    monster.magicDefense += magicDefenseIncrease;
                    // Increase stats
                    newModifier = Instantiate(modifier);
                    monster.ListOfModifiers.Add(newModifier);
                    monsterObj.GetComponent<CreateMonster>().AddSpecialStatusIcon(newModifier);
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name}'s physical and magic defense was increased by {modifier.modifierName} (+{physicalDefenseIncrease}|+{magicDefenseIncrease}).");
                    break;

                case AdventureModifiers.AdventureModifierReferenceList.TenaciousGuard:

                    //AttackEffect temp = AttackEffect.CreateInstance<AttackEffect>();
                    // Increase stats
                    newModifier = Instantiate(modifier);
                    monster.ListOfModifiers.Add(newModifier);
                    monsterObj.GetComponent<CreateMonster>().ModifyStats(newModifier.statModified, newModifier, true);
                    monsterObj.GetComponent<CreateMonster>().UpdateStats(false, null, false);
                    //temp.CreateAndAddModifiers(0, false, monster, monsterObj, modifier.modifierDuration, monsterObj, modifier.statModified);
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} gained immunity to statuses and debuffs for 3 rounds!");
                    break;

                case AdventureModifiers.AdventureModifierReferenceList.CrushingBlows:
                    // Increase stats
                    newModifier = Instantiate(modifier);
                    monster.ListOfModifiers.Add(newModifier);
                    monsterObj.GetComponent<CreateMonster>().ModifyStats(newModifier.statModified, newModifier, true);
                    monsterObj.GetComponent<CreateMonster>().UpdateStats(false, null, false);

                    // Create attack effect
                    AttackEffect effect = new AttackEffect(newModifier.statModified, newModifier.statChangeType, AttackEffect.EffectTime.PostAttack, newModifier.statusEffectType, false, false, false, 1, 1, newModifier.modifierAmount, combatManagerScript);
                    effect.typeOfEffect = AttackEffect.TypeOfEffect.InflictStunned;
                    effect.attackEffectDuration = AttackEffect.AttackEffectDuration.Temporary;

                    // Add stun attack effect to each attack if offensive move
                    foreach(MonsterAttack attack in monster.ListOfMonsterAttacks)
                    {
                        if (attack.monsterAttackType == MonsterAttack.MonsterAttackType.Attack)
                        {
                            attack.ListOfAttackEffects.Add(effect);
                            if (attack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.AllTargets)
                            {
                                effect.effectTime = AttackEffect.EffectTime.DuringAttack;
                            }
                        }
                    }

                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} gained 25% chance to stun on hit from {newModifier.modifierName}!");
                    break;

                default:
                    break;
            }
        }
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
        ListOfUnlockedNodes.Clear();
        ListOfLockedNodes.Clear();

        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    // This function resets all locked nodes to restart game
    public void RestartGame()
    {
        rerollAmount = 3;

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
        ListOfUnlockedNodes.Clear();
        ListOfLockedNodes.Clear();
        ListOfCurrentModifiers.Clear();
        ListOfCurrentMonsters.Clear();
        ListOfEnemyBattleMonsters.Clear();

        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    #region Old Code
    //
    /*
    public void InitiateSceneData2()
    {
        routeText = AdventureMenu.GetComponentInChildren<TextMeshProUGUI>();
        subScreenMenuText = SubscreenMenu.GetComponentInChildren<TextMeshProUGUI>();
        nodeSelectionTargeter = GameObject.FindGameObjectWithTag("Targeter");

        if (adventureBegin == false)
        {
            adventureBoss = currentSelectedAdventure.adventureBoss;
            ListOfAllNodes = GameObject.FindGameObjectsWithTag("Node");
            foreach (GameObject node in ListOfAllNodes)
            {
                node.SetActive(false);
            }

            if (NodeToReturnTo == null)
            {
                foreach (GameObject node in ListOfAllNodes)
                {
                    node.SetActive(true);
                    if (node.GetComponent<CreateNode>().nodeLocked)
                    {
                        ListOfLockedNodes.Add(node);
                    }
                }
            }
        }

        adventureBegin = true;

        foreach (GameObject node in ListOfUnlockedNodes)
        {
            if (node != null)
            {
                node.SetActive(true);
                node.GetComponent<CreateNode>().nodeLocked = false;
                node.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        //
        foreach (GameObject node in ListOfLockedNodes)
        {
            if (node != null)
            {
                node.SetActive(true);
                node.GetComponent<CreateNode>().nodeLocked = true;
                node.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }

        //
        if (NodeToReturnTo != null)
        {
            cachedSelectedNode = NodeToReturnTo;
            ActivateNextNode();
        }

    }
    */
    #endregion

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
                ListOfUnlockedNodes.Add(node);
                nodeSelectionTargeter.transform.position = ListOfUnlockedNodes[0].GetComponent<CreateNode>().selectedPosition.transform.position;
            }
            //
            foreach (GameObject node in cachedSelectedNode.GetComponent<CreateNode>().nodesToLock)
            {
                node.GetComponent<CreateNode>().nodeLocked = true;
                node.GetComponent<CreateNode>().SetNodeState(CreateNode.NodeState.Locked);
                ListOfLockedNodes.Add(node);
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

    // Unload adventure mode data
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
