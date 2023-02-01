using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;

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
    public InventoryManager inventoryManager;

    public TextMeshProUGUI rewardName;
    public TextMeshProUGUI rewardDescription;
    public enum TypeOfMonsterSelect { View, ReceiveItem, PreBattle }
    public TypeOfMonsterSelect typeOfMonsterSelect;

    public bool selectable = true;
    public bool selected;

    public Color32 baseColor;
    public Color32 selectedColor;

    [Header("Monster Select Card")]
    public Image monsterSelectCardBase;

    public TMP_Dropdown monsterRowSelect;

    public TextMeshProUGUI monsterSelectionIndex;
    public Image monsterSelectMiniSprite;
    public TextMeshProUGUI monsterSelectData;

    [BoxGroup("Elements")]
    public Color32 windColor;
    [BoxGroup("Elements")]
    public Color32 shadowColor;
    [BoxGroup("Elements")]
    public Color32 waterColor;
    [BoxGroup("Elements")]
    public Color32 soundColor;
    [BoxGroup("Elements")]
    public Color32 fireColor;
    [BoxGroup("Elements")]
    public Color32 stoneColor;
    [BoxGroup("Elements")]
    public Color32 electricColor;
    [BoxGroup("Elements")]
    public Color32 elixirColor;
    [BoxGroup("Elements")]
    public Color32 earthColor;
    [BoxGroup("Elements")]
    public Color32 timeColor;
    [BoxGroup("Elements")]
    public Color32 lightColor;

    public void Awake()
    {
        rewardImage = GetComponent<Image>();
    }

    public void InitializeSelectable(AdventureManager _adventureManager, TypeOfMonsterSelect _typeOfMonsterSelect)
    {
        adventureManager = _adventureManager;
        subscreenManager = _adventureManager.subscreenManager;
        monsterStatScreenScript = subscreenManager.monsterStatScreenScript;
        typeOfMonsterSelect = _typeOfMonsterSelect;

        switch (typeOfMonsterSelect)
        {
            case TypeOfMonsterSelect.View:
                monsterRowSelect.gameObject.SetActive(false);
                break;

            case TypeOfMonsterSelect.ReceiveItem:
                monsterRowSelect.gameObject.SetActive(false);
                break;

            case TypeOfMonsterSelect.PreBattle:
                monsterRowSelect.interactable = true;
                break;

            default:
                break;
        }
    }

    public void InitializeMonsterSelectCardData(Monster monster)
    {
        monsterReward = monster;
        monsterSelectData.text =
            ($"{monsterReward.name}\n" +
            $"Lvl: {monsterReward.level}\n" +
            $"HP: {monsterReward.health}/{monsterReward.maxHealth}");

        monsterSelectMiniSprite.sprite = monsterReward.baseSprite;

        monsterSelectCardBase.color = GetElementColor(monsterReward.monsterElement);
    }

    public Color32 GetElementColor(ElementClass element)
    {
        switch (element.element)
        {
            case ElementClass.MonsterElement.Fire:
                return fireColor;
            case ElementClass.MonsterElement.Water:
                return waterColor;
            case ElementClass.MonsterElement.Earth:
                return earthColor;
            case ElementClass.MonsterElement.Wind:
                return windColor;
            case ElementClass.MonsterElement.Shadow:
                return shadowColor;
            case ElementClass.MonsterElement.Neutral:
                return Color.gray;
            case ElementClass.MonsterElement.None:
                return Color.gray;
            case ElementClass.MonsterElement.Light:
                return lightColor;
            case ElementClass.MonsterElement.Time:
                return timeColor;
            case ElementClass.MonsterElement.Elixir:
                return elixirColor;
            case ElementClass.MonsterElement.Electric:
                return electricColor;
            case ElementClass.MonsterElement.Stone:
                return stoneColor;
            case ElementClass.MonsterElement.Sound:
                return soundColor;
            default:
                return Color.gray;
        }
    }

    public void SelectReward()
    {
        adventureManager.routeText.text = ($"Select Destination...");

        if (selectable)
        {
            if (rewardType == AdventureManager.RewardType.Monster)
            {
                if (adventureManager.ListOfCurrentMonsters.Count == adventureManager.playerMonsterLimit)
                {
                    adventureManager.subScreenMenuText.text = ($"Chimeric limit reached!");
                    return;
                }

                monsterReward.monsterIsOwned = true;

                adventureManager.ListOfCurrentMonsters.Add(monsterReward);

                adventureManager.ListOfAllMonsters.Add(monsterReward);

                monsterReward = null;

                adventureManager.SubscreenMenu.SetActive(false);

                adventureManager.ActivateNextNode();
            }
            else if (rewardType == AdventureManager.RewardType.Modifier)
            {
                adventureManager.ListOfCurrentModifiers.Add(modifierReward);

                adventureManager.ApplyPassiveModifiers(modifierReward);

                modifierReward = null;

                adventureManager.ResetModifierList();

                adventureManager.SubscreenMenu.SetActive(false);

                adventureManager.ActivateNextNode();
            }
            else if (rewardType == AdventureManager.RewardType.Equipment)
            {
                adventureManager.ListOfCurrentEquipment.Add(modifierReward);

                modifierReward = null;

                adventureManager.ResetEquipmentList();

                adventureManager.SubscreenMenu.SetActive(false);

                adventureManager.ActivateNextNode();
            }
        }
    }

    public void Interact()
    {
        switch (typeOfMonsterSelect)
        {
            case TypeOfMonsterSelect.View:
                DisplayMonsterStats();
                break;

            case TypeOfMonsterSelect.ReceiveItem:
                SelectMonsterToReceiveConsumableHealing();
                break;

            case TypeOfMonsterSelect.PreBattle:
                SelectMonsterForBattle();
                break;

            default:
                break;
        }

    }

    public void SelectMonsterToReceiveConsumableHealing()
    {
        if (monsterReward == null)
            return;

        Item currentItem = monsterStatScreenScript.monstersSubScreenManager.currentItem;

        if (currentItem == null)
            return;

        if (monsterReward.health == monsterReward.maxHealth)
        {
            // Text = "Chimeric is already full health! It cannot be healed."
            Debug.Log($"{monsterReward} is already at full health!");
            return;
        }

        adventureManager.HealMonster(monsterReward, currentItem.modifierAmount);
    }

    public void SetTextRarityColor()
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

    public void SelectMonsterForBattle()
    {
        if (typeOfMonsterSelect != TypeOfMonsterSelect.PreBattle)
            return;

        if (selectable)
        {
            if (!selected)
            {
                selected = true;

                adventureManager.ListOfAllyBattleMonsters.Add(monsterReward);

                AssignMonsterRowPosition();

                adventureManager.subScreenMenuText.text = ($"Current Chimerics: {adventureManager.ListOfAllyBattleMonsters.Count}/{adventureManager.randomBattleMonsterLimit}");

                monsterSelectionIndex.text = ($"{adventureManager.ListOfAllyBattleMonsters.IndexOf(monsterReward) + 1}");
            }
            else
            {
                selected = false;

                adventureManager.ListOfAllyBattleMonsters.Remove(monsterReward);

                adventureManager.subScreenMenuText.text = ($"Current Chimerics: {adventureManager.ListOfAllyBattleMonsters.Count}/{adventureManager.randomBattleMonsterLimit}");

                monsterSelectionIndex.text = ("");

                // Adjust other monsters index
                foreach (GameObject monsterSlot in subscreenManager.monstersSelectPanelManager.monsterSelectCards)
                {
                    if (monsterSlot != this.gameObject)
                    {
                        CreateReward monsterComponent = monsterSlot.GetComponent<CreateReward>();
                        if (monsterComponent.selected)
                            monsterComponent.monsterSelectionIndex.text = ($"{adventureManager.ListOfAllyBattleMonsters.IndexOf(monsterComponent.monsterReward) + 1}");
                    }
                }
            }
        }
    }

    public void GoToBattleScene()
    {
        if (selectable)
        {
            if (adventureManager.ListOfAllyBattleMonsters.Count == 0)
            {
                adventureManager.subScreenMenuText.text = ($"Please select atleast one Chimeric.");
                return;
            }
            else
            if (adventureManager.ListOfAllyBattleMonsters.Count > adventureManager.randomBattleMonsterLimit)
            {
                adventureManager.subScreenMenuText.text = ($"Too many Chimerics selected!");
                return;
            }

            adventureManager.GoToBattleScene();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (rewardType == AdventureManager.RewardType.Monster && monsterReward != null)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                DisplayMonsterStats();
            }
        }
    }

    public void DisplayMonsterStats()
    {
        subscreenManager.monsterStatsWindow.SetActive(true);

        monsterStatScreenScript.DisplayMonsterStatScreenStats(monsterReward);

        monsterStatScreenScript.monstersSubScreenManager = subscreenManager.monstersSubScreenManager;

        inventoryManager = monsterStatScreenScript.gameObject.GetComponent<InventoryManager>();

        inventoryManager.currentMonster = monsterReward;
    }

    public void AssignMonsterRowPosition()
    {
        string monsterRowPosition = monsterRowSelect.captionText.text;

        switch (monsterRowPosition)
        {
            case ("Defensive"):
                monsterReward.cachedMonsterRowPosition = CreateMonster.MonsterStance.Defensive;
                break;

            case ("Neutral"):
                monsterReward.cachedMonsterRowPosition = CreateMonster.MonsterStance.Neutral;
                break;

            case ("Aggressive"):
                monsterReward.cachedMonsterRowPosition = CreateMonster.MonsterStance.Aggressive;
                break;

            default:
                break;
        }
    }
}
