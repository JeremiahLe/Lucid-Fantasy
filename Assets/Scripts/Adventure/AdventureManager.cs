using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using TMPro;

public class AdventureManager : MonoBehaviour
{
    [Title("Pre-Adventure Components")]
    public bool adventureBegin = false;
    public GameObject ConfirmAdventureMenu;
    public SceneButtonManager sceneButtonManager;

    [Title("Adventure Components")]
    public GameObject nodeSelectionTargeter;
    public bool AdventureMode = false;
    public string adventureSceneName;
    public Adventure currentSelectedAdventure;

    public GameObject currentSelectedNode;
    public GameObject cachedSelectedNode;
    public CreateNode NodeComponent;
    public Monster adventureBoss;

    public GameObject AdventureMenu;
    public GameObject SubscreenMenu;

    public TextMeshProUGUI routeText;
    public TextMeshProUGUI subScreenMenuText;

    public SubscreenManager subscreenManager;

    public GameObject RewardSlotOne;
    public GameObject RewardSlotTwo;
    public GameObject RewardSlotThree;

    [Title("Adventure - Player Components")]
    public int rerollAmount;
    public int playerGold;

    public List<Monster> ListOfCurrentMonsters;
    public List<Modifier> ListOfCurrentModifiers;

    [Title("Other Adventure Modules")]
    public enum RewardType { Monster, Modifier }
    public RewardType currentRewardType;

    public Monster currentHoveredRewardMonster;
    public Modifier currentHoveredRewardModifier;

    public List<GameObject> ListOfUnlockedNodes;
    public List<GameObject> ListOfLockedNodes;

    public GameObject[] ListOfAllNodes;
    public bool BossDefeated = false;
    public bool BossBattle = false;

    [Title("Monster Lists")]
    public List<Monster> ListOfAvailableRewardMonsters;
    public List<Modifier> ListOfAvailableRewardModifiers;

    public List<Modifier> DefaultListOfAvailableRewardModifiers;

    [Title("Pre-Battle Setup")]
    public int randomBattleMonsterCount;
    public int randomBattleMonsterLimit;

    [Title("Adventure - Battle Setup and Components")]
    public GameObject NodeToReturnTo;
    public GameObject CombatManagerObject;
    public CombatManagerScript combatManagerScript;

    public List<Monster> ListOfEnemyBattleMonsters;
    public List<Monster> ListOfAllyBattleMonsters;
    public List<Modifier> ListOfEnemyModifiers;

    public void Start()
    {
    }

    public void Awake()
    {
        sceneButtonManager = GetComponent<SceneButtonManager>();
        CopyDefaultModifierList();
    }
    
    //
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
                statModified = item.statModified
            });
        }
    }

    //
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
                statModified = item.statModified
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

    //
    public void GoToBattleScene()
    {
        DontDestroyOnLoad(gameObject);

        // save nodes
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
            //Destroy(node);
        }

        NodeToReturnTo = cachedSelectedNode;
        if (NodeToReturnTo.GetComponent<CreateNode>().nodeType == CreateNode.NodeType.Boss)
        {
            BossBattle = true;
        }

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
        if (scene.name == "StartScreen")
        {
            Destroy(gameObject);
        }
        else if (scene.name == "AdventureBeginScene")
        {
            //Destroy(gameObject);
        }
        else if (scene.name != "SetupCombatScene")
        {
            AdventureMenu = GameObject.FindGameObjectWithTag("AdventureMenu");
            SubscreenMenu = GameObject.FindGameObjectWithTag("SubscreenMenu");
            subscreenManager = SubscreenMenu.GetComponent<SubscreenManager>();
            SubscreenMenu.SetActive(false);
            InitiateSceneData();
            adventureSceneName = SceneManager.GetActiveScene().name;
        }
        else
        if (scene.name == "SetupCombatScene")
        {
            CombatManagerObject = GameObject.FindGameObjectWithTag("GameController");
            combatManagerScript = CombatManagerObject.GetComponent<CombatManagerScript>();
            combatManagerScript.adventureMode = true;
            combatManagerScript.previousSceneName = adventureSceneName;
        }
    }

    // 
    public void InitiateSceneData()
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
            node.SetActive(true);
            node.GetComponent<CreateNode>().nodeLocked = false;
            node.GetComponent<SpriteRenderer>().color = Color.white;
        }
        //
        foreach (GameObject node in ListOfLockedNodes)
        {
            node.SetActive(true);
            node.GetComponent<CreateNode>().nodeLocked = true;
            node.GetComponent<SpriteRenderer>().color = Color.red;
        }

        //
        if (NodeToReturnTo != null)
        {
            cachedSelectedNode = NodeToReturnTo;
            ActivateNextNode();
        }
    }

    // this function runs on click
    public void CheckNodeLocked()
    {
        if (currentSelectedNode != null)
        {
            NodeComponent = currentSelectedNode.GetComponent<CreateNode>();
        }

        if (NodeComponent.nodeLocked && !BossDefeated)
        {
            routeText.text = ($"Route is locked! Select previous route first!");
            return;
        }

        if (!BossDefeated)
        {
            ShowSubscreenMenu();
        }
    }

    //
    public void ActivateNextNode()
    {
        foreach (GameObject node in cachedSelectedNode.GetComponent<CreateNode>().nodesToUnlock)
        {
            node.GetComponent<CreateNode>().nodeLocked = false;
            node.GetComponent<SpriteRenderer>().color = Color.white;
            ListOfUnlockedNodes.Add(node);
        }
        //
        foreach (GameObject node in cachedSelectedNode.GetComponent<CreateNode>().nodesToLock)
        {
            node.GetComponent<CreateNode>().nodeLocked = true;
            node.GetComponent<SpriteRenderer>().color = Color.red;
            ListOfLockedNodes.Add(node);
        }

        if (BossDefeated)
        {
            routeText.text = ($"You Win!");
        }
    }

    //
    public void ShowSubscreenMenu()
    {
        SubscreenMenu.SetActive(true);
        cachedSelectedNode = currentSelectedNode;

        switch (NodeComponent.nodeType)
        {
            case CreateNode.NodeType.Start:
                subScreenMenuText.text = ($"Select starting monster...");
                subscreenManager.LoadRewardSlots(RewardType.Monster);
                break;

            case CreateNode.NodeType.RandomReward:
                currentRewardType = ReturnRandomRewardType();
                subScreenMenuText.text = ($"Select {currentRewardType.ToString()}...");
                subscreenManager.LoadRewardSlots(currentRewardType);
                break;

            case CreateNode.NodeType.ModifierReward:
                currentRewardType = RewardType.Modifier;
                subScreenMenuText.text = ($"Select {currentRewardType.ToString()}...");
                subscreenManager.LoadRewardSlots(RewardType.Modifier);
                break;

            case CreateNode.NodeType.MonsterReward:
                currentRewardType = RewardType.Monster;
                subScreenMenuText.text = ($"Select {currentRewardType.ToString()}...");
                subscreenManager.LoadRewardSlots(RewardType.Monster);
                break;

            case CreateNode.NodeType.RandomCombat:
                subScreenMenuText.text = ($"Select monsters and begin battle...");
                subscreenManager.HideRewardSlots();
                subscreenManager.LoadRandomBattle();
                break;

            case CreateNode.NodeType.Boss:
                subScreenMenuText.text = ($"Select monsters and begin final battle...");
                subscreenManager.HideRewardSlots();
                subscreenManager.LoadRandomBattle();
                break;

            default:
                break;
        }
    }

    RewardType ReturnRandomRewardType()
    {
        RewardType randReward = (RewardType)Random.Range(0, 1);
        return randReward;
    }


}
