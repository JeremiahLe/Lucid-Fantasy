using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateReward : MonoBehaviour
{
    public Image rewardImage;

    public Monster monsterReward;
    public Modifier modifierReward;

    public AdventureManager.RewardType rewardType;
    public SubscreenManager subscreenManager;
    public AdventureManager adventureManager;

    public TextMeshProUGUI rewardName;
    public TextMeshProUGUI rewardDescription;

    public bool selected;
    public Sprite selectedSprite;

    public bool selectable = true;

    public void Awake()
    {
        //adventureManager = subscreenManager.adventureManager;
        rewardImage = GetComponent<Image>();
    }

    // Select reward
    public void SelectReward()
    {
        if (selectable)
        {
            if (rewardType == AdventureManager.RewardType.Monster)
            {
                adventureManager.ListOfCurrentMonsters.Add(monsterReward);
                adventureManager.ListOfAllMonsters.Add(monsterReward);
                monsterReward = null;
                adventureManager.SubscreenMenu.SetActive(false);
                adventureManager.ActivateNextNode();
            }
            else
            {
                adventureManager.ListOfCurrentModifiers.Add(modifierReward);
                modifierReward = null;
                adventureManager.ResetModifierList();
                adventureManager.SubscreenMenu.SetActive(false);
                adventureManager.ActivateNextNode();
            }
        }
    }

    // 
    public void SelectMonsterForBattle()
    {
        if (selectable)
        {
            if (!selected)
            {
                selected = true;
                rewardImage = GetComponent<Image>();
                rewardImage.sprite = selectedSprite;
                adventureManager.ListOfAllyBattleMonsters.Add(monsterReward);
                adventureManager.subScreenMenuText.text = ($"Current monsters: {adventureManager.ListOfAllyBattleMonsters.Count}/{adventureManager.randomBattleMonsterLimit}");
            }
            else
            {
                selected = false;
                rewardImage = GetComponent<Image>();
                rewardImage.sprite = monsterReward.baseSprite;
                adventureManager.ListOfAllyBattleMonsters.Remove(monsterReward);
                adventureManager.subScreenMenuText.text = ($"Current monsters: {adventureManager.ListOfAllyBattleMonsters.Count}/{adventureManager.randomBattleMonsterLimit}");
            }
        }
    }

    // Begin battle
    public void GoToBattleScene()
    {
        if (selectable)
        {
            if (adventureManager.ListOfAllyBattleMonsters.Count == 0)
            {
                adventureManager.subScreenMenuText.text = ($"Please select atleast one monster.");
                return;
            }
            else
            if (adventureManager.ListOfAllyBattleMonsters.Count > adventureManager.randomBattleMonsterLimit)
            {
                adventureManager.subScreenMenuText.text = ($"Too many monsters selected!");
                return;
            }

            adventureManager.GoToBattleScene();
        }
    }
}
