using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI CombatOrderTextList;
    public TextMeshProUGUI BattleStartTextPopup;
    public TextMeshProUGUI RoundStartTextPopup;

    public TextMeshProUGUI RoundCountText;

    public TextMeshProUGUI AllyTextList;
    public TextMeshProUGUI EnemyTextList;

    public GameObject monsterTargeter;
    public GameObject monsterTurnIndicator;

    public HUDAnimationManager HUDanimationManager;
    public CombatManagerScript combatManagerScript;

    public GameObject DetailedMonsterStatsWindow;
    //public GameObject TooltipWindow;

    Monster monsterRef;

    [Header("List of Sprites")]
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite physicalAttackSprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite physicalDefenseSprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite magicAttackSprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite magicDefenseSprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite speedSprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite evasionSprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite accuracySprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite critChanceSprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite critDamageSprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite poisonedUISprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite burningUISprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite dazedUISprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite weakenedUISprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite stunnedUISprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite crippledUISprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite damageImmuneUISprite;
    [AssetSelector(Paths = "Assets/Sprites/UI/Status Effects and Stats")]
    public Sprite debuffsImmuneUISprite;

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
    }

    // This function initializes the gameObjects components
    public void InitializeComponents()
    {
        combatManagerScript = GetComponent<CombatManagerScript>();
        HUDanimationManager = GetComponent<HUDAnimationManager>();

        InitializeUI();
    }

    // This function initializes the on-screen elements
    public void InitializeUI()
    {
        RoundStartTextPopup.CrossFadeAlpha(0f, 0.01f, false); // set round start invisible at start
        FadeText(BattleStartTextPopup); // fade battle start text
        RoundStartTextPopup.CrossFadeAlpha(1f, 1f, false); // fade in round start

        AllyTextList.text = ("Allies:\n");
        EnemyTextList.text = ("Enemies:\n");
    }

    // This function handles the monster turn indicator UI object
    public void InitiateMonsterTurnIndicator(GameObject currentMonsterTurn)
    {
        monsterTurnIndicator.SetActive(true);
        if (currentMonsterTurn != null)
        {
            monsterTurnIndicator.transform.position = new Vector3(currentMonsterTurn.transform.position.x, currentMonsterTurn.transform.position.y + 2.0f, currentMonsterTurn.transform.position.z);
        }
        else
        {
            monsterTurnIndicator.SetActive(false);
        }
    }

    // This function updates the UI elements on screen when called (monster init, list clearing/updating)
    public void UpdateMonsterList(List<GameObject> monsterList, Monster.AIType aIType)
    {
        Monster monsterRef = null;

        switch (aIType)
        {
            case Monster.AIType.Ally:
                AllyTextList.text = ("Allies:\n");
                foreach (GameObject monster in monsterList)
                {
                    monsterRef = monster.GetComponent<CreateMonster>().monsterReference;
                    if (monsterRef.name != "")
                    {
                        AllyTextList.text += ($"{monsterRef.name}, Lvl: {monsterRef.level}\n");
                    }
                }
                break;

            case Monster.AIType.Enemy:
                EnemyTextList.text = ("Enemies:\n");
                foreach (GameObject monster in monsterList)
                {
                    monsterRef = monster.GetComponent<CreateMonster>().monsterReference;
                    if (monsterRef.name != "")
                    {
                        EnemyTextList.text += ($"{monsterRef.name}, Lvl: {monsterRef.level}\n");
                    }
                }
                break;

            default:
                Debug.Log("Missing monster, list, or AIType reference?", this);
                break;
        }
    }

    // This function updates on-screen battle sequence - TODO - Implement Me
    public void UpdateBattleSequenceList(List<GameObject> currentBattleSequence)
    {

    }

    // This function fades text passed in
    public void FadeText(TextMeshProUGUI textToFade)
    {
        textToFade.CrossFadeAlpha(0, 1.25f, true);
    }

    // This function fades round text
    public void FadeRoundText()
    {
        RoundStartTextPopup.CrossFadeAlpha(0, .75f, true);
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

    // This function increments turn count
    public void IncrementRoundCount(int currentRound)
    {
        RoundCountText.text = ($"Round {currentRound}");
        RoundStartTextPopup.text = ($"Round {currentRound}");

        RoundStartTextPopup.CrossFadeAlpha(1f, .5f, true);
        Invoke("FadeRoundText", .75f);
    }

    // This is a startup function to hide everything then renable it on round start
    public void HideEverything()
    {
        EditCombatMessage(); // hide combat message

        AllyTextList.text = ""; // hide ally text list
        EnemyTextList.text = ""; // hide ally text list
        CombatOrderTextList.text = ""; // hide combat order text list

        RoundCountText.text = ""; // hide round counter text
    }

    //public void DisplayToolTipWindow(Modifier modifier)
    //{
    //    TooltipWindow.SetActive(true);
    //    TooltipWindow.transform.position = modifier.statusEffectIconGameObject.transform.position;
    //    TooltipWindow.GetComponentInChildren<TextMeshProUGUI>().text = 
    //        ($"{modifier.modifierSource}" +
    //        $"Duration: {modifier.modifierCurrentDuration} rounds" +
    //        $"Type: {modifier.statChangeType}");
    //}
}
