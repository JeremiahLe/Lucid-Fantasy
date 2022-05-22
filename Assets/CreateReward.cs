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

    public void Awake()
    {
        //adventureManager = subscreenManager.adventureManager;
    }

    // Select reward
    public void SelectReward()
    {
        if (rewardType == AdventureManager.RewardType.Monster)
        {
            adventureManager.ListOfCurrentMonsters.Add(monsterReward);
            adventureManager.SubscreenMenu.SetActive(false);
            adventureManager.ActivateNextNode();
        }
        else
        {
            adventureManager.ListOfCurrentModifiers.Add(modifierReward);
            adventureManager.SubscreenMenu.SetActive(false);
            adventureManager.ActivateNextNode();
        }
    }
}
