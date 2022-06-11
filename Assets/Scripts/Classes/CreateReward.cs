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

    public GameObject StatScreenWindowGameObject;
    public TextMeshProUGUI StatScreenWindowText;

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
        adventureManager.routeText.text = ($"Select Destination...");

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
            else if (rewardType == AdventureManager.RewardType.Modifier)
            {
                adventureManager.ListOfCurrentModifiers.Add(modifierReward);
                modifierReward = null;
                adventureManager.ResetModifierList();
                adventureManager.SubscreenMenu.SetActive(false);
                adventureManager.ActivateNextNode();
            }
            else if (rewardType == AdventureManager.RewardType.Equipment)
            {
                if (!selected)
                {
                    foreach (GameObject monsterEquipmentSelection in subscreenManager.listOfRewardSlots)
                    {
                        monsterEquipmentSelection.GetComponent<CreateReward>().rewardImage.sprite = monsterEquipmentSelection.GetComponent<CreateReward>().modifierReward.baseSprite;
                        monsterEquipmentSelection.GetComponent<CreateReward>().selected = false;
                    }

                    selected = true;
                    rewardImage = GetComponent<Image>();
                    rewardImage.sprite = selectedSprite;
                    adventureManager.currentSelectedEquipment = modifierReward;
                }
                else
                {
                    selected = false;
                    rewardImage = GetComponent<Image>();
                    rewardImage.sprite = modifierReward.baseSprite;
                    adventureManager.currentSelectedEquipment = null;
                }
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

    // 
    public void SelectMonsterForEquipment()
    {
        if (selectable)
        {
            if (!selected)
            {
                foreach (GameObject monsterEquipmentSelection in subscreenManager.listOfMonsterSlotsEquipment)
                {
                    if (monsterEquipmentSelection.activeInHierarchy)
                    {
                        monsterEquipmentSelection.GetComponent<CreateReward>().rewardImage.sprite = monsterEquipmentSelection.GetComponent<CreateReward>().monsterReward.baseSprite;
                        monsterEquipmentSelection.GetComponent<CreateReward>().selected = false;
                    }
                }

                selected = true;
                rewardImage = GetComponent<Image>();
                rewardImage.sprite = selectedSprite;
                adventureManager.currentSelectedMonsterForEquipment = monsterReward;
            }
            else
            {
                selected = false;
                rewardImage = GetComponent<Image>();
                rewardImage.sprite = monsterReward.baseSprite;
                adventureManager.currentSelectedMonsterForEquipment = null;
            }
        }
    }

    //
    public void ConfirmEquipment()
    {
        if (adventureManager.currentSelectedEquipment != null && adventureManager.currentSelectedMonsterForEquipment != null)
        {
            adventureManager.currentSelectedMonsterForEquipment.ListOfModifiers.Add(adventureManager.currentSelectedEquipment);
            subscreenManager.ShowAlliedMonstersAvailableEquipment(false);
            subscreenManager.ConfirmEquipmentButton.SetActive(false);

            modifierReward = null;
            adventureManager.ResetEquipmentList();
            adventureManager.SubscreenMenu.SetActive(false);
            adventureManager.ActivateNextNode();
        }
        else
        {
            adventureManager.subScreenMenuText.text = ($"Please select 1 equipment and monster.");
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

    public float delayTime = 0.01f;
    public float currentTime = 0.0f;
    public bool windowShowing = false;

    // Check mouse hover
    private void OnMouseOver()
    {
        if (rewardType == AdventureManager.RewardType.Monster)
        {
            if (currentTime >= delayTime)
            {
                if (!windowShowing)
                {
                    //DisplayStatScreenWindow(true);
                    windowShowing = true;
                }
            }
            else
            {
                currentTime += Time.deltaTime;
            }
        }
    }

    // This function passes in the new target to the combatManager
    private void OnMouseExit()
    {
        windowShowing = false;
        currentTime = 0.0f;
        DisplayStatScreenWindow(false);
    }

    
    public void DisplayStatScreenWindow(bool showWindow)
    {
        if (showWindow)
        {
            StatScreenWindowGameObject.SetActive(true);
            StatScreenWindowText.text =
                (
                $"\nPhysical Attack: {monsterReward.physicalAttack}" +
                $"\nMagic Attack: {monsterReward.magicAttack}" +
                $"\nPhysical Defense: {monsterReward.physicalDefense}" +
                $"\nMagic Defense: {monsterReward.magicDefense}" +
                $"\nEvasion: {monsterReward.evasion}" +
                $"\nCrit Chance: {monsterReward.critChance}");
        }
        else
        if (!showWindow)
        {
            StatScreenWindowGameObject.SetActive(false);
            StatScreenWindowText.text = "";
        }
    }
    
}
