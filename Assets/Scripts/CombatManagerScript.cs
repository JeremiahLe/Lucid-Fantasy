using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class CombatManagerScript : MonoBehaviour
{
    public List<GameObject> BattleSequence;
    public GameObject[] MonstersInBattle;

    private TextMeshProUGUI CombatOrderTextList;
    private TextMeshProUGUI BattleStartTextPopup;
    public MessageManager CombatLog;

    // For Later
    //public List<Action> BattleActions;

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        FadeText(BattleStartTextPopup);
        GetAllMonstersInBattle();
    }

    // This function initializes all components at start
    public void InitializeComponents()
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            switch (child.name)
            {
                case "BattleStartText":
                    BattleStartTextPopup = child.GetComponent<TextMeshProUGUI>();
                    break;

                case "CombatOrderText":
                    CombatOrderTextList = child.GetComponent<TextMeshProUGUI>();
                    break;

                case "CombatManagerObj":
                    CombatLog = GetComponent<MessageManager>();
                    break;

                default:
                    Debug.Log($"Missing a text object reference? {child.name}", this);
                    break;
            }
        }
    }

    // This function adds all monsters in battle into a list of monsters
    public void GetAllMonstersInBattle()
    {
        MonstersInBattle = GameObject.FindGameObjectsWithTag("Monster");

        foreach (GameObject monster in MonstersInBattle)
        {
            BattleSequence.Add(monster);
        }

        SortMonsterBattleSequence(); // This should be called after the foreach loop, meaning all monsters in battle have been added to list
    }

    // This function sorts the monster battle sequence by speed after all monsters have been identified and added to list
    public void SortMonsterBattleSequence()
    {
        BattleSequence = BattleSequence.OrderByDescending(Monster => Monster.GetComponent<CreateMonster>().monster.speed).ToList();
        CombatOrderTextList.text = ($"Combat Order:\n");

        for (int i = 0; i < BattleSequence.Count; i++)
        {
            Monster monster = BattleSequence[i].GetComponent<CreateMonster>().monster;
            CombatOrderTextList.text += ($"Monster {i + 1}: {monster.name} || Speed: {monster.speed}\n");
            CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} has joined the battle!"); // Update Combat Log with all monsters in battle
        }
    }

    // This function fades text passed in
    public void FadeText(TextMeshProUGUI textToFade)
    {
        textToFade.CrossFadeAlpha(0, 1f, true);
    }
}

