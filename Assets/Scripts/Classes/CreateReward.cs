using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CreateReward : MonoBehaviour, IPointerClickHandler
{
    public Image rewardImage;

    public Monster monsterReward;
    public Modifier modifierReward;

    public GameObject StatScreenWindowGameObject;
    public GameObject container;
    public TextMeshProUGUI StatScreenWindowText;

    public AdventureManager.RewardType rewardType;
    public SubscreenManager subscreenManager;
    public AdventureManager adventureManager;
    public MonsterStatScreenScript monsterStatScreenScript;

    public TextMeshProUGUI rewardName;
    public TextMeshProUGUI rewardDescription;

    public bool selected;
    public Sprite selectedSprite;
    public Sprite baseSprite;
    public bool selectable = true;

    public TMP_Dropdown monsterRowSelect;

    public void Awake()
    {
        rewardImage = GetComponent<Image>();
    }

    // This function handles the selection of rewards
    public void SelectReward()
    {
        adventureManager.routeText.text = ($"Select Destination...");

        if (selectable)
        {
            if (rewardType == AdventureManager.RewardType.Monster)
            {
                if (adventureManager.ListOfCurrentMonsters.Count == 4)
                {
                    adventureManager.subScreenMenuText.text = ($"Monster limit reached!");
                    return;
                }

                adventureManager.ListOfCurrentMonsters.Add(monsterReward);
                adventureManager.ListOfAllMonsters.Add(monsterReward);
                monsterReward = null;
                adventureManager.SubscreenMenu.SetActive(false);
                adventureManager.ActivateNextNode();
            }
            else if (rewardType == AdventureManager.RewardType.Modifier)
            {
                adventureManager.ListOfCurrentModifiers.Add(modifierReward);
                adventureManager.ApplyPassiveModifiers();
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

    // This function handles the appearance of the reward
    public void SetRarityColor()
    {
        switch (modifierReward.modifierRarity)
        {
            case Modifier.ModifierRarity.Common:
                //<#24d152>
                rewardName.text = ($"<b><#24d152>{modifierReward.modifierName}</color></b>" +
                    $"\n{modifierReward.modifierDescription}");
                break;

            case Modifier.ModifierRarity.Uncommon:
                //#5255b3<#c55fde>
                rewardName.text = ($"<b><#2596be>{modifierReward.modifierName}</color></b>" +
                    $"\n{modifierReward.modifierDescription}");
                break;

            case Modifier.ModifierRarity.Rare:
                //#<c55fde><#2596be>
                rewardName.text = ($"<b><#9925be>{modifierReward.modifierName}</color></b>" +
                    $"\n{modifierReward.modifierDescription}");
                break;

            case Modifier.ModifierRarity.Legendary:
                //#<f0a346>
                rewardName.text = ($"<b><#f0a346>{modifierReward.modifierName}</color></b>" +
                    $"\n{modifierReward.modifierDescription}");
                break;

            default:
                break;
        }
    }

    // This function selects a monster for battle
    public void SelectMonsterForBattle()
    {
        if (selectable)
        {
            if (!selected)
            {
                selected = true;
                rewardImage = container.GetComponent<Image>();
                rewardImage.sprite = selectedSprite;
                adventureManager.ListOfAllyBattleMonsters.Add(monsterReward);
                adventureManager.subScreenMenuText.text = ($"Current monsters: {adventureManager.ListOfAllyBattleMonsters.Count}/{adventureManager.randomBattleMonsterLimit}");
            }
            else
            {
                selected = false;
                rewardImage = container.GetComponent<Image>();
                rewardImage.sprite = baseSprite;
                adventureManager.ListOfAllyBattleMonsters.Remove(monsterReward);
                adventureManager.subScreenMenuText.text = ($"Current monsters: {adventureManager.ListOfAllyBattleMonsters.Count}/{adventureManager.randomBattleMonsterLimit}");
            }
        }
    }

    // This function selects a monster to recieve an equipment
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

    // This function confirms the player's equipment and monster selection
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

    // This function travels to the battle scene once the player selects their monsters
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

    // This function displays monster stats on right-click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (rewardType == AdventureManager.RewardType.Monster && monsterReward != null)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                subscreenManager.monsterStatsWindow.SetActive(true);
                monsterStatScreenScript.DisplayMonsterStatScreenStats(monsterReward);
            }
        }
    }

    // This function assigns the current monster slot a row position before the battle begins
    public void AssignMonsterRowPosition()
    {
        string monsterRowPosition = monsterRowSelect.captionText.text;

        switch (monsterRowPosition)
        {
            case ("Back Row"):
                monsterReward.cachedMonsterRowPosition = CreateMonster.MonsterRowPosition.BackRow;
                break;

            case ("Center Row"):
                monsterReward.cachedMonsterRowPosition = CreateMonster.MonsterRowPosition.CenterRow;
                break;

            case ("Front Row"):
                monsterReward.cachedMonsterRowPosition = CreateMonster.MonsterRowPosition.FrontRow;
                break;

            default:
                break;
        }
    }
}
