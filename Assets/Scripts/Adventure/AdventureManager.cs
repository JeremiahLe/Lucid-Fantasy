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
    public GameObject ConfirmAdventureMenu;
    public SceneButtonManager sceneButtonManager;

    [Title("Adventure Components")]
    public bool AdventureMode = false;
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

    [Title("Monster Lists")]
    public List<Monster> ListOfAvailableRewardMonsters;
    public List<Modifier> ListOfAvailableRewardModifiers;

    public void Start()
    {
        
    }

    public void Awake()
    {
        sceneButtonManager = GetComponent<SceneButtonManager>();
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
                sceneButtonManager.GoToSceneCoroutine("BasicAdventureHardScene");
                break;

            default:
                break;
        }
    }

    // When adventure scene is loaded, get scene data
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AdventureMenu = GameObject.FindGameObjectWithTag("AdventureMenu");
        SubscreenMenu = GameObject.FindGameObjectWithTag("SubscreenMenu");
        subscreenManager = SubscreenMenu.GetComponent<SubscreenManager>();
        SubscreenMenu.SetActive(false);
        InitiateSceneData();
    }

    // 
    public void InitiateSceneData()
    {
        routeText = AdventureMenu.GetComponentInChildren<TextMeshProUGUI>();
        subScreenMenuText = SubscreenMenu.GetComponentInChildren<TextMeshProUGUI>();
    }

    // this function runs on click
    public void CheckNodeLocked()
    {
        NodeComponent = currentSelectedNode.GetComponent<CreateNode>();

        if (NodeComponent.nodeLocked)
        {
            routeText.text = ($"Route is locked! Select previous route first!");
            return;
        }

        ShowSubscreenMenu();
    }

    //
    public void ActivateNextNode()
    {
        foreach (GameObject node in cachedSelectedNode.GetComponent<CreateNode>().nodesToUnlock)
        {
            node.GetComponent<CreateNode>().nodeLocked = false;
            node.GetComponent<SpriteRenderer>().color = Color.white;
        }
        //
        foreach (GameObject node in cachedSelectedNode.GetComponent<CreateNode>().nodesToLock)
        {
            node.GetComponent<CreateNode>().nodeLocked = true;
            node.GetComponent<SpriteRenderer>().color = Color.red;
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
                break;

            case CreateNode.NodeType.ModifierReward:
                currentRewardType = RewardType.Modifier;
                subScreenMenuText.text = ($"Select {currentRewardType.ToString()}...");
                break;

            case CreateNode.NodeType.MonsterReward:
                currentRewardType = RewardType.Monster;
                subScreenMenuText.text = ($"Select {currentRewardType.ToString()}...");
                break;

            case CreateNode.NodeType.RandomCombat:
                subScreenMenuText.text = ($"Begin battle?");
                break;

            case CreateNode.NodeType.Boss:
                subScreenMenuText.text = ($"Begin final battle?");
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
