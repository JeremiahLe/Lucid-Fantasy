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

    // For Later
    //public List<Action> BattleActions;

    // Start is called before the first frame update
    void Start()
    {
        GetAllMonstersInBattle();
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
        CombatOrderTextList = GetComponentInChildren<Canvas>().GetComponentInChildren<TextMeshProUGUI>();
        CombatOrderTextList.text = ($"Combat Order:\n");

        for (int i = 0; i < BattleSequence.Count; i++)
        {
            Monster monster = BattleSequence[i].GetComponent<CreateMonster>().monster;
            CombatOrderTextList.text += ($"Monster {i + 1}: {monster.name} || Speed: {monster.speed}\n");
        }
    }
}

