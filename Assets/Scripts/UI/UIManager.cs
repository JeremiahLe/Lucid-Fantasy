using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI CombatOrderTextList;
    public TextMeshProUGUI BattleStartTextPopup;

    public TextMeshProUGUI RoundCountText;

    public TextMeshProUGUI AllyTextList;
    public TextMeshProUGUI EnemyTextList;

    public GameObject monsterTargeter;

    public HUDAnimationManager HUDanimationManager;
    public CombatManagerScript combatManagerScript;

    Monster monsterRef;

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
        FadeText(BattleStartTextPopup);
        AllyTextList.text = ("Allies:\n");
        EnemyTextList.text = ("Enemies:\n");
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
        textToFade.CrossFadeAlpha(0, 1f, true);
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
    }
}
